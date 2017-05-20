using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Extensions;
using GridDominance.Shared.Screens.ScreenGame;

namespace GridDominance.Shared.Screens.NormalGameScreen.Background
{
	internal sealed class GridCellMembership
	{
		public Fraction Fraction = null;

		public float PowerCurr = 0;
		public float PowerNext = 0;

		public Cannon SpawnSource = null;
		public int SourceDistance = GDCellularBackground.SRC_DIST_INF;

		public bool BlockNorth = false;
		public bool BlockEast  = false;
		public bool BlockSouth = false;
		public bool BlockWest  = false;

		public bool IsNeutralDraining = false;
	}

	class GDCellularBackground : GameBackground, IGDGridBackground
	{
		private const int TILE_COUNT_X = GDConstants.GRID_WIDTH  + 2 * MAX_EXTENSION;
		private const int TILE_COUNT_Y = GDConstants.GRID_HEIGHT + 2 * MAX_EXTENSION;

		private const int MAX_EXTENSION = 2;
		
		public const int SRC_DIST_INF = 12;

		private const float MIN_TRANSFER_POWER     = 0.25f;
		private const float MIN_NEUTRALIZE_POWER   = 0.55f; 

		private const float SPAWNCELL_REGENERATION = 2.50f; // power per second
		private const float CELL_ATTACK_SPEED      = 1.05f; // power per second
		private const float CELL_OCCUPY_SPEED      = 0.35f; // power per second
		private const float CELL_DECAY_SPEED       = 0.70f; // power per second
		private const float CELL_NEUTRALIZE_SPEED  = 0.20f; // power per second

		private readonly GridCellMembership[,] _grid = new GridCellMembership[TILE_COUNT_X, TILE_COUNT_Y];
		private readonly FRectangle[,] _rects = new FRectangle[TILE_COUNT_X, TILE_COUNT_Y];
		
		public GDCellularBackground(GDGameScreen scrn) : base(scrn)
		{
			Initialize();
		}

		private void Initialize()
		{
			for (int x = 0; x < TILE_COUNT_X; x++)
			{
				for (int y = 0; y < TILE_COUNT_Y; y++)
				{
					_grid[x, y] = new GridCellMembership();
					if (x == 0) _grid[x, y].BlockWest = true;
					if (y == 0) _grid[x, y].BlockNorth = true;
					if (x == TILE_COUNT_X-1) _grid[x, y].BlockEast = true;
					if (y == TILE_COUNT_Y-1) _grid[x, y].BlockSouth = true;
				}
			}

			for (int ox = 0; ox < TILE_COUNT_X; ox++)
			{
				for (int oy = 0; oy < TILE_COUNT_Y; oy++)
				{
					var x = ox - MAX_EXTENSION; // array coords -> real coords
					var y = oy - MAX_EXTENSION;

					_rects[ox, oy] = new FRectangle(
						x * GDConstants.TILE_WIDTH, 
						y * GDConstants.TILE_WIDTH, 
						1 * GDConstants.TILE_WIDTH, 
						1 * GDConstants.TILE_WIDTH);
				}
			}
		}

