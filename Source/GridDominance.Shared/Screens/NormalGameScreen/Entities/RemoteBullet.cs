using System;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.EntityOperations;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class RemoteBullet : BaseBullet
	{
		public enum RemoteBulletState { Normal = 0, Dying_Explosion = 1, Dying_ShrinkFast = 2, Dying_ShrinkSlow = 3, Dying_Fade = 4, Dying_Instant = 5, Dying_FadeSlow = 6 } // max=15

		public const int POST_DEATH_TRANSMITIONCOUNT = 8;
		public const float DEATH_PREDICTION_TOLERANCE = 0.1f;

		public FPoint LastRemotePosition = FPoint.Zero;
		public bool ClientPredictionMiss = false;

		public Vector2 BulletVelocity;
		
		public long LastUpdateBigSeq;
		
		public RemoteBulletState RemoteState     = RemoteBulletState.Normal;
		public RemoteBulletState PredictionState = RemoteBulletState.Normal;
		public float TimeOfPredictedDeath = 0f;
		public readonly GDGameScreen GDOwner;

		public override FPoint Position => BulletPosition;
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;

		public RemoteBullet(GDGameScreen scrn, FPoint pos, Vector2 velo, ushort id, float entityScale, Fraction frac, long seq) 
			: base(scrn, frac, entityScale)
		{
			BulletPosition = pos;
			LastRemotePosition = pos;
			BulletVelocity = velo;
			GDOwner = scrn;
			BulletID = id;
			LastUpdateBigSeq = seq;

			DrawingBoundingBox = new FSize(Scale * Bullet.BULLET_DIAMETER, Scale * Bullet.BULLET_DIAMETER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateCircle(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Scale * Bullet.BULLET_DIAMETER / 2), 1, ConvertUnits2.ToSimUnits(BulletPosition), BodyType.Dynamic, this);
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(BulletVelocity);
			PhysicsBody.CollidesWith = Category.All;
			PhysicsBody.Restitution = 1f;              // Bouncability, 1=bounce always elastic
			PhysicsBody.AngularDamping = 1f;           // Practically no angular rotation
			PhysicsBody.Friction = 0f;
			PhysicsBody.LinearDamping = 0f;            // no slowing down
			PhysicsBody.OnCollision += OnCollision;
			PhysicsBody.AngularVelocity = 0;
			//Body.Mass = Scale * Scale; // Weight dependent on size
		}

		private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			if (!Alive || IsDying || RemoteState != RemoteBulletState.Normal || PredictionState != RemoteBulletState.Normal) return false;

			#region RemoteBullet
			var otherBullet = fixtureB.UserData as RemoteBullet;
			if (otherBullet != null)
			{

				if (otherBullet.Fraction == Fraction) return true;
				if (!otherBullet.Alive) return false;

				if (otherBullet.Scale / Scale >= 2f && !otherBullet.Fraction.IsNeutral)
				{
					// split other
					otherBullet.PredictKill(RemoteBulletState.Dying_Instant);
					PredictKill(RemoteBulletState.Dying_Explosion);
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
				else if (Scale / otherBullet.Scale >= 2f && Fraction.IsNeutral)
				{
					// split me
					otherBullet.PredictKill(RemoteBulletState.Dying_Explosion);
					PredictKill(RemoteBulletState.Dying_Instant);
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
				else
				{
					otherBullet.PredictKill(RemoteBulletState.Dying_Explosion);
					PredictKill(RemoteBulletState.Dying_Explosion);
					MainGame.Inst.GDSound.PlayEffectCollision();
					return false;
				}
			}
			#endregion

			#region Cannon
			var otherCannon = fixtureB.UserData as Cannon;
			if (otherCannon != null)
			{
				if (otherCannon.Fraction == Fraction)
				{
					// stupid approximation to prevent source cannon coll (lifetime > 0.5s)
					if (Lifetime > 0.5f || fixtureB == otherCannon.PhysicsFixtureBase)
					{
						PredictKill(RemoteBulletState.Dying_ShrinkFast);
						otherCannon.ApplyBoost();
						MainGame.Inst.GDSound.PlayEffectBoost();
					}
				}
				else // if (otherCannon.Fraction != this.Fraction)
				{
					PredictKill(RemoteBulletState.Dying_Fade);
					otherCannon.TakeDamage(Fraction, Scale);
					MainGame.Inst.GDSound.PlayEffectHit();
				}

				return false;
			}
			#endregion

			#region VoidWall
			var otherVoidWall = fixtureB.UserData as VoidWall;
			if (otherVoidWall != null)
			{
				PredictKill(RemoteBulletState.Dying_Explosion);
				MainGame.Inst.GDSound.PlayEffectCollision();
				return false;
			}
			#endregion

			#region VoidCircle
			var otherVoidCircle = fixtureB.UserData as VoidCircle;
			if (otherVoidCircle != null)
			{
				PredictKill(RemoteBulletState.Dying_Explosion);
				MainGame.Inst.GDSound.PlayEffectCollision();
				return false;
			}
			#endregion

			#region GlassBlock
			var otherGlassBlock = fixtureB.UserData as GlassBlock;
			if (otherGlassBlock != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect();
				return true;
			}
			#endregion

			#region Portal
			var otherPortal = fixtureB.UserData as Portal;
			if (otherPortal != null)
			{
				var inPortal = otherPortal;

				Vector2 normal;
				FixedArray2<Vector2> t;
				contact.GetWorldManifold(out normal, out t);

				bool hit = FloatMath.DiffRadiansAbs(normal.ToAngle(), inPortal.Normal) < FloatMath.RAD_POS_001;

				if (!hit)
				{
					// back-side hit
					PredictKill(RemoteBulletState.Dying_Explosion);
					return false;
				}

				if (inPortal.Links.Count == 0)
				{
					// void portal
					PredictKill(RemoteBulletState.Dying_ShrinkFast);
					return false;
				}

				PredictKill(RemoteBulletState.Dying_ShrinkFast);
				return false;
			}
			#endregion

			#region MirrorBlock
			var otherMirrorBlock = fixtureB.UserData as MirrorBlock;
			if (otherMirrorBlock != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect(); //TODO evtl other sound?
				return true;
			}
			#endregion

			#region MirrorCircle
			var otherMirrorCircle = fixtureB.UserData as MirrorCircle;
			if (otherMirrorCircle != null)
			{
				MainGame.Inst.GDSound.PlayEffectReflect(); //TODO evtl other sound?
				return true;
			}
			#endregion

			#region RefractionMarker
			var otherRefractionMarker1 = fixtureB.UserData as MarkerRefractionEdge;
			if (otherRefractionMarker1 != null) return false;
			var otherRefractionMarker2 = fixtureB.UserData as MarkerRefractionCorner;
			if (otherRefractionMarker2 != null) return false;
			#endregion

			#region BorderMarker
			var otherBorderMarker = fixtureB.UserData as MarkerCollisionBorder;
			if (otherBorderMarker != null)
			{
				if (GDOwner.WrapMode == GameWrapMode.Reflect) return true;
				return false;
			}
			#endregion

			return false;
		}

		public override void OnRemove()
		{
			this.GDManager().PhysicsWorld.RemoveBody(PhysicsBody);
			if (GDOwner.RemoteBulletMapping[BulletID] == this) GDOwner.RemoteBulletMapping[BulletID] = null;
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			BulletPosition = ConvertUnits2.ToDisplayUnitsPoint(PhysicsBody.Position);
			BulletVelocity = ConvertUnits.ToDisplayUnits(PhysicsBody.LinearVelocity);

			if (Lifetime > MAXIMUM_LIFETIME) PredictKill(RemoteBulletState.Dying_FadeSlow);

			if (GDOwner.WrapMode == GameWrapMode.Donut && !GDOwner.MapFullBounds.Contains(BulletPosition))
			{
				DonutWrap();
			}

			this.GDOwner().LaserNetwork.SemiDirty = true;
		}

		private void DonutWrap()
		{
			BulletPosition = BulletPosition.ModuloToToSize(GDOwner.MapFullBounds.Size);

			PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
		}

		private void MakePredictionKillReal(RemoteBulletState state)
		{
			switch (state)
			{
				case RemoteBulletState.Dying_Explosion:
					Alive = false;
					break;

				case RemoteBulletState.Dying_ShrinkFast:
					AddEntityOperation(new BulletDelayedDieOperation(0.15f));
					break;

				case RemoteBulletState.Dying_ShrinkSlow:
					AddEntityOperation(new BulletDelayedDieOperation(0.35f));
					break;

				case RemoteBulletState.Dying_Fade:
					AddEntityOperation(new BulletDelayedDieOperation(0.05f));
					break;

				case RemoteBulletState.Dying_FadeSlow:
					AddEntityOperation(new BulletDelayedDieOperation(1f));
					break;

				case RemoteBulletState.Dying_Instant:
					Alive = false;
					break;
					
				case RemoteBulletState.Normal:
				default:
					SAMLog.Error("RBULL::EnumSwitch_MPKR", "Invalid value: " + state);
					break;
			}
		}

		private void PredictKill(RemoteBulletState prediction, bool killReal = false)
		{
			TimeOfPredictedDeath = GDOwner.LevelTime;
			PredictionState = prediction;

			switch (prediction)
			{
				case RemoteBulletState.Dying_Explosion:

					if (MainGame.Inst.Profile.EffectsEnabled)
					{
						for (int i = 0; i < 8; i++)
							Manager.AddEntity(new BulletSplitter(Owner, this, (FlatAlign8)i));
					}

					if (killReal)
						Alive = false;
					else
						BulletAlpha = 0f;

					break;

				case RemoteBulletState.Dying_ShrinkFast:
					AddEntityOperation(new BulletShrinkAndDieOperation(0.15f, killReal));
					break;

				case RemoteBulletState.Dying_ShrinkSlow:
					AddEntityOperation(new BulletShrinkAndDieOperation(0.35f, killReal));
					break;

				case RemoteBulletState.Dying_Fade:
					AddEntityOperation(new BulletFadeAndDieOperation(0.05f, killReal));
					break;

				case RemoteBulletState.Dying_FadeSlow:
					AddEntityOperation(new BulletFadeAndDieOperation(1f, killReal));
					break;

				case RemoteBulletState.Dying_Instant:

					if (killReal)
						Alive = false;
					else
						BulletAlpha = 0f;

					break;
					
				case RemoteBulletState.Normal:
				default:
					SAMLog.Error("RBULL::EnumSwitch_PK", "Invalid value: " + prediction);
					break;
			}

			BulletVelocity = Vector2.Zero;
			PhysicsBody.LinearVelocity = Vector2.Zero;
		}
		
		protected override void OnDraw(IBatchRenderer sbatch)
		{
			sbatch.DrawCentered(
				Textures.TexBullet, 
				BulletPosition, 
				Scale * BulletExtraScale * BULLET_DIAMETER,
				Scale * BulletExtraScale * BULLET_DIAMETER,
				Fraction.Color * BulletAlpha, BulletRotation);

#if DEBUG
			if (DebugSettings.Get("DebugMultiplayer"))
			{
				sbatch.DrawCircle(LastRemotePosition, Scale * BulletExtraScale * BULLET_DIAMETER / 2f, 6, ClientPredictionMiss ? Color.Crimson : Color.DeepSkyBlue, 2*Owner.PixelWidth);
				sbatch.DrawLine(LastRemotePosition, BulletPosition, Color.DeepSkyBlue, Owner.PixelWidth);
			}
#endif
		}

		public void RemoteUpdate(RemoteBulletState state, float px, float py, Vector2 veloc, Fraction fraction, float scale, long seq, float sendertime)
		{
			if (Math.Abs(Scale - scale) > 0.01f) SAMLog.Error("RBULL::CHANGE-SCALE", $"Remote changed scale from {Scale} to {scale} ");
			if (fraction != Fraction) SAMLog.Error("RBULL::CHANGE-FRAC", $"Remote changed fraction from {Fraction} to {fraction} ");
			
			Scale = scale;
			Fraction = fraction;

			var delta = GDOwner.LevelTime - sendertime;

			if (state == RemoteBulletState.Normal)
			{
				if (PredictionState == RemoteBulletState.Normal)
				{
					// all good

					BulletVelocity = veloc;
					var newpos = new FPoint(px, py) + BulletVelocity * delta * GDOwner.GameSpeed;

					ClientPredictionMiss = (newpos - BulletPosition).LengthSquared() > 1f;

					BulletPosition = newpos;

					LastRemotePosition = new FPoint(px, py);
					PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
					PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);
					RemoteState = state;
				}
				else
				{
					if (sendertime - DEATH_PREDICTION_TOLERANCE >= TimeOfPredictedDeath)
					{
						// prediction said we should be death - was wrong

						SAMLog.Debug("Revert prediction-miss: (Revive wrongfully killed bullet)"); // Well, I'm sorry ... I guess

						AbortAllOperations(o => o.Name == BulletShrinkAndDieOperation.NAME);
						AbortAllOperations(o => o.Name == BulletFadeAndDieOperation.NAME);
						BulletAlpha = 1f;
						BulletExtraScale = 1f;

						BulletVelocity = veloc;
						BulletPosition = new FPoint(px, py) + BulletVelocity * delta * GDOwner.GameSpeed;

						ClientPredictionMiss = true;

						LastRemotePosition = new FPoint(px, py);
						PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
						PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);
						RemoteState = state;
						PredictionState = state;
					}
					else
					{
						// we will (predictably) die in the future
					}
				}

			}
			else
			{
				if (PredictionState != RemoteBulletState.Normal)
				{
					if (state != PredictionState)
					{
						// Prediction was correct but death cause was wrong (wtf??)

						AbortAllOperations(o => o.Name == BulletShrinkAndDieOperation.NAME);
						AbortAllOperations(o => o.Name == BulletFadeAndDieOperation.NAME);
						BulletAlpha = 1f;
						BulletExtraScale = 1f;


						BulletPosition = new FPoint(px, py);
						BulletVelocity = veloc;

						LastRemotePosition = BulletPosition;
						PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
						PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);
						RemoteState = state;
						PredictionState = state;

						PredictKill(state, true);
					}
					else
					{
						// Prediction was right (lucky path)

						BulletPosition = new FPoint(px, py);
						BulletVelocity = veloc;

						LastRemotePosition = BulletPosition;
						PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
						PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);
						RemoteState = state;

						MakePredictionKillReal(state);
					}

				}
				else
				{
					// dead but no death prediction

					BulletPosition = new FPoint(px, py);
					BulletVelocity = veloc;

					LastRemotePosition = BulletPosition;
					PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);
					PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);
					RemoteState = state;
					PredictionState = state;

					PredictKill(state, true);

					MainGame.Inst.GDSound.PlayEffectCollision();
				}

				if (GDOwner.RemoteBulletMapping[BulletID] == this) GDOwner.RemoteBulletMapping[BulletID] = null;
			}

			LastUpdateBigSeq = seq;
		}
	}
}
