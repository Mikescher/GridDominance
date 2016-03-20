using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
	class GameGridBackground
	{
		private const int TILE_WIDTH = GameScreen.TILE_WIDTH;

		private const int TILE_COUNT_X = 16;
		private const int TILE_COUNT_Y = 10;

		protected readonly GraphicsDevice Graphics;
		protected readonly TolerantBoxingViewportAdapter Adapter;

		private readonly List<BackgroundParticle> particles = new List<BackgroundParticle>(); 
		private readonly List<BackgroundParticle>[,] particlesHorizontal = new List<BackgroundParticle>[TILE_COUNT_X, TILE_COUNT_Y + 1];
		private readonly List<BackgroundParticle>[,] particlesVertical = new List<BackgroundParticle>[TILE_COUNT_X + 1, TILE_COUNT_Y];
		private readonly List<object>[,] blockedGridPoints = new List<object>[TILE_COUNT_X + 1, TILE_COUNT_Y + 1];

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
					blockedGridPoints[x, y] = new List<object>();
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
					sbatch.Draw(Textures.TexPixel, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), FlatColors.Background);
					sbatch.Draw(Textures.TexTileBorder, new Rectangle(x * TILE_WIDTH, y * TILE_WIDTH, TILE_WIDTH, TILE_WIDTH), Color.White);
				}
			}

#if DEBUG
			for (int x = 0; x < TILE_COUNT_X; x++)
			{
				for (int y = 0; y <= TILE_COUNT_Y; y++)
				{
					if (particlesHorizontal[x, y].Any())
					{
						sbatch.DrawLine(new Vector2(x * TILE_WIDTH, y * TILE_WIDTH), new Vector2((x + 1) * TILE_WIDTH, y * TILE_WIDTH), Color.Black, 4);
					}
				}
			}
			for (int x = 0; x <= TILE_COUNT_X; x++)
			{
				for (int y = 0; y < TILE_COUNT_Y; y++)
				{
					if (particlesVertical[x, y].Any())
					{
						sbatch.DrawLine(new Vector2(x * TILE_WIDTH, y * TILE_WIDTH), new Vector2(x * TILE_WIDTH, (y + 1) * TILE_WIDTH), Color.Black, 4);
					}
				}
			}
#endif

			foreach (var particle in particles)
			{
				sbatch.Draw(
					Textures.TexPixel.Texture, 
					new Vector2(particle.X, particle.Y),
					Textures.TexPixel.Bounds,
					particle.Fraction.Color * BackgroundParticle.PARTICLE_ALPHA,
					particle.Rotation,
					new Vector2(0.5f, 0.5f),
					BackgroundParticle.PARTICLE_WIDTH, 
					SpriteEffects.None, 1);
			}
		}

		#endregion

		#region Update

		public void Update(GameTime gameTime, InputState state)
		{
			foreach (var particle in particles.ToList())
			{
				if (! particle.Alive) continue;

				particle.Rotation = FloatMath.IncModulo(particle.Rotation, particle.RotationSpeed * gameTime.GetElapsedSeconds(), FloatMath.TAU);

				switch (particle.Direction)
				{
					case Direction4.North:
						{
							var before = particle.Y;
							particle.Y -= BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Y >= after && p.Y <= before);

							if (coll != null)
							{
								RemoveParticle(particle);
								RemoveParticle(coll);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								int x = (int)particle.X / TILE_WIDTH;
								int y = (int)(before / TILE_WIDTH);

								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}

							break;
						}
					case Direction4.East:
						{
							var before = particle.X;
							particle.X += BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.X >= before && p.X <= after);

							if (coll != null)
							{
								RemoveParticle(particle);
								RemoveParticle(coll);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								int x = (int)(after / TILE_WIDTH);
								int y = (int)particle.Y / TILE_WIDTH;

								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}

							break;
						}
					case Direction4.South:
						{
							var before = particle.Y;
							particle.Y += BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.Y;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.Y >= before && p.Y <= after);

							if (coll != null)
							{
								RemoveParticle(particle);
								RemoveParticle(coll);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								int x = (int)particle.X / TILE_WIDTH;
								int y = (int)(after / TILE_WIDTH);

								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}

							break;
						}
					case Direction4.West:
						{
							var before = particle.X;
							particle.X -= BackgroundParticle.PARTICLE_SPEED * gameTime.GetElapsedSeconds();
							var after = particle.X;

							var coll = particle.TravelSection.FirstOrDefault(p => p != particle && p.X >= after && p.X <= before);

							if (coll != null)
							{
								RemoveParticle(particle);
								RemoveParticle(coll);
							}
							else if (FloatMath.Floor(before / TILE_WIDTH) != FloatMath.Floor(after / TILE_WIDTH))
							{
								int x = (int)(before / TILE_WIDTH);
								int y = (int)particle.Y / TILE_WIDTH;

								RemoveParticle(particle);

								if (particle.OriginY >= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y);
								if (particle.OriginX <= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y);
								if (particle.OriginY <= y) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y);
								if (particle.OriginX >= x) AddParticle(new BackgroundParticle(particle, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y);
							}

							break;
						}
				}
			}
		}

		private void RemoveParticle(BackgroundParticle p)
		{
			p.Alive = false;

			p.TravelSection.Remove(p);
			particles.Remove(p);
		}

		private void AddParticle(BackgroundParticle p, int x, int y, bool ignoreBlockedSpawns = false)
		{
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

			particles.Add(p);
		}

		public void SpawnParticles(Fraction fraction, int x, int y)
		{
			AddParticle(new BackgroundParticle(x, y, fraction, fraction.ParticleLifetime, x * TILE_WIDTH, y * TILE_WIDTH - 0.01f, Direction4.North), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, fraction.ParticleLifetime, x * TILE_WIDTH + 0.01f, y * TILE_WIDTH, Direction4.East), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, fraction.ParticleLifetime, x * TILE_WIDTH, y * TILE_WIDTH + 0.01f, Direction4.South), x, y, true);
			AddParticle(new BackgroundParticle(x, y, fraction, fraction.ParticleLifetime, x * TILE_WIDTH - 0.01f, y * TILE_WIDTH, Direction4.West), x, y, true);
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
