#if DEBUG
//#define DEBUG_GAME_GRID_BACKGROUND
#endif

using System.Collections.Generic;
using System.Linq;
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
	}

	class GameGridBackground
	{
		private const int TILE_WIDTH = GameScreen.TILE_WIDTH;

		private const int TILE_COUNT_X = GameScreen.GRID_WIDTH;
		private const int TILE_COUNT_Y = GameScreen.GRID_HEIGHT;

		private const float GRID_ADAPTION_SPEED = 0.2f;
		private const float GRID_ADAPTION_SPEED_DIRECT = 2f;

		protected readonly GraphicsDevice Graphics;
		protected readonly TolerantBoxingViewportAdapter Adapter;

		public readonly List<BackgroundParticle> Particles = new List<BackgroundParticle>();
		private readonly List<BackgroundParticle>[,] particlesHorizontal = new List<BackgroundParticle>[TILE_COUNT_X, TILE_COUNT_Y + 1];
		private readonly List<BackgroundParticle>[,] particlesVertical = new List<BackgroundParticle>[TILE_COUNT_X + 1, TILE_COUNT_Y];
		private readonly List<Cannon>[,] blockedGridPoints = new List<Cannon>[TILE_COUNT_X + 1, TILE_COUNT_Y + 1];
		private readonly GridCellMembership[,] gridColor = new GridCellMembership[TILE_COUNT_X + 2, TILE_COUNT_Y + 2]; 

		public GameGridBackground(GraphicsDevice graphicsDevice, TolerantBoxingViewportAdapter adapter)
		{
			Graphics = graphicsDevice;
			Adapter = adapter;

			for (int x = 0; x < TILE_COUNT_X; x++)
				for (int y = 0; y <= TILE_COUNT_Y; y++)
					particlesHorizontal[x, y] = new List<BackgroundParticle>();

			for (int x = 0; x <= TILE_COUNT_X; x++)
				for (int y = 0; y < TILE_COUNT_Y; y++)
					particlesVertical[x, y] = new List<BackgroundParticle>();

			for (int x = 0; x <= TILE_COUNT_X; x++)
				for (int y = 0; y <= TILE_COUNT_Y; y++)
					blockedGridPoints[x, y] = new List<Cannon>();

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

#if DEBUG_GAME_GRID_BACKGROUND
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

#if DEBUG_GAME_GRID_BACKGROUND

			foreach (var particle in Particles)
			{
				sbatch.Draw(
					Textures.TexPixel.Texture, 
					new Vector2(particle.X, particle.Y),
					Textures.TexPixel.Bounds,
					particle.Fraction.Color * 0.6f * particle.PowerPercentage,
					0,
					new Vector2(0.5f, 0.5f),
					8, 
					SpriteEffects.None, 1);
			}

			sbatch.DrawRectangle(new Rectangle(0, 0, TILE_COUNT_X * TILE_WIDTH, TILE_COUNT_Y * TILE_WIDTH), Color.Magenta);
#endif
		}

		#endregion

		#region Update

		public void Update(GameTime gameTime, InputState state)
		{
			foreach (var particle in Particles.ToList())
			{
				if (! particle.Alive) continue;
				
				switch (particle.Direction)
				{
					case Direction4.North:
						#region North
						{
							var before = particle.Y;
							particle.Y -= BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.South && p.Y >= after && p.Y <= before);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.North && p.Y >= after && p.Y <= before);

							int x = (int)particle.X / TILE_WIDTH;
							int y = (int)(before / TILE_WIDTH);

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x + 0, y + 0);
								ColorGridCell(gameTime, particle, x - 1, y + 0);
							}

							break;
						}
					#endregion
					case Direction4.East:
						#region East
						{
							var before = particle.X;
							particle.X += BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.West && p.X >= before && p.X <= after);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.East && p.X >= before && p.X <= after);

							int x = (int)(after / TILE_WIDTH);
							int y = (int)particle.Y / TILE_WIDTH;

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x, y - 1);
								ColorGridCell(gameTime, particle, x, y + 0);
							}

							break;
						}
					#endregion
					case Direction4.South:
						#region South
						{
							var before = particle.Y;
							particle.Y += BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.North && p.Y >= before && p.Y <= after);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.South && p.Y >= before && p.Y <= after);

							int x = (int)particle.X / TILE_WIDTH;
							int y = (int)(after / TILE_WIDTH);

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}
							else
							{
								ColorGridCell(gameTime, particle, x - 1, y);
								ColorGridCell(gameTime, particle, x + 0, y);
							}

							break;
						}
						#endregion
					case Direction4.West:
						#region West
						{
							var before = particle.X;
							particle.X -= BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.East && p.X >= after && p.X <= before);
							var merge = particle.TravelSection.FirstOrDefault(p => p != particle && p.Direction == Direction4.West && p.X >= after && p.X <= before);

							int x = (int)(before / TILE_WIDTH);
							int y = (int)particle.Y / TILE_WIDTH;

							if (merge != null)
							{
								MergeParticles(merge, particle);
							}
							else if (coll != null)
							{
								RemoveParticles(coll, particle);
							}
							else if (FloatMath.Floor(before/TILE_WIDTH) != FloatMath.Floor(after/TILE_WIDTH))
							{
								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x*TILE_WIDTH, y*TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x*TILE_WIDTH + 0.01f, y*TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x*TILE_WIDTH, y*TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x*TILE_WIDTH - 0.01f, y*TILE_WIDTH, Direction4.West), x, y);
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
					if (blockedGridPoints[x, y].Any(p => FloatMath.FloatEquals(p.CannonHealth.TargetValue, 1f)))
					{
						var f = blockedGridPoints[x, y].First().Fraction;
						if (f.IsNeutral) f = null;

						ColorGridCellDirect(f, x - 0, y - 0, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 1, y - 0, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 0, y - 1, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT);
						ColorGridCellDirect(f, x - 1, y - 1, gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED_DIRECT);
					}
				}
			}
		}

		private void ColorGridCell(GameTime gameTime, BackgroundParticle p, int x, int y)
		{
			if (x < -1) return;
			if (y < -1) return;
			if (x > TILE_COUNT_X) return;
			if (y > TILE_COUNT_Y) return;

			var power = gameTime.GetElapsedSeconds() * GRID_ADAPTION_SPEED * p.PowerPercentage;

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
				case Direction4.North:
					if (y <= 0) return;
					p.TravelSection = particlesVertical[x, y - 1];
					p.TravelSection.Add(p);
					break;
				case Direction4.East:
					if (x >= TILE_COUNT_X) return;
					p.TravelSection = particlesHorizontal[x, y];
					p.TravelSection.Add(p);
					break;
				case Direction4.South:
					if (y >= TILE_COUNT_Y) return;
					p.TravelSection = particlesVertical[x, y];
					p.TravelSection.Add(p);
					break;
				case Direction4.West:
					if (x <= 0) return;
					p.TravelSection = particlesHorizontal[x - 1, y];
					p.TravelSection.Add(p);
					break;
			}

			Particles.Add(p);
		}

		public void SpawnParticles(Fraction fraction, int x, int y)
		{
			AddParticle(new BackgroundParticle(x, y, fraction, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y, true);
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
