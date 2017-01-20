using System.Collections.Generic;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Screens.Background;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Screens.ScreenGame.Background
{
	class GridCellMembership
	{
		public Fraction Fraction = null;
		public float Strength = 0;
	}

	class GDGridBackground : GameBackground
	{
		private const int TILE_COUNT_X = GDConstants.GRID_WIDTH;
		private const int TILE_COUNT_Y = GDConstants.GRID_HEIGHT;

		private const float GRID_ADAPTION_SPEED = 0.2f;
		private const float GRID_ADAPTION_SPEED_DIRECT = 2f;

		public readonly List<BackgroundParticle> Particles = new List<BackgroundParticle>();
		private readonly List<BackgroundParticle>[,] particlesHorizontal = new List<BackgroundParticle>[TILE_COUNT_X, TILE_COUNT_Y + 1];
		private readonly List<BackgroundParticle>[,] particlesVertical = new List<BackgroundParticle>[TILE_COUNT_X + 1, TILE_COUNT_Y];
		private readonly List<Cannon>[,] blockedGridPoints = new List<Cannon>[TILE_COUNT_X + 1, TILE_COUNT_Y + 1];
		private readonly GridCellMembership[,] gridColor = new GridCellMembership[TILE_COUNT_X + 2, TILE_COUNT_Y + 2]; 

		public GDGridBackground(GameScreen scrn) : base(scrn)
		{
			Initialize();
		}

		private void Initialize()
		{
			for (int x = 0; x < TILE_COUNT_X; x++)
				for (int y = 0; y <= TILE_COUNT_Y; y++)
					particlesHorizontal[x, y] = new List<BackgroundParticle>();

			for (int x = 0; x <= TILE_COUNT_X; x++)
				for (int y = 0; y < TILE_COUNT_Y; y++)
					particlesVertical[x, y] = new List<BackgroundParticle>();

			for (int x = 0; x <= TILE_COUNT_X; x++)
				for (int y = 0; y <= TILE_COUNT_Y; y++)
					blockedGridPoints[x, y] = new List<Cannon>();

			for (int x = 0; x < TILE_COUNT_X + 2; x++)
				for (int y = 0; y < TILE_COUNT_Y + 2; y++)
					gridColor[x, y] = new GridCellMembership();
		}

		#region Draw

		public override void Draw(IBatchRenderer sbatch)
		{
			int extensionX = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetX / GDConstants.TILE_WIDTH);
			int extensionY = FloatMath.Ceiling(VAdapter.VirtualGuaranteedBoundingsOffsetY / GDConstants.TILE_WIDTH);

			for (int x = -extensionX; x < TILE_COUNT_X + extensionX; x++)
			{
				for (int y = -extensionY; y < TILE_COUNT_Y + extensionY; y++)
				{
					var color = GetGridColor(x, y);

					sbatch.DrawStretched(Textures.TexPixel, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), color);
					sbatch.DrawStretched(Textures.TexTileBorder, new FRectangle(x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH), Color.White);

#if DEBUG
					if (DebugSettings.Get("DebugBackground"))
					{
						if (x < -1) continue;
						if (y < -1) continue;
						if (x > TILE_COUNT_X) continue;
						if (y > TILE_COUNT_Y) continue;

						var tx = x * GDConstants.TILE_WIDTH + 8;
						var ty = y * GDConstants.TILE_WIDTH + 8;

						sbatch.DrawString(
							Textures.DebugFontSmall,
							string.Format("{0,2}: {1:000}", gridColor[x + 1, y + 1].Fraction?.ToString() ?? "##", gridColor[x + 1, y + 1].Strength * 100),
							new Vector2(tx, ty),
							gridColor[x + 1, y + 1].Fraction?.Color ?? Color.Black);
					}
#endif
				}
			}

#if DEBUG
			if (DebugSettings.Get("DebugBackground"))
			{
				foreach (var particle in Particles)
				{
					sbatch.DrawCentered(Textures.TexPixel, new Vector2(particle.X, particle.Y), 8, 8, particle.Fraction.Color * 0.6f * particle.PowerPercentage, 0, 1);
				}

				sbatch.DrawRectangle(new FRectangle(0, 0, TILE_COUNT_X * GDConstants.TILE_WIDTH, TILE_COUNT_Y * GDConstants.TILE_WIDTH), Color.Magenta);
			}
#endif
		}

		#endregion

		#region Update

		public override void Update(SAMTime gameTime, InputState state)
		{
			foreach (var particle in Particles.ToList())
			{
				if (! particle.Alive) continue;
				
				switch (particle.Direction)
				{
					case FlatAlign4.NORTH:
						#region North
						{
							var before = particle.Y;
							particle.Y -= BackgroundParticle.PARTICLE_SPEED * gameTime.ElapsedSeconds;
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.SOUTH && p.Y >= after && p.Y <= before);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.NORTH && p.Y >= after && p.Y <= before);

							int x = (int)particle.X / GDConstants.TILE_WIDTH;
							int y = (int)(before / GDConstants.TILE_WIDTH);

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / GDConstants.TILE_WIDTH) != FloatMath.Floor(after / GDConstants.TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH - 0.01f, FlatAlign4.NORTH), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH + 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.EAST), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 0.01f, FlatAlign4.SOUTH), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH - 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.WEST), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x + 0, y + 0);
								ColorGridCell(gameTime, particle, x - 1, y + 0);
							}

							break;
						}
					#endregion
					case FlatAlign4.EAST:
						#region East
						{
							var before = particle.X;
							particle.X += BackgroundParticle.PARTICLE_SPEED * gameTime.ElapsedSeconds;
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.WEST && p.X >= before && p.X <= after);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.EAST && p.X >= before && p.X <= after);

							int x = (int)(after / GDConstants.TILE_WIDTH);
							int y = (int)particle.Y / GDConstants.TILE_WIDTH;

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / GDConstants.TILE_WIDTH) != FloatMath.Floor(after / GDConstants.TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH - 0.01f, FlatAlign4.NORTH), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH + 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.EAST), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 0.01f, FlatAlign4.SOUTH), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH - 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.WEST), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x, y - 1);
								ColorGridCell(gameTime, particle, x, y + 0);
							}

							break;
						}
					#endregion
					case FlatAlign4.SOUTH:
						#region South
						{
							var before = particle.Y;
							particle.Y += BackgroundParticle.PARTICLE_SPEED * gameTime.ElapsedSeconds;
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.NORTH && p.Y >= before && p.Y <= after);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.SOUTH && p.Y >= before && p.Y <= after);

							int x = (int)particle.X / GDConstants.TILE_WIDTH;
							int y = (int)(after / GDConstants.TILE_WIDTH);

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / GDConstants.TILE_WIDTH) != FloatMath.Floor(after / GDConstants.TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH - 0.01f, FlatAlign4.NORTH), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH + 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.EAST), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 0.01f, FlatAlign4.SOUTH), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * GDConstants.TILE_WIDTH - 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.WEST), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x - 1, y);
								ColorGridCell(gameTime, particle, x + 0, y);
							}

							break;
						}
						#endregion
					case FlatAlign4.WEST:
						#region West
						{
							var before = particle.X;
							particle.X -= BackgroundParticle.PARTICLE_SPEED * gameTime.ElapsedSeconds;
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.EAST && p.X >= after && p.X <= before);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == FlatAlign4.WEST && p.X >= after && p.X <= before);

							int x = (int)(before / GDConstants.TILE_WIDTH);
							int y = (int)particle.Y / GDConstants.TILE_WIDTH;

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before/GDConstants.TILE_WIDTH) != FloatMath.Floor(after/GDConstants.TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x*GDConstants.TILE_WIDTH, y*GDConstants.TILE_WIDTH - 0.01f, FlatAlign4.NORTH), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x*GDConstants.TILE_WIDTH + 0.01f, y*GDConstants.TILE_WIDTH, FlatAlign4.EAST), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x*GDConstants.TILE_WIDTH, y*GDConstants.TILE_WIDTH + 0.01f, FlatAlign4.SOUTH), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x*GDConstants.TILE_WIDTH - 0.01f, y*GDConstants.TILE_WIDTH, FlatAlign4.WEST), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x + 0, y - 1);
								ColorGridCell(gameTime, particle, x + 0, y + 0);
							}

							break;
						}
						#endregion
				}
			}

			for (int x = 0; x <= TILE_COUNT_X; x++)
			{
				for (int y = 0; y <= TILE_COUNT_Y; y++)
				{
					if (blockedGridPoints[x, y].Any(p => FloatMath.IsOne(p.CannonHealth.TargetValue)))
					{
						var f = blockedGridPoints[x, y].First().Fraction;
						if (f.IsNeutral) f = null;

						ColorGridCellDirect(f, x - 0, y - 0, gameTime.ElapsedSeconds * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 1, y - 0, gameTime.ElapsedSeconds * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 0, y - 1, gameTime.ElapsedSeconds * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 1, y - 1, gameTime.ElapsedSeconds * GRID_ADAPTION_SPEED_DIRECT);
					}
				}
			}
		}

		private void ColorGridCell(SAMTime gameTime, BackgroundParticle p, int x, int y)
		{
			if (x < -1) return;
			if (y < -1) return;
			if (x > TILE_COUNT_X) return;
			if (y > TILE_COUNT_Y) return;

			var power = gameTime.ElapsedSeconds * GRID_ADAPTION_SPEED * p.PowerPercentage;

			ColorGridCellDirect(p.Fraction, x, y, power);
		}

		private void ColorGridCellDirect(Fraction f, int x, int y, float power)
		{
			if (x < 0 || y < 0 || x >= TILE_COUNT_X || y >= TILE_COUNT_Y) power *= 4;

			if (f.IsNeutral)
			{
				bool convert;
				gridColor[x + 1, y + 1].Strength = FloatMath.LimitedDec(gridColor[x + 1, y + 1].Strength, power, 0f, out convert);

				if (convert)
				{
					gridColor[x + 1, y + 1].Fraction = null;
					gridColor[x + 1, y + 1].Strength = 0f;
				}
			}
			else if (gridColor[x + 1, y + 1].Fraction == f)
			{
				gridColor[x + 1, y + 1].Strength = FloatMath.LimitedInc(gridColor[x + 1, y + 1].Strength, power, 1f);
			}
			else if (gridColor[x + 1, y + 1].Fraction == null)
			{
				gridColor[x + 1, y + 1].Fraction = f;
				gridColor[x + 1, y + 1].Strength = 0f;
			}
			else
			{
				bool convert;
				gridColor[x + 1, y + 1].Strength = FloatMath.LimitedDec(gridColor[x + 1, y + 1].Strength, power, 0f, out convert);

				if (convert)
				{
					gridColor[x + 1, y + 1].Fraction = f;
					gridColor[x + 1, y + 1].Strength = 0f;
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

		private void RemoveParticle(BackgroundParticle p)
		{
			p.Alive = false;

			p.TravelSection.Remove(p);
			Particles.Remove(p);
		}

		private void RemoveParticles(BackgroundParticle a, BackgroundParticle b)
		{
			if (a.Fraction == b.Fraction)
			{
				RemoveParticle(a);
				RemoveParticle(b);
			}
			else if (a.RemainingPower < b.RemainingPower)
			{
				RemoveParticle(a);
				b.RemainingPower -= a.RemainingPower;
			}
			else if (a.RemainingPower > b.RemainingPower)
			{
				RemoveParticle(b);
				a.RemainingPower -= b.RemainingPower;
			}
			else
			{
				RemoveParticle(a);
				RemoveParticle(b);
			}
		}

		private void MergeParticles(BackgroundParticle a, BackgroundParticle b)
		{
			if (a.RemainingPower > b.RemainingPower)
				RemoveParticle(a);
			else
				RemoveParticle(b);
		}

		private void AddParticle(BackgroundParticle p, int x, int y, bool ignoreBlockedSpawns = false)
		{
			if (p.RemainingPower <= 0) return;

			if (blockedGridPoints[x, y].Any() && !ignoreBlockedSpawns) return;

			switch (p.Direction)
			{
				case FlatAlign4.NORTH:
					if (y <= 0) return;
					p.TravelSection = particlesVertical[x, y - 1];
					p.TravelSection.Add(p);
					break;
				case FlatAlign4.EAST:
					if (x >= TILE_COUNT_X) return;
					p.TravelSection = particlesHorizontal[x, y];
					p.TravelSection.Add(p);
					break;
				case FlatAlign4.SOUTH:
					if (y >= TILE_COUNT_Y) return;
					p.TravelSection = particlesVertical[x, y];
					p.TravelSection.Add(p);
					break;
				case FlatAlign4.WEST:
					if (x <= 0) return;
					p.TravelSection = particlesHorizontal[x - 1, y];
					p.TravelSection.Add(p);
					break;
			}

			Particles.Add(p);
		}

		public void SpawnParticles(Fraction fraction, int x, int y)
		{
			AddParticle(new BackgroundParticle(x, y, fraction, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH - 0.01f, FlatAlign4.NORTH), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * GDConstants.TILE_WIDTH + 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.EAST), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * GDConstants.TILE_WIDTH, y * GDConstants.TILE_WIDTH + 0.01f, FlatAlign4.SOUTH), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * GDConstants.TILE_WIDTH - 0.01f, y * GDConstants.TILE_WIDTH, FlatAlign4.WEST), x, y, true);
		}

		public void RegisterBlockedSpawn(Cannon cannon, int x, int y)
		{
			blockedGridPoints[x, y].Add(cannon);
		}

		public void DeregisterBlockedSpawn(Cannon cannon, int x, int y)
		{
			blockedGridPoints[x, y].Remove(cannon);
		}

		#endregion
	}
}
