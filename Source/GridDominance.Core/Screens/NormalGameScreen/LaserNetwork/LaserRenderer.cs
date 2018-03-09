using System.Linq;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	class LaserRenderer : GameEntity
	{
		private const float RAY_WIDTH            = 8f;
		private const float FLARE_SIZE_SMALL     = 16f;
		private const float FLARE_SIZE_BIG       = 32f;
		private const float FLARE_ROTATION_SPEED = 0.1f; // rot/sec
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
						sbatch.DrawCentered(
							(src.Type == RayType.Laser) ? Textures.TexLaserBase : Textures.TexShieldLaserBase, 
							FPoint.MiddlePoint(ray.Start, ray.End), 
							ray.Length + RAY_WIDTH / 4f, 
							RAY_WIDTH, 
							Color.White, 
							ray.Angle);
					}
					else
					{
						sbatch.DrawCentered(
							(src.Type == RayType.Laser) ? Textures.TexLaserPointer : Textures.TexShieldLaserPointer, 
							FPoint.MiddlePoint(ray.Start, ray.End), 
							ray.Length, RAY_WIDTH,
							Color.White, 
							ray.Angle);
					}
				}
			}

			foreach (var src in _network.Sources)
			{
				if (src.LaserPowered)
				{
					var pwr = FloatMath.AbsSin(src.UserData.LaserPulseTime * FloatMath.TAU * Cannon.LASER_GLOW_FREQ);

					foreach (var ray in src.Lasers)
					{
						sbatch.DrawCentered(
							(src.Type == RayType.Laser) ? Textures.TexLaserGlow : Textures.TexShieldLaserGlow, 
							FPoint.MiddlePoint(ray.Start, ray.End), 
							ray.Length + RAY_WIDTH / 4f, 
							2*RAY_WIDTH, 
							src.LaserFraction.Color * pwr, 
							ray.Angle);
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
							sbatch.DrawCentered(ray.RayType == RayType.Laser ? Textures.TexLaserFlare : Textures.TexShieldLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.Target:
							if (ray.RayType == RayType.Laser) sbatch.DrawCentered(Textures.TexLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.LaserMultiTerm:

							var lsr = ray.RayType == RayType.Laser || ray.TerminatorRays.Any(r => r.Item1.RayType == RayType.Laser);

							sbatch.DrawCentered(lsr ? Textures.TexLaserFlareHalf : Textures.TexShieldLaserFlareHalf, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.LaserSelfTerm:
						case LaserRayTerminator.LaserFaultTerm:
							sbatch.DrawCentered(ray.RayType == RayType.Laser ? Textures.TexLaserFlare : Textures.TexShieldLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;
						case LaserRayTerminator.BulletTerm:
							sbatch.DrawCentered(ray.RayType == RayType.Laser ? Textures.TexLaserFlare : Textures.TexShieldLaserFlare, ray.End, size, size, Color.White, _flareRotation);
							break;

						case LaserRayTerminator.OOB:
						case LaserRayTerminator.Portal:
						case LaserRayTerminator.Glass:
						case LaserRayTerminator.Mirror:
							//no draw
							break;

						default:
							SAMLog.Error("LASER::EnumSwitch_DN", "value: " + ray.Terminator);
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
