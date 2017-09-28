using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Persistance;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public class ParticleEmitterConfig
	{
		public class ParticleEmitterConfigBuilder
		{
			public int TextureIndex;

			public float SpawnRate = 0f; // particles per second

			public float ParticleLifetimeMin = 0;
			public float ParticleLifetimeMax = 0;
			public float? ParticleLifetime
			{
				get { return FloatMath.FloatEquals(ParticleLifetimeMin, ParticleLifetimeMax) ? ParticleLifetimeMin : (float?) null; }
				set { if (value != null) { ParticleLifetimeMin = value.Value; ParticleLifetimeMax = value.Value;}}
			}

			public float ParticleSpawnAngleMin = 0;
			public float ParticleSpawnAngleMax = FloatMath.TAU;
			public float? ParticleSpawnAngle
			{
				get { return FloatMath.FloatEquals(ParticleSpawnAngleMin, ParticleSpawnAngleMax) ? ParticleSpawnAngleMin : (float?) null; }
				set { if (value != null) { ParticleSpawnAngleMin = value.Value; ParticleSpawnAngleMax = value.Value;}}
			}

			public float ParticleVelocityMin = 0;
			public float ParticleVelocityMax = FloatMath.TAU;
			public float? ParticleVelocity
			{
				get { return FloatMath.FloatEquals(ParticleVelocityMin, ParticleVelocityMax) ? ParticleVelocityMin : (float?)null; }
				set { if (value != null) { ParticleVelocityMin = value.Value; ParticleVelocityMax = value.Value; } }
			}

			public float ParticleAlphaInitial = 1f;
			public float ParticleAlphaFinal = 1f;
			public float? ParticleAlpha
			{
				get { return FloatMath.FloatEquals(ParticleAlphaInitial, ParticleAlphaFinal) ? ParticleAlphaInitial : (float?)null; }
				set { if (value != null) { ParticleAlphaInitial = value.Value; ParticleAlphaFinal = value.Value; } }
			}

			public float ParticleSizeInitialMin = 1f;
			public float ParticleSizeInitialMax = 1f;
			public float? ParticleSizeInitial
			{
				get { return FloatMath.FloatEquals(ParticleSizeInitialMin, ParticleSizeInitialMax) ? ParticleSizeInitialMin : (float?)null; }
				set { if (value != null) { ParticleSizeInitialMin = value.Value; ParticleSizeInitialMax = value.Value; } }
			}

			public float ParticleSizeFinalMin = 1f;
			public float ParticleSizeFinalMax = 1f;
			public float? ParticleSizeFinal
			{
				get { return FloatMath.FloatEquals(ParticleSizeFinalMin, ParticleSizeFinalMax) ? ParticleSizeFinalMin : (float?)null; }
				set { if (value != null) { ParticleSizeFinalMin = value.Value; ParticleSizeFinalMax = value.Value; } }
			}

			public float? ParticleSize
			{
				get { return FloatMath.FloatEquals(ParticleSizeInitialMin, ParticleSizeInitialMax, ParticleSizeFinalMin, ParticleSizeFinalMax) ? ParticleSizeInitialMin : (float?)null; }
				set { if (value != null) { ParticleSizeInitialMin = value.Value; ParticleSizeInitialMax = value.Value; ParticleSizeFinalMin = value.Value; ParticleSizeFinalMax = value.Value; } }
			}

			public Color ColorInitial = Microsoft.Xna.Framework.Color.White;
			public Color ColorFinal = Microsoft.Xna.Framework.Color.White;
			public Color? Color
			{
				get { return (ColorInitial == ColorFinal) ? ColorInitial : (Color?)null; }
				set { if (value != null) { ColorInitial = value.Value; ColorFinal = value.Value; } }
			}

			public static ParticleEmitterConfigBuilder LoadFromXConfig(XConfigFile xcfg)
			{
				var builder = new ParticleEmitterConfigBuilder();

				builder.TextureIndex = xcfg.GetInt("TextureIndex");
				builder.SpawnRate = xcfg.GetInt("SpawnRate");

				if (xcfg.Contains("ParticleLifetime")) builder.ParticleLifetime = xcfg.GetFloat("ParticleLifetime");
				if (xcfg.Contains("ParticleLifetimeMin")) builder.ParticleLifetimeMin = xcfg.GetFloat("ParticleLifetimeMin");
				if (xcfg.Contains("ParticleLifetimeMax")) builder.ParticleLifetimeMax = xcfg.GetFloat("ParticleLifetimeMax");

				if (xcfg.Contains("ParticleSpawnAngle")) builder.ParticleSpawnAngle = xcfg.GetFloat("ParticleSpawnAngle");
				if (xcfg.Contains("ParticleSpawnAngleMin")) builder.ParticleSpawnAngleMin = xcfg.GetFloat("ParticleSpawnAngleMin");
				if (xcfg.Contains("ParticleSpawnAngleMax")) builder.ParticleSpawnAngleMax = xcfg.GetFloat("ParticleSpawnAngleMax");

				if (xcfg.Contains("ParticleVelocity")) builder.ParticleVelocity = xcfg.GetFloat("ParticleVelocity") ;
				if (xcfg.Contains("ParticleVelocityMin")) builder.ParticleVelocityMin = xcfg.GetFloat("ParticleVelocityMin") ;
				if (xcfg.Contains("ParticleVelocityMax")) builder.ParticleVelocityMax = xcfg.GetFloat("ParticleVelocityMax") ;

				if (xcfg.Contains("ParticleAlpha")) builder.ParticleAlpha = xcfg.GetFloat("ParticleAlpha");
				if (xcfg.Contains("ParticleAlphaInitial")) builder.ParticleAlphaInitial = xcfg.GetFloat("ParticleAlphaInitial");
				if (xcfg.Contains("ParticleAlphaFinal")) builder.ParticleAlphaFinal = xcfg.GetFloat("ParticleAlphaFinal");

				if (xcfg.Contains("ParticleSize")) builder.ParticleSize = xcfg.GetFloat("ParticleSize") ;

				if (xcfg.Contains("ParticleSizeInitial")) builder.ParticleSizeInitial = xcfg.GetFloat("ParticleSizeInitial") ;
				if (xcfg.Contains("ParticleSizeInitialMin")) builder.ParticleSizeInitialMin = xcfg.GetFloat("ParticleSizeInitialMin") ;
				if (xcfg.Contains("ParticleSizeInitialMax")) builder.ParticleSizeInitialMax = xcfg.GetFloat("ParticleSizeInitialMax") ;

				if (xcfg.Contains("ParticleSizeFinal")) builder.ParticleSizeFinal = xcfg.GetFloat("ParticleSizeFinal");
				if (xcfg.Contains("ParticleSizeFinalMin")) builder.ParticleSizeFinalMin = xcfg.GetFloat("ParticleSizeFinalMin");
				if (xcfg.Contains("ParticleSizeFinalMax")) builder.ParticleSizeFinalMax = xcfg.GetFloat("ParticleSizeFinalMax");

				if (xcfg.Contains("Color")) builder.Color = xcfg.GetKnownColor("Color");
				if (xcfg.Contains("ColorInitial")) builder.ColorInitial = xcfg.GetKnownColor("ColorInitial");
				if (xcfg.Contains("ColorFinal")) builder.ColorFinal = xcfg.GetKnownColor("ColorFinal");

				return builder;
			}

			public ParticleEmitterConfig Build(TextureRegion2D[] texArray, float size = 1, float length = 1) => new ParticleEmitterConfig(this, texArray, size, length);
		}

		private static readonly Vector2 vectorOne = Vector2.UnitX;

		public readonly TextureRegion2D Texture;
		public readonly Rectangle TextureBounds;
		public readonly FPoint TextureCenter;
		public readonly float TextureSize;

		public readonly float SpawnRate;
		public readonly float SpawnDelay;

		public readonly float ParticleLifetimeMin;
		public readonly float ParticleLifetimeMax;
		public readonly bool ParticleLifetimeIsRandom;

		public readonly float ParticleRespawnTime;

		public readonly float ParticleSpawnAngleMin;
		public readonly float ParticleSpawnAngleMax;
		public readonly bool ParticleSpawnAngleIsRandom;
		public readonly bool ParticleSpawnAngleIsTotal;
		public readonly Vector2 FixedParticleSpawnAngle;

		public readonly float ParticleVelocityMin;
		public readonly float ParticleVelocityMax;
		public readonly bool ParticleVelocityIsRandom;

		public readonly float ParticleAlphaInitial;
		public readonly float ParticleAlphaFinal;

		public readonly float ParticleSizeInitialMin;
		public readonly float ParticleSizeInitialMax;
		public readonly bool ParticleSizeInitialIsRandom;

		public readonly float ParticleSizeFinalMin;
		public readonly float ParticleSizeFinalMax;
		public readonly bool ParticleSizeFinalIsRandom;

		public readonly Color ColorInitial;
		public readonly Color ColorFinal;
		public readonly bool ColorIsChanging;

		internal ParticleEmitterConfig(ParticleEmitterConfigBuilder b, TextureRegion2D[] texArray, float size, float length)
		{
			Texture = texArray[b.TextureIndex];
			TextureBounds = Texture.Bounds;
			TextureCenter = Texture.Center();
			TextureSize = TextureBounds.Width;

			SpawnRate = b.SpawnRate * length;

			SpawnDelay = 1f / b.SpawnRate;

			ParticleLifetimeMin = b.ParticleLifetimeMin;
			ParticleLifetimeMax = b.ParticleLifetimeMax;
			ParticleRespawnTime = b.ParticleLifetimeMax;
			ParticleLifetimeIsRandom = FloatMath.FloatInequals(ParticleLifetimeMin, ParticleLifetimeMax);

			ParticleSpawnAngleMin = b.ParticleSpawnAngleMin;
			ParticleSpawnAngleMax = b.ParticleSpawnAngleMax;
			ParticleSpawnAngleIsRandom = FloatMath.FloatInequals(ParticleSpawnAngleMin, ParticleSpawnAngleMax);
			ParticleSpawnAngleIsTotal = FloatMath.FloatInequals(ParticleSpawnAngleMin, ParticleSpawnAngleMax) && FloatMath.EpsilonEquals(FloatMath.NormalizeAngle(ParticleSpawnAngleMin), FloatMath.NormalizeAngle(ParticleSpawnAngleMax));
			if (!ParticleSpawnAngleIsRandom) FixedParticleSpawnAngle = vectorOne.Rotate(ParticleSpawnAngleMin);

			ParticleVelocityMin = b.ParticleVelocityMin ;
			ParticleVelocityMax = b.ParticleVelocityMax ;
			ParticleVelocityIsRandom = FloatMath.FloatInequals(ParticleVelocityMin, ParticleVelocityMax);

			ParticleAlphaInitial = b.ParticleAlphaInitial;
			ParticleAlphaFinal = b.ParticleAlphaFinal;

			ParticleSizeInitialMin = b.ParticleSizeInitialMin * size;
			ParticleSizeInitialMax = b.ParticleSizeInitialMax * size;
			ParticleSizeInitialIsRandom = FloatMath.FloatInequals(ParticleSizeInitialMin, ParticleSizeInitialMax);

			ParticleSizeFinalMin = b.ParticleSizeFinalMin * size;
			ParticleSizeFinalMax = b.ParticleSizeFinalMax * size;
			ParticleSizeFinalIsRandom = FloatMath.FloatInequals(ParticleSizeFinalMin, ParticleSizeFinalMax);

			ColorInitial = b.ColorInitial;
			ColorFinal = b.ColorFinal;
			ColorIsChanging = (ColorInitial != ColorFinal);
		}

		public float GetParticleLifetime()
		{
			if (ParticleLifetimeIsRandom)
				return FloatMath.GetRangedRandom(ParticleLifetimeMin, ParticleLifetimeMax);
			else
				return ParticleLifetimeMin;
		}

		public float GetParticleAngle()
		{
			if (ParticleSpawnAngleIsRandom)
				return FloatMath.GetRangedRandom(ParticleSpawnAngleMin, ParticleSpawnAngleMax);
			else
				return ParticleSpawnAngleMin;
		}

		public void SetParticleVelocity(ref Vector2 vec)
		{
			var velocity = GetParticleVelocity();

			if (ParticleSpawnAngleIsRandom)
			{
				var rvec = vectorOne.Rotate(GetParticleAngle());

				vec.X = rvec.X * velocity;
				vec.Y = rvec.Y * velocity;
			}
			else
			{
				vec.X = FixedParticleSpawnAngle.X * velocity;
				vec.Y = FixedParticleSpawnAngle.Y * velocity;
			}
		}

		public float GetParticleVelocity()
		{
			if (ParticleVelocityIsRandom)
				return FloatMath.GetRangedRandom(ParticleVelocityMin, ParticleVelocityMax);
			else
				return ParticleVelocityMin;
		}

		public float GetParticleSizeInitial()
		{
			if (ParticleSizeInitialIsRandom)
				return FloatMath.GetRangedRandom(ParticleSizeInitialMin, ParticleSizeInitialMax);
			else
				return ParticleSizeInitialMin;
		}

		public float GetParticleSizeFinal()
		{
			if (ParticleSizeFinalIsRandom)
				return FloatMath.GetRangedRandom(ParticleSizeFinalMin, ParticleSizeFinalMax);
			else
				return ParticleSizeFinalMin;
		}
	}
}
