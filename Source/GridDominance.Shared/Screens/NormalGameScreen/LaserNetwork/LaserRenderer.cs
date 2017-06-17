using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Levelfileformat.Blueprint;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	class LaserRenderer : GameEntity
	{
		private const float RAY_WIDTH            = 8f;
		private const float FLARE_SIZE_SMALL     = 16f;
		private const float FLARE_SIZE_BIG       = 32f;
		private const float FLARE_ROTATION_SPEED = 0.1f; // rot/sec
		private const float SPECK_TRAVEL_SPEED   = 1024f;
		private const float SPECK_DISTANCE       = 128f;
		private const float FLARE_PULSE_TIME     = 0.4f;
		private const float FLARE_PULSE_SCALE    = 2f;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.Transparent;

		private readonly LaserNetwork _network;

		private float _flareRotation = 0f;
		private float _flareScale    = 0f;

		public LaserRenderer(GDGameScreen scrn, LaserNetwork nw, LevelBlueprint bp) : base(scrn, GDConstants.ORDER_GAME_LASER)
		{
			_network = nw;

			Position = new FPoint(bp.LevelWidth / 2f, bp.LevelHeight / 2f);
			DrawingBoundingBox = new FSize(bp.LevelWidth, bp.LevelHeight);
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			_network.Update(gameTime, istate);

			_flareRotation = (_flareRotation + (FLARE_ROTATION_SPEED * FloatMath.TAU) * gameTime.ElapsedSeconds) % FloatMath.RAD_POS_360;

			_flareScale += gameTime.ElapsedSeconds * (FLARE_PULSE_TIME * FloatMath.TAU);

			foreach (var src in _network.Sources)
			{
				if (src.LaserPowered)
					src.SpeckTravel += SPECK_TRAVEL_SPEED * gameTime.ElapsedSeconds;
				else
					src.SpeckTravel = 0;
			}
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			DrawNetwork(sbatch);

#if DEBUG
			if (DebugSettings.Get("DebugLaserNetwork"))
			{
				DrawNetworkDebug(sbatch);
			}
#endif
		}

		private void DrawNetwork(IBatchRenderer sbatch)
		{
			foreach (var src in _network.Sources)
			{
				foreach (var ray in src.Lasers)
				{
					if (src.LaserPowered)
					{
						sbatch.DrawCentered(Textures.TexLaserBase, FPoint.MiddlePoint(ray.Start, ray.End), ray.Length + RAY_WIDTH / 4f, RAY_WIDTH, Color.White, ray.Angle);
					}
					else
					{
						sbatch.DrawCentered(Textures.TexLaserPointer, FPoint.MiddlePoint(ray.Start, ray.End), ray.Length, RAY_WIDTH, Color.White, ray.Angle);
					}
				}
			}

			foreach (var src in _network.Sources)
			{
				foreach (var ray in src.Lasers)
				{
					if (src.LaserPowered)
					{
						float d = SPECK_DISTANCE * (int)(ray.SourceDistance / SPECK_DISTANCE) - SPECK_DISTANCE + src.SpeckTravel % SPECK_DISTANCE;
						for (;; d += SPECK_DISTANCE)
						{
							if (d < ray.SourceDistance) continue;
							if (d > ray.SourceDistance + ray.Length) break;
							if (d > src.SpeckTravel) break;

							var p = ray.Start + (ray.End - ray.Start) * ((d - ray.SourceDistance) / ray.Length);

							sbatch.DrawCentered(Textures.TexParticle[16], p, 2*RAY_WIDTH, 2*RAY_WIDTH, src.LaserFraction.Color);
						}
					}
				}
			}

			foreach (var src in _network.Sources)
			{
				foreach (var ray in src.Lasers)
				{
					var size = FLARE_SIZE_SMALL;
					if (src.LaserPowered) size = FLARE_SIZE_BIG;

					size += FloatMath.Sin(_flareScale) * FLARE_PULSE_SCALE;
					
					switch (ray.Terminator)
					{
						case LaserRayTerminator.VoidObject:
							sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.Target:
							sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.LaserMultiTerm:
							sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White * 0.5f, _flareRotation);
							break;
						case LaserRayTerminator.LaserSelfTerm:
						case LaserRayTerminator.LaserFaultTerm:
							sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.BulletTerm:
							sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
					}
				}
			}
		}

		private void DrawNetworkDebug(IBatchRenderer sbatch)
		{
			foreach (var src in _network.Sources)
			{
				foreach (var ray in src.Lasers)
				{
					sbatch.DrawLine(ray.Start, ray.End, src.LaserPowered ? Color.LimeGreen : Color.LightGreen, 4);

					if (ray.Terminator == LaserRayTerminator.LaserMultiTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.Salmon);
					if (ray.Terminator == LaserRayTerminator.LaserSelfTerm)  sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.CornflowerBlue);
					if (ray.Terminator == LaserRayTerminator.LaserFaultTerm) sbatch.FillRectangle(ray.End - new Vector2(4, 4), new FSize(8, 8), Color.Magenta);
				}
			}
		}
	}
}