		#region Draw

		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = MathHelper.Min(MAX_EXTENSION, FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / GDConstants.TILE_WIDTH));
			int extensionY = MathHelper.Min(MAX_EXTENSION, FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / GDConstants.TILE_WIDTH));

			for (int ox = -extensionX; ox < GDConstants.GRID_WIDTH + extensionX; ox++)
			{
				for (int oy = -extensionY; oy < GDConstants.GRID_HEIGHT + extensionY; oy++)
				{
					var x = ox + MAX_EXTENSION; // real coords -> array coords
					var y = oy + MAX_EXTENSION;

					if (x < 0) continue;
					if (y < 0) continue;
					if (x >= TILE_COUNT_X) continue;
					if (y >= TILE_COUNT_Y) continue;

					var color = GetGridColor(x, y);

					sbatch.DrawStretched(Textures.TexPixel, _rects[x, y], color);
					sbatch.DrawStretched(Textures.TexTileBorder, _rects[x, y], Color.White);

#if DEBUG
					if (DebugSettings.Get("DebugBackground"))
					{

						var tx = _rects[x, y].X + 8;
						var ty = _rects[x, y].Y + 8;

						sbatch.DrawString(
							Textures.DebugFontSmall,
							string.Format("({4}|{5})\n{0,2}: {1:000}\n[{2}]{3}", _grid[x, y].Fraction?.ToString() ?? "##", _grid[x, y].PowerCurr * 100, _grid[x, y].SourceDistance, _grid[x, y].IsNeutralDraining ? "D" : "", x, y),
							new Vector2(tx, ty),
							_grid[x, y].Fraction?.Color ?? Color.Black);

						if (_grid[x, y].SpawnSource != null)
							SimpleRenderHelper.DrawCross(sbatch, _rects[x + MAX_EXTENSION, y + MAX_EXTENSION], _grid[x, y].SpawnSource.Fraction.Color * 0.5f, 2);

						if (_grid[x, y].BlockNorth)
							sbatch.DrawLine(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 6, (x+1) * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 6, Color.Red, 4);

						if (_grid[x, y].BlockEast)
							sbatch.DrawLine((x+1) * GDConstants.TILE_WIDTH - 6, (y+1) * GDConstants.TILE_WIDTH, (x+1) * GDConstants.TILE_WIDTH - 6, y * GDConstants.TILE_WIDTH, Color.Red, 4);

						if (_grid[x, y].BlockSouth)
							sbatch.DrawLine((x+1) * GDConstants.TILE_WIDTH, (y+1) * GDConstants.TILE_WIDTH - 6, x * GDConstants.TILE_WIDTH, (y+1) * GDConstants.TILE_WIDTH - 6, Color.Red, 4);

						if (_grid[x, y].BlockWest)
							sbatch.DrawLine(x * GDConstants.TILE_WIDTH + 6, y * GDConstants.TILE_WIDTH, x * GDConstants.TILE_WIDTH + 6, (y+1) * GDConstants.TILE_WIDTH, Color.Red, 4);
					}
#endif
				}
			}
		}

		#endregion

		#region Update

		public override void Update(SAMTime gameTime, InputState state)
		{
			for (int x = 0; x < TILE_COUNT_X; x++)
			{
				for (int y = 0; y < TILE_COUNT_Y; y++)
				{
					UpdateCell(x, y, gameTime);
					
					if (_grid[x, y].SpawnSource != null)
					{
						UpdateSpawnCell(x, y, gameTime);
					}
				}
			}

			for (int x = 0; x < TILE_COUNT_X; x++)
			{
				for (int y = 0; y < TILE_COUNT_Y; y++)
				{
					_grid[x, y].PowerCurr = _grid[x, y].PowerNext;
				}
			}
		}

		private void UpdateCell(int x, int y, SAMTime gameTime)
		{
			List<GridCellMembership> ncells = new List<GridCellMembership>();
			if (!_grid[x, y].BlockNorth) ncells.Add(_grid[x + 0, y - 1]);
			if (!_grid[x, y].BlockEast)  ncells.Add(_grid[x + 1, y + 0]);
			if (!_grid[x, y].BlockSouth) ncells.Add(_grid[x + 0, y + 1]);
			if (!_grid[x, y].BlockWest)  ncells.Add(_grid[x - 1, y + 0]);

			// 1. pathological Case
			if (ncells.Count == 0)
			{
				_grid[x, y].SourceDistance = SRC_DIST_INF;
			}


			// 2. Update SourceDistance
			if (!ncells.Any(c => c.Fraction == _grid[x, y].Fraction))
			{
				_grid[x, y].SourceDistance = SRC_DIST_INF;
			}
			else
			{
				var newSD = ncells.Where(c => c.Fraction == _grid[x, y].Fraction).Min(c => c.SourceDistance + 1);
				if (newSD >= SRC_DIST_INF)
					_grid[x, y].SourceDistance = SRC_DIST_INF;
				else if (newSD <= _grid[x, y].SourceDistance)
					_grid[x, y].SourceDistance = newSD;
				else
					_grid[x, y].SourceDistance = SRC_DIST_INF;
			}

			// 3. GainPower from highest Neighbor
			var support = ncells.Where(c => c.Fraction != null && c.Fraction == _grid[x, y].Fraction && c.SourceDistance < SRC_DIST_INF && c.PowerCurr > MIN_TRANSFER_POWER).OrderBy(c => c.SourceDistance).FirstOrDefault();
			var attack  = ncells.Where(c => c.Fraction != null && c.Fraction != _grid[x, y].Fraction && c.SourceDistance < SRC_DIST_INF && c.PowerCurr > MIN_TRANSFER_POWER).OrderBy(c => c.SourceDistance).FirstOrDefault();

			if (support == null && attack == null)
			{
				// border of unsupported island
				LoosePower(x, y, gameTime.ElapsedSeconds * CELL_DECAY_SPEED);
			}
			else if (support != null && attack == null)
			{
				//improv agains none
				GainPower(support.Fraction, x, y, gameTime.ElapsedSeconds * CELL_OCCUPY_SPEED);
			}
			else if (support != null && attack != null && attack.SourceDistance > support.SourceDistance)
			{
				//improv against enemy
				GainPower(support.Fraction, x, y, gameTime.ElapsedSeconds * CELL_ATTACK_SPEED);
			}
			else if (support == null && attack != null)
			{
				//loose with no support
				GainPower(attack.Fraction, x, y, gameTime.ElapsedSeconds * CELL_DECAY_SPEED);
			}
			else if (support != null && attack != null && attack.SourceDistance < support.SourceDistance)
			{
				//loose with against enemy
				GainPower(attack.Fraction, x, y, gameTime.ElapsedSeconds * CELL_ATTACK_SPEED);
			}
			else if (support != null && attack != null && attack.SourceDistance == support.SourceDistance)
			{
				//contested cell
				if (_grid[x, y].PowerCurr > MIN_NEUTRALIZE_POWER || _grid[x, y].IsNeutralDraining)
				{
					// drain
					_grid[x, y].IsNeutralDraining = true;
					GainPower(attack.Fraction, x, y, gameTime.ElapsedSeconds * CELL_NEUTRALIZE_SPEED);
				}
				else
				{
					//gain
					GainPower(support.Fraction, x, y, gameTime.ElapsedSeconds * CELL_NEUTRALIZE_SPEED);
				}
			}
		}

		private void UpdateSpawnCell(int x, int y, SAMTime gameTime)
		{
			if (_grid[x, y].PowerCurr < 1f || _grid[x, y].Fraction != _grid[x, y].SpawnSource.Fraction)
			{
				// Regenerate spawn cells
				GainPower(_grid[x, y].SpawnSource.Fraction, x, y, gameTime.ElapsedSeconds * SPAWNCELL_REGENERATION);

				if (_grid[x, y].Fraction != null && _grid[x, y].Fraction.IsNeutral) _grid[x, y].PowerNext = 0f;

				if (_grid[x, y].PowerNext < 1f) _grid[x, y].SourceDistance = SRC_DIST_INF;
			}
			else
			{
				if (_grid[x, y].SpawnSource.CannonHealth.TargetValue < 0.95f)
					_grid[x, y].SourceDistance = SRC_DIST_INF;
				else
					_grid[x, y].SourceDistance = 0;
			}
		}

		private void GainPower(Fraction f, int x, int y, float power)
		{
			if (_grid[x, y].Fraction == null || _grid[x, y].Fraction.IsNeutral)
			{
				_grid[x, y].Fraction = f;
				_grid[x, y].PowerNext = power;
			}
			else if (f == _grid[x, y].Fraction)
			{
				_grid[x, y].PowerNext += power;
				if (_grid[x, y].PowerNext > 1)
				{
					_grid[x, y].PowerNext = 1;
					_grid[x, y].IsNeutralDraining = false;
				}
			}
			else
			{
				_grid[x, y].PowerNext -= power;
				if (_grid[x, y].PowerNext < 0)
				{
					_grid[x, y].Fraction = f;
					_grid[x, y].PowerNext = -_grid[x, y].PowerNext;
					_grid[x, y].IsNeutralDraining = false;
				}
			}
		}

		private void LoosePower(int x, int y, float power)
		{
			if (_grid[x, y].Fraction == null || _grid[x, y].Fraction.IsNeutral) return;

			_grid[x, y].PowerNext -= power;
			if (_grid[x, y].PowerNext < 0)
			{
				_grid[x, y].Fraction = null;
				_grid[x, y].PowerNext = 0;
				_grid[x, y].IsNeutralDraining = false;
			}
		}

		private Color GetGridColor(int x, int y)
		{
			if (x < 0) return GetGridColor(0, y);
			if (y < 0) return GetGridColor(x, 0);
			if (x >= TILE_COUNT_X) return GetGridColor(TILE_COUNT_X-1, y);
			if (y >= TILE_COUNT_Y) return GetGridColor(x, TILE_COUNT_Y-1);

			if (_grid[x, y].Fraction == null) return FlatColors.Background;

			return ColorMath.Blend(FlatColors.Background, _grid[x, y].Fraction.BackgroundColor, _grid[x, y].PowerCurr);
		}

		public void RegisterSpawn(Cannon cannon, FCircle circle)
		{
			for (int ox = (int)((circle.X-circle.Radius) / GDConstants.TILE_WIDTH); ox <= (int)((circle.X + circle.Radius) / GDConstants.TILE_WIDTH); ox++)
			{
				for (int oy = (int)((circle.Y - circle.Radius) / GDConstants.TILE_WIDTH); oy <= (int)((circle.Y + circle.Radius) / GDConstants.TILE_WIDTH); oy++)
				{
					int x = ox + MAX_EXTENSION; // real coords -> array coords
					int y = oy + MAX_EXTENSION;

					if (x < 0) continue;
					if (y < 0) continue;
					if (x >= TILE_COUNT_X) continue;
					if (y >= TILE_COUNT_Y) continue;
					
					if (!_rects[x, y].Intersects(circle)) continue;

					_grid[x, y].SpawnSource = cannon;
					_grid[x, y].PowerCurr = 1f;
				}
			}
		}

		public void RegisterBlockedLine(Vector2 start, Vector2 end)
		{

		}

		public void RegisterBlockedCircle(Vector2 pos, float r)
		{

		}

		public void RegisterBlockedBlock(FRectangle block)
		{

		}

		#endregion
	}
}
