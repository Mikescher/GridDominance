using System.Runtime.InteropServices;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace GridDominance.Shared.Screens.GameScreen.Background
{
	class GridCellMembership
	{
		public Fraction Fraction = null;
		public float Strength = 0;

		public Fraction NextFraction = null;
		public float NextStrength = 0;

		public void Swap()
		{
			Fraction = NextFraction;
			Strength = NextStrength;
		}
	}

	class GameGridBackground
	{
		private const int TILE_WIDTH = GameScreen.TILE_WIDTH;

		private const int TILE_COUNT_X = 16;
		private const int TILE_COUNT_Y = 10;

		private const float GRID_ADAPTION_SPEED = 0.3f;
		private const float GRID_ADAPTION_SPEED_DIRECT = 0.8f;

		protected readonly GraphicsDevice Graphics;
		protected readonly TolerantBoxingViewportAdapter Adapter;
		
		private readonly Cannon[,] sourceGridCells = new Cannon[TILE_COUNT_X + 2, TILE_COUNT_Y + 2];
		private readonly GridCellMembership[,] gridColor = new GridCellMembership[TILE_COUNT_X + 2, TILE_COUNT_Y + 2]; 


		public GameGridBackground(GraphicsDevice graphicsDevice, TolerantBoxingViewportAdapter adapter)
		{
			Graphics = graphicsDevice;
			Adapter = adapter;
			
			for (int x = 0; x < TILE_COUNT_X+2; x++)
				for (int y = 0; y < TILE_COUNT_Y+2; y++)
					gridColor[x, y] = new GridCellMembership();
		}

		#region Draw

		public void Draw(SpriteBatch sbatch)
		{
			int extensionX = FloatMath.Ceiling((Graphics.Viewport.Width  - Adapter.RealWidth)  / (TILE_WIDTH * 2f * Adapter.GetScale()));
			int extensionY = FloatMath.Ceiling((Graphics.Viewport.Height - Adapter.RealHeight) / (TILE_WIDTH * 2f * Adapter.GetScale()));

			for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
			{
				for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
				{
					var color = GetGridColor(x, y);

					sbatch.Draw(Textures.TexPixel, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), color);
					sbatch.Draw(Textures.TexTileBorder, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);

#if DEBUG
					if (x < -1) continue;
					if (y < -1) continue;
					if (x > TILE_COUNT_X) continue;
					if (y > TILE_COUNT_Y) continue;

					var tx = x * TILE_WIDTH + 8;
					var ty = y * TILE_WIDTH + 8;

					sbatch.DrawString(
						Textures.DebugFontSmall, 
						string.Format("{0,2}: {1:000}", gridColor[x + 1, y + 1].Fraction?.ToString() ?? "##", gridColor[x + 1, y + 1].Strength * 100), 
						new Vector2(tx, ty),
						gridColor[x + 1, y + 1].Fraction?.Color ?? Color.Black);
#endif
				}
			}

#if DEBUG
			sbatch.DrawRectangle(new Rectangle(0, 0, TILE_COUNT_X * TILE_WIDTH, TILE_COUNT_Y * TILE_WIDTH), Color.Magenta);
#endif
		}

		#endregion

		#region Update

		public void Update(GameTime gameTime, InputState state)
		{
			// Update sources
			for (int x = -1; x < TILE_COUNT_X + 1; x++)
			{
				for (int y = -1; y < TILE_COUNT_Y + 1; y++)
				{
					if (sourceGridCells[x + 1, y + 1] != null)
					{
						var frac = sourceGridCells[x + 1, y + 1].Fraction;

						if (FloatMath.IsOne(sourceGridCells[x+1, y+1].CannonHealth.TargetValue))
						{
							ColorGridCellDirect(frac, x, y, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT);
						}
						else if (frac.IsNeutral)
						{
							ColorGridCellDirect(frac, x, y, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT * 0.5f);
						}
					}
					else
					{
						ColorGridCell(gridColor[x + 1, y + 1], x, y, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED);
					}
				}
			}
			
			// swap
			for (int x = -1; x < TILE_COUNT_X + 1; x++)
			{
				for (int y = -1; y < TILE_COUNT_Y; y++)
				{
					gridColor[x + 1, y + 1].Swap();
				}
			}
		}

		private void ColorGridCell(GridCellMembership cell, int x, int y, float power)
		{
			if (x >= 0)
				FlowCellColor(cell, power, gridColor[x + 0, y + 1]);

			if (y >= 0)
				FlowCellColor(cell, power, gridColor[x + 1, y + 0]);

			if (x < TILE_COUNT_X)
				FlowCellColor(cell, power, gridColor[x + 2, y + 1]);

			if (y < TILE_COUNT_Y)
				FlowCellColor(cell, power, gridColor[x + 1, y + 2]);
		}

		private static void FlowCellColor(GridCellMembership cell, float power, GridCellMembership other)
		{
			if (other.Fraction == null) return;

			if (other.Fraction == cell.NextFraction)
			{
				var flow = FloatMath.Limit(other.Strength - cell.NextStrength, 0f, power);
				cell.NextStrength = FloatMath.LimitedInc(cell.NextStrength, flow, 1f);
			}
			else if (other.Fraction != cell.NextFraction)
			{
				var flow = FloatMath.Limit(FloatMath.Abs(other.Strength - cell.NextStrength), 0f, power);

				bool convert;
				cell.NextStrength = FloatMath.LimitedDec(cell.NextStrength, flow, 0f, out convert);
				if (convert)
				{
					cell.NextFraction = other.Fraction;
					cell.NextStrength = FloatMath.Abs(cell.NextStrength - flow);
				}
			}
		}
		
		private void ColorGridCellDirect(Fraction f, int x, int y, float power)
		{
			var member = gridColor[x + 1, y + 1];

			if (member.Fraction == null)
			{
				member.NextFraction = f;
				member.NextStrength = 0f;
			}
			else if(member.Fraction == f)
			{
				member.NextStrength = FloatMath.LimitedInc(member.Strength, power, 1f);
			}
			else // if(member.Fraction != f)
			{
				bool convert;
				member.NextStrength = FloatMath.LimitedDec(member.Strength, power, 0f, out convert);

				if (convert)
				{
					member.NextFraction = f;
					member.NextStrength = 0f;
				}
			}
		}

		private Color GetGridColor(int x, int y)
		{
			if (x < -1) return GetGridColor(-1, y);
			if (y < -1) return GetGridColor(x, -1);
			if (x > TILE_COUNT_X) return GetGridColor(TILE_COUNT_X, y);
			if (y > TILE_COUNT_Y) return GetGridColor(x, TILE_COUNT_Y);

			if (gridColor[x + 1, y + 1].Fraction == null) return FlatColors.Background;

			return ColorMath.Blend(FlatColors.Background, gridColor[x + 1, y + 1].Fraction.BackgroundColor, gridColor[x + 1, y + 1].Strength);
		}
		
		public void RegisterBlockedSpawn(Cannon cannon, int x, int y)
		{
			sourceGridCells[x + 0, y + 0] = cannon;
			sourceGridCells[x + 1, y + 0] = cannon;
			sourceGridCells[x + 0, y + 1] = cannon;
			sourceGridCells[x + 1, y + 1] = cannon;
		}

		public void DeregisterBlockedSpawn(Cannon cannon, int x, int y)
		{
			sourceGridCells[x + 0, y + 0] = null;
			sourceGridCells[x + 1, y + 0] = null;
			sourceGridCells[x + 0, y + 1] = null;
			sourceGridCells[x + 1, y + 1] = null;
		}

		#endregion

#if DEBUG
		public string GetDebugSummary()
		{
			return "?";
		}
#endif

	}
}
