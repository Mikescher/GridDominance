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
		private const float CORE_PULSE_FREQ = 2f;
		private const float CORE_PULSE = 0.06f; // perc

		private readonly LaserCannonBlueprint Blueprint;
		private readonly LaserSource _laserSource;

		private readonly DeltaLimitedFloat corePulse = new DeltaLimitedFloat(1, CORE_PULSE * CORE_PULSE_FREQ * 2);

		private readonly int coreImage;
		private readonly float coreRotation;

		public LaserCannon(GDGameScreen scrn, LaserCannonBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;

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
				Scale * corePulse.ActualValue,
				Color.White,
				coreRotation);

			if (!Fraction.IsNeutral && CannonHealth.ActualValue > 0)
			{
				sbatch.DrawScaled(
					Textures.TexCannonCore[coreImage],
					Position,
					Scale * corePulse.ActualValue * CannonHealth.ActualValue,
					Fraction.Color,
					coreRotation);
			}
		}

#if DEBUG

		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			DrawDebugView(sbatch);

			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost / BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
		}

		private void DrawDebugView(IBatchRenderer sbatch)
		{
			var innerRadius = Scale * CANNON_DIAMETER / 2;

			var rectChargeFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (0 * 12) + 4, innerRadius * 2, 8);
			var rectChargeProg = new FRectangle(Position.X, Position.Y + innerRadius + (0 * 12) + 4, ((corePulse.ActualValue-1)/CORE_PULSE) * innerRadius, 8).AsNormalized();

			var rectHealthFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2, 8);
			var rectHealthProgT = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.TargetValue, 8);
			var rectHealthProgA = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.ActualValue, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.OrangeRed);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);
			
			if (CannonHealth.IsDecreasing())
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color.Lighten());
				sbatch.FillRectangle(rectHealthProgT, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}
			else if (CannonHealth.IsIncreasing())
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgT, Fraction.Color.Lighten());
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}
			else
			{
				sbatch.FillRectangle(rectHealthFull, Color.White);
				sbatch.FillRectangle(rectHealthProgA, Fraction.Color);
				sbatch.DrawRectangle(rectHealthFull, Color.Black);
			}

			for (int i = 0; i < ActiveOperations.Count; i++)
			{
				var rectFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + ((i + 2) * 12) + 16, innerRadius * 2, 8);
				var rectProg = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + ((i + 2) * 12) + 16, innerRadius * 2 * (1 - ActiveOperations[i].Progress), 8);

				sbatch.FillRectangle(rectFull, Color.White);
				sbatch.FillRectangle(rectProg, Color.Chocolate);
				sbatch.DrawRectangle(rectFull, Color.Black);
			}

			var kicontroller = controller as KIController;
			if (kicontroller != null)
			{
				var r = new FRectangle(Position.X - DrawingBoundingBox.Width * 0.5f, Position.Y - DrawingBoundingBox.Height / 2f, DrawingBoundingBox.Width, 12);

				sbatch.FillRectangle(r, Color.LightGray * 0.5f);
				FontRenderHelper.DrawSingleLineInBox(sbatch, Textures.DebugFontSmall, kicontroller.LastKIFunction, r, 1, true, Color.Black);
			}
		}
#endif
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			Rotation.Update(gameTime);
			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateNetwork(gameTime);
			UpdateCore(gameTime);

#if DEBUG
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("AssimilateCannon"))
			{
				while (Fraction.Type != FractionType.PlayerFraction)
					TakeDamage(this.GDOwner().GetPlayerFraction(), 1);

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
				corePulse.Set(1);
			else
				corePulse.Set(1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * CORE_PULSE_FREQ) * CORE_PULSE);
			corePulse.Update(gameTime);
		}

		private void UpdateNetwork(SAMTime gameTime)
		{
			bool active = CannonHealth.TargetValue >= 1 && controller.DoBarrelRecharge();
			_laserSource.SetState(active, Fraction, Rotation.ActualValue);
		}

		public override void ResetChargeAndBooster()
		{
			FinishAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			//
		}
	}
}
