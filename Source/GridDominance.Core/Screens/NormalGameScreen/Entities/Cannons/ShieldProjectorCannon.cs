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
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Sound;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using System;
using GridDominance.Shared.Screens.Common;
using MonoSAMFramework.Portable.ColorHelper;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class ShieldProjectorCannon : Cannon, ILaserCannon
	{
		private const float RAY_FORCE = 0.175f;

		public readonly ShieldProjectorBlueprint Blueprint;
		public readonly LaserSource LaserSource;
		private readonly GDGameScreen _screen;

		public readonly DeltaLimitedFloat CorePulse  = new DeltaLimitedFloat(1, CORE_PULSE * CORE_PULSE_FREQ * 2);
		public float LaserPulseTime = 0f;

		private float _coreRotation = 0f;
		public float ChargeTime = 0f;

		private readonly SAMEffectWrapper _soundeffect;
		private readonly bool _muted;

		float ILaserCannon.LaserPulseTime => LaserPulseTime;

		public override bool IsLaser => true;

		public readonly DeltaLimitedFloat SatelliteExpansion = new DeltaLimitedFloat(0f, SHIELD_SATELLITE_EXPANSIONSPEED);

		public ShieldProjectorCannon(GDGameScreen scrn, ShieldProjectorBlueprint bp, Fraction[] fractions) : 
			base(scrn, fractions, bp.Player, bp.X, bp.Y, bp.Diameter, bp.CannonID, bp.Rotation, bp.PrecalculatedPaths)
		{
			Blueprint = bp;
			_screen = scrn;
			_muted = scrn.IsPreview;

			LaserSource = scrn.LaserNetwork.AddSource(this);

			_soundeffect = MainGame.Inst.GDSound.GetEffectLaser(this);
			_soundeffect.IsLooped = true;
		}

		protected override void CreatePhysics()
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), 0, BodyType.Static);

			PhysicsFixtureBase = FixtureFactory.AttachCircle(
				ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 1,
				PhysicsBody,
				Vector2.Zero, this);

			FixtureFactory.AttachRectangle(
				ConvertUnits.ToSimUnits(Scale * BARREL_WIDTH), ConvertUnits.ToSimUnits(Scale * LASER_BARREL_HEIGHT), 1,
				new Vector2(ConvertUnits.ToSimUnits(Scale * CANNON_DIAMETER / 2), 0),
				PhysicsBody, this);
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			CommonCannonRenderer.DrawShieldCannon_BG(sbatch, Position, Scale, Rotation.ActualValue);
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			DrawCrosshair(sbatch);

			CommonCannonRenderer.DrawShieldCannon_FG(sbatch, Position, Scale, Rotation.ActualValue, Fraction.IsNeutral, CannonHealth.ActualValue, _coreRotation, SatelliteExpansion.ActualValue, Fraction.Color);

			//DrawShield(sbatch);
		}

		private void DrawBodyAndBarrel_BG(IBatchRenderer sbatch)
		{
			var barrelCenter = Position + new Vector2(Scale * (CANNON_DIAMETER / 2f), 0).Rotate(Rotation.ActualValue);

			sbatch.DrawScaled(
				Textures.TexShieldBarrelShadow,
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
				Textures.TexShieldBarrel,
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
			if (SatelliteExpansion.ActualValue <= 0)
			{
				sbatch.DrawCentered(
					Textures.TexCircle,
					Position,
					Scale * CANNON_DIAMETER * 0.5f,
					Scale * CANNON_DIAMETER * 0.5f,
					FlatColors.Clouds);

				if (!Fraction.IsNeutral && CannonHealth.ActualValue > 0)
				{
					sbatch.DrawCentered(
						Textures.TexCircle,
						Position,
						Scale * CANNON_DIAMETER * 0.5f * CannonHealth.ActualValue,
						Scale * CANNON_DIAMETER * 0.5f * CannonHealth.ActualValue,
						Fraction.Color);
				}
			}
			else
			{
				if (Fraction.IsNeutral)
				{
					sbatch.DrawCentered(
						Textures.TexCircle,
						Position,
						Scale * CANNON_DIAMETER * 0.5f,
						Scale * CANNON_DIAMETER * 0.5f,
						FlatColors.Clouds);
				}
				else
				{
					var p1 = Position + new Vector2(Scale * CANNON_DIAMETER * 0.2f * SatelliteExpansion.ActualValue, 0).Rotate(_coreRotation + FloatMath.RAD_POS_000);
					var p2 = Position + new Vector2(Scale * CANNON_DIAMETER * 0.2f * SatelliteExpansion.ActualValue, 0).Rotate(_coreRotation + FloatMath.RAD_POS_120);
					var p3 = Position + new Vector2(Scale * CANNON_DIAMETER * 0.2f * SatelliteExpansion.ActualValue, 0).Rotate(_coreRotation + FloatMath.RAD_POS_240);

					var ws = Scale * CANNON_DIAMETER * 0.5f;
					var we = Scale * CANNON_DIAMETER * 0.5f / 2f;

					var diam = ws + (we - ws) * SatelliteExpansion.ActualValue;

					sbatch.DrawCentered(Textures.TexCircle, p1, diam, diam, Fraction.Color);
					sbatch.DrawCentered(Textures.TexCircle, p2, diam, diam, Fraction.Color);
					sbatch.DrawCentered(Textures.TexCircle, p3, diam, diam, Fraction.Color);
				}
			}
		}
		
		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			controller.Update(gameTime, istate);

			bool change = Rotation.CUpdate(gameTime);
			if (change) {_screen.LaserNetwork.SemiDirty = true; ChargeTime = 0; }

			CrosshairSize.Update(gameTime);

			UpdatePhysicBodies();
			UpdateHealth(gameTime);
			UpdateBoost(gameTime);
			UpdateNetwork(gameTime);
			UpdateCore(gameTime);
			UpdateDamage(gameTime);
			UpdateShield(gameTime);
			
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
			if (IsMouseDownOnThis(istate) && DebugSettings.Get("LooseCannon"))
			{
				var bckp = DebugSettings.Get("ImmortalCannons");
				DebugSettings.SetManual("ImmortalCannons", false);

				while (Fraction.Type != FractionType.ComputerFraction)
					TakeDamage(this.GDOwner().GetComputerFraction(), 1);

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
			if (CannonHealth.ActualValue < FULL_LASER_HEALTH || Fraction.IsNeutral)
			{
				CorePulse.Set(1);
			}
			else
			{
				CorePulse.Set(1 + FloatMath.Sin(gameTime.TotalElapsedSeconds * CORE_PULSE_FREQ) * CORE_PULSE);
			}
			CorePulse.Update(gameTime);

			if (CannonHealth.ActualValue < FULL_LASER_HEALTH || Fraction.IsNeutral || !LaserSource.LaserPowered)
			{
				if (LaserPulseTime > 0f) LaserPulseTime = FloatMath.LimitedDec(LaserPulseTime, gameTime.ElapsedSeconds, 0f);
			}
			else
			{
				LaserPulseTime += gameTime.ElapsedSeconds;
			}
			
			ChargeTime += gameTime.ElapsedSeconds;

			if (CannonHealth.ActualValue < 1) ChargeTime = 0;

			if (CannonHealth.ActualValue < 1)
			{
				SatelliteExpansion.Set(0);
			}
			else
			{
				SatelliteExpansion.Set(1);
			}

			SatelliteExpansion.Update(gameTime);

			if (SatelliteExpansion.ActualValue > 0) CannonHealth.SetActual(1);

			_coreRotation += gameTime.ElapsedSeconds * FloatMath.TAU * SHIELD_SATELLITE_SPEED;
		}

		private void UpdateNetwork(SAMTime gameTime)
		{
			bool active = CannonHealth.TargetValue >= FULL_LASER_HEALTH;

			LaserSource.SetState(active, Fraction, Rotation.ActualValue, ChargeTime > SHIELDLASER_CHARGE_COOLDOWN);

			if (!_muted && MainGame.Inst.Profile.EffectsEnabled)
			{
				if ( LaserSource.LaserPowered && !_soundeffect.IsPlaying) _soundeffect.Play();
				if (!LaserSource.LaserPowered &&  _soundeffect.IsPlaying) _soundeffect.Stop();
			}
		}

		private void UpdateDamage(SAMTime gameTime)
		{
			if (!LaserSource.LaserActive) return;

			bool hit = false;

			foreach (var ray in LaserSource.Lasers)
			{
				if (ray.Terminator != LaserRayTerminator.Target) continue;
				
				if (ray.TargetCannon == null) continue;

				if (ray.TargetCannon.Fraction == Fraction)
				{
					if (ray.TargetCannon == this) continue; // stop touching yourself

					hit = true;

					if (!LaserSource.LaserPowered) continue;
					ray.TargetCannon.ApplyShield(gameTime);
				}
			}

			if (!hit) ChargeTime = 0f;
		}
		
		public override void ApplyBoost()
		{
			if (Fraction.IsNeutral) return;

			CannonHealth.Inc(HEALTH_HIT_GEN);
			if (CannonHealth.Limit(0f, 1f) == 1)
			{
				AddOperation(new CannonBooster(1 / (BOOSTER_LIFETIME_MULTIPLIER * Fraction.LaserMultiplicator)));
			}
		}

		public override void ResetChargeAndBooster()
		{
			AbortAllOperations(o => o is CannonBooster);
		}

		public override void ForceResetBarrelCharge()
		{
			ChargeTime = 0f;
		}

		public override KIController CreateKIController(GDGameScreen screen, Fraction fraction)
		{
			return new ShieldProjectorKIController(screen, this, fraction);
		}

		public void RemoteUpdate(Fraction frac, float hp, byte boost, float chrg, float sendertime)
		{
			if (frac != Fraction) SetFraction(frac);

			ManualBoost = boost;

			var delta = GDOwner.LevelTime - sendertime;

			ChargeTime = chrg + delta;

			CannonHealth.Set(hp);

			var ups = delta / (1 / 30f);

			if (ups > 1)
			{
				var iups = FloatMath.Min(FloatMath.Round(ups), 10);
				var gt30 = new SAMTime((delta / iups) * GDOwner.GameSpeed, MonoSAMGame.CurrentTime.TotalElapsedSeconds);

				for (int i = 0; i < iups; i++)
				{
					CannonHealth.Update(gt30);

					if (CannonHealth.TargetValue < 1 && CannonHealth.TargetValue > MIN_REGEN_HEALTH && (LastAttackingLasersEnemy <= LastAttackingLasersFriends))
					{
						var bonus = START_HEALTH_REGEN + (END_HEALTH_REGEN - START_HEALTH_REGEN) * CannonHealth.TargetValue;

						bonus /= Scale;

						CannonHealth.Inc(bonus * gt30.ElapsedSeconds);
						CannonHealth.Limit(0f, 1f);
					}
				}
			}
		}

		public override void ApplyShield(SAMTime gameTime)
		{
			ShieldTime = 0; // no recursive shielding
		}

		protected override void CreateShieldPhysics()
		{
			// NO SHIELD
		}

		public override AbstractFractionController CreateNeutralController(GDGameScreen screen, Fraction fraction)
		{
			return new EmptyController(screen, this, fraction);
		}

		public override void SetFractionAndHealth(Fraction fraction, float hp)
		{
			SetFraction(fraction);
			CannonHealth.Set(1);
		}
	}
}
