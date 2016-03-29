using FarseerPhysics;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper;

namespace GridDominance.Shared.Screens.ScreenGame.Entities
{
	class BulletSplitter : GDEntity
	{
		private const float SPLITTER_SPEED = 30f;
		private const float SPLITTER_LIFETIME_MIN = 0.15f;
		private const float SPLITTER_LIFETIME_MAX = 0.35f;

		private static readonly Vector2[] VELOCITIES =
		{
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.NN * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.NE * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.EE * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.SE * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.SS * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.SW * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.WW * 45f)),
			new Vector2(0, -SPLITTER_SPEED).Rotate(FloatMath.ToRadians(22.5f + (int)Direction8.NW * 45f)),
		};

		public readonly Fraction Fraction;
		public float ShapeAlpha;
		public Vector2 ShapePosition;
		public float ShapeRotation = 0f;

		private readonly float scale;
		
		private readonly float maxLifetime;

		private readonly float rotationSpeed = 0f;
		private readonly Vector2 velocity;
		private readonly Vector2 velocityAngular;
		private readonly Direction8 direction;

		public override Vector2 Position => ShapePosition;

		public BulletSplitter(GDGameScreen scrn, Bullet b, Direction8 d) : base(scrn)
		{
			scale = b.Scale;
			ShapePosition = b.BulletPosition;
			ShapeAlpha = 1f;
			Fraction = b.Fraction;
			direction = d;
			velocity = VELOCITIES[(int)d] * FloatMath.GetRangedRandom(0.5f, 2f) + ConvertUnits.ToDisplayUnits(b.PhysicsBody.LinearVelocity)/10f;
			maxLifetime = FloatMath.GetRangedRandom(SPLITTER_LIFETIME_MIN, SPLITTER_LIFETIME_MAX);
			ShapeRotation = FloatMath.ToRadians((int) direction * 45f);
			rotationSpeed = FloatMath.GetRangedRandom(-FloatMath.TAU, FloatMath.TAU);
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}
		
		protected override void OnUpdate(GameTime gameTime, InputState istate)
		{
			ShapePosition += velocity * gameTime.GetElapsedSeconds();

			ShapeRotation = FloatMath.IncModulo(ShapeRotation, rotationSpeed * gameTime.GetElapsedSeconds(), FloatMath.TAU);

			if (Lifetime < maxLifetime) ShapeAlpha = 1 - Lifetime / maxLifetime;
			if (Lifetime > maxLifetime) Alive = false;
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(
				Textures.TexBulletSplitter.Texture,
				ShapePosition,
				Textures.TexBulletSplitter.Bounds,
				Fraction.Color * ShapeAlpha,
				ShapeRotation,
				new Vector2(Textures.TexBullet.Width / 2f, Textures.TexBullet.Height / 2f),
				scale * Textures.DEFAULT_TEXTURE_SCALE,
				SpriteEffects.None,
				0);
		}
	}
}
