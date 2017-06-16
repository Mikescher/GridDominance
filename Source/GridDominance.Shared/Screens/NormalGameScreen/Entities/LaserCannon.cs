using System;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class LaserCannon : Cannon
	{
		
		private readonly LaserCannonBlueprint Blueprint;
		private readonly LaserSource _laserSource;
		private readonly GDGameScreen _screen;

		public readonly DeltaLimitedFloat CorePulse = new DeltaLimitedFloat(1, CORE_PULSE * CORE_PULSE_FREQ * 2);

		private readonly int coreImage;
		private readonly float coreRotation;
		private float chargeTime = 0f;

		public LaserCannon(GDGameScreen scrn, LaserCannonBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;
			_screen = scrn;

			_laserSource = scrn.LaserNetwork.AddSource(this);

			coreImage = FloatMath.GetRangedIntRandom(0, Textures.CANNONCORE_COUNT);
			coreRotation = FloatMath.GetRangedRandom(FloatMath.RAD_POS_000, FloatMath.RAD_POS_360);
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			DrawBodyAndBarrel_BG(sbatch);
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			DrawCrosshair(sbatch);
			DrawBodyAndBarrel_FG(sbatch);
			DrawCore(sbatch);
		}

		private void DrawCrosshair(IBatchRenderer sbatch)
		{
			if (FloatMath.IsNotZero(CrosshairSize.ActualValue))
			{
				sbatch.DrawScaled(
					Textures.TexCannonCrosshair,
					Position,
					Scale * CrosshairSize.ActualValue,
					Color.White * (CROSSHAIR_TRANSPARENCY * CrosshairSize.ActualValue),
					Rotation.TargetValue);
			}
		}

		private void DrawBodyAndBarrel_BG(IBatchRenderer sbatch)
		{
			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f), 0).Rotate(Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBodyShadow,
				Position,
				Scale,
				Color.White,
				Rotation.ActualValue);
		}

		private void DrawBodyAndBarrel_FG(IBatchRenderer sbatch)
		{
			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f), 0).Rotate(Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter,
				Scale,
				Color.White,
				Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexCannonBody,
				Position,
				Scale,
				Color.White,
				Rotation.ActualValue);
		}

		private void DrawCore(IBatchRenderer sbatch)
		{
			sbatch.DrawScaled(
				Textures.TexCannonCoreShadow[coreImage],
				Position,
				Scale * CorePulse.ActualValue,
				Color.White,
				coreRotation);

			if (!Fraction.IsNeutral && CannonHealth.ActualValue > 0)
			{
				sbatch.DrawScaled(
					Textures.TexCannonCore[coreImage],
					Position,
					Scale * CorePulse.ActualValue * FloatMath.Sqrt(CannonHealth.ActualValue),
					Fraction.Color,
					coreRotation);
			}
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			bool change = Rotation.CUpdate(gameTime);
			if (change) {_screen.LaserNetwork.SemiDirty = true; chargeTime = 0; }

			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateBoost(gameTime);
			UpdateNetwork(gameTime);
			UpdateCore(gameTime);
			UpdateDamage(gameTime);

#if DEBUG
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AssimilateCannon"))
			{
				var bckp = DebugSettings.Get("ImmortalCannons");
				DebugSettings.SetManual("ImmortalCannons", false);

				while (Fraction.Type != FractionType.PlayerFraction)
					TakeDamage(this.GDOwner().GetPlayerFraction(), 1);

				DebugSettings.SetManual("ImmortalCannons", bckp);

				CannonHealth.SetForce(1f);
			}
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AbandonCannon"))
			{
				CannonHealth.SetForce(0f);
				SetFraction(Fraction.GetNeutral());
			}
#endif
		}

		private void UpdateCore(SAMTime gameTime)
		{
			if (CannonHealth.ActualValue < 1 || Fraction.IsNeutral)
				CorePulse.Set(1);
			else
				CorePulse.Set(1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * CORE_PULSE_FREQ) * CORE_PULSE);
			CorePulse.Update(gameTime);

			chargeTime += gameTime.ElapsedSeconds;

			if (CannonHealth.ActualValue < 1) chargeTime = 0;
		}

		private void UpdateNetwork(SAMTime gameTime)
		{
			bool active = CannonHealth.TargetValue >= 1 && controller.DoBarrelRecharge();

			_laserSource.SetState(active, Fraction, Rotation.ActualValue, chargeTime > LASER_CHARGE_COOLDOWN);
		}

		private void UpdateDamage(SAMTime gameTime)
		{
			if (!_laserSource.LaserPowered || !_laserSource.LaserActive) return;

			foreach (var ray in _laserSource.Lasers)
			{
				if (ray.Terminator != LaserRayTerminator.Target) continue;
				if (ray.TerminatorCannon == null) continue;

				if (ray.TerminatorCannon.Fraction == Fraction)
				{
					if (ray.TerminatorCannon == this) continue; // stop touching yourself

					ray.TerminatorCannon.ApplyLaserBoost(this, Fraction.Multiplicator * Scale * gameTime.ElapsedSeconds * LASER_BOOST_PER_SECOND);
				}
				else
				{
					ray.TerminatorCannon.TakeLaserDamage(Fraction, ray, Fraction.Multiplicator * Scale * gameTime.ElapsedSeconds * LASER_DAMAGE_PER_SECOND);
				}
				
			}
		}

		public override void ResetChargeAndBooster()
		{
			FinishAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			chargeTime = 0f;
		}
	}
}
