using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using System;
using System.Linq;
using MonoSAMFramework.Portable.Extensions;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class BulletCannon : Cannon
	{
		private readonly CannonBlueprint Blueprint;

		private float barrelCharge = 0f;
		private float barrelRecoil = 0f;
		private float cannonCogRotation;

		public BulletCannon(GDGameScreen scrn, CannonBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;
		}

		#region Update

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			Rotation.Update(gameTime);
			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateCog(gameTime);
			UpdateBarrel(gameTime);

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

		private void UpdateBarrel(SAMTime gameTime)
		{
			if ((CannonHealth.TargetValue >= 1 || Fraction.IsNeutral) && controller.DoBarrelRecharge())
			{
				float chargeDelta = BARREL_CHARGE_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds;
				if (Scale > 2.5f) chargeDelta /= Scale;

				barrelCharge += chargeDelta;

				if (barrelCharge >= 1f)
				{
					barrelCharge -= 1f;

					Shoot();
				}
			}

			if (barrelRecoil < 1)
			{
				barrelRecoil = FloatMath.LimitedInc(barrelRecoil, BARREL_RECOIL_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds, 1f);
			}
		}
		
		private void UpdateCog(SAMTime gameTime)
		{
			if (CannonHealth.ActualValue >= 1 || (CannonHealth.ActualValue <= 0 && Fraction.IsNeutral))
			{
				var rotInc = BASE_COG_ROTATION_SPEED * Fraction.Multiplicator * RealBoost * gameTime.ElapsedSeconds;

				cannonCogRotation = (cannonCogRotation + rotInc) % (FloatMath.PI / 2);
			}
			else
			{
				if (FloatMath.FloatInequals(cannonCogRotation, FloatMath.PI / 2))
				{
					var rotInc = BASE_COG_ROTATION_SPEED * Fraction.GetNeutral().Multiplicator * RealBoost * gameTime.ElapsedSeconds;

					bool isLimited;
					cannonCogRotation = FloatMath.LimitedInc(cannonCogRotation, rotInc, FloatMath.PI / 2, out isLimited);
					if (isLimited) cannonCogRotation = FloatMath.PI / 2;
				}
			}
		}

		private void Shoot()
		{
			var position = GetBulletSpawnPoint();
			var velocity = GetBulletVelocity();

			//Screen.PushNotification($"Cannon :: Shoot ({position.X:000.0}|{position.Y:000.0}) at {FloatMath.ToDegree(velocity.ToAngle()):000}°");

			barrelRecoil = 0f;

			Manager.AddEntity(new Bullet(GDOwner, this, position, velocity, Scale, Fraction));
			MainGame.Inst.GDSound.PlayEffectShoot();
		}

		public Vector2 GetBulletSpawnPoint()
		{
			return Position + new Vector2(Scale * (CANNON_DIAMETER / 2 + Bullet.BULLET_DIAMETER * 0.66f), 0).Rotate(Rotation.ActualValue);
		}

		public Vector2 GetBulletVelocity()
		{
			var variance = FloatMath.GetRangedRandom(-BULLET_ANGLE_VARIANCE, +BULLET_ANGLE_VARIANCE);
			var angle = FloatMath.AddRads(Rotation.ActualValue, variance);

			return new Vector2(1, 0).Rotate(angle) * BULLET_INITIAL_SPEED;
		}

		#endregion

		#region Draw

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			DrawBodyAndBarrel_BG(sbatch);
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			DrawCrosshair(sbatch);
			DrawBodyAndBarrel_FG(sbatch);
			DrawCog(sbatch);

#if DEBUG
			// ASSERTION
			if (ActiveOperations.Count(p => p is CannonBooster) != FloatMath.Round(TotalBoost / BOOSTER_POWER)) throw new Exception("Assertion failed TotalBoost == Boosters");
#endif
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
			var recoil = (1 - barrelRecoil) * BARREL_RECOIL_LENGTH;

			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);

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
			var recoil = (1 - barrelRecoil) * BARREL_RECOIL_LENGTH;

			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation.ActualValue);

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

		private void DrawCog(IBatchRenderer sbatch)
		{
			sbatch.DrawScaled(
				Textures.AnimCannonCog[Textures.ANIMATION_CANNONCOG_SIZE - 1],
				Position,
				Scale,
				FlatColors.Clouds,
				cannonCogRotation + 3 * (FloatMath.PI / 2));

			sbatch.DrawScaled(
				Textures.AnimCannonCog[(int)(CannonHealth.ActualValue * (Textures.ANIMATION_CANNONCOG_SIZE - 1))],
				Position,
				Scale,
				Fraction.Color,
				cannonCogRotation + 3 * (FloatMath.PI / 2));
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
			var rectChargeProg = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (0 * 12) + 4, innerRadius * 2 * barrelCharge, 8);

			sbatch.FillRectangle(rectChargeFull, Color.White);
			sbatch.FillRectangle(rectChargeProg, Color.DarkGray);
			sbatch.DrawRectangle(rectChargeFull, Color.Black);

			var rectHealthFull = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2, 8);
			var rectHealthProgT = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.TargetValue, 8);
			var rectHealthProgA = new FRectangle(Position.X - innerRadius, Position.Y + innerRadius + (1 * 12) + 4, innerRadius * 2 * CannonHealth.ActualValue, 8);

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

		#endregion

		#region Change State

		public override void ResetChargeAndBooster()
		{
			barrelCharge = 0f;

			FinishAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			barrelCharge = 0f;
		}

		#endregion
	}
}
