using System;
using FarseerPhysics;
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
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class RemoteBullet : BaseBullet
	{
		public enum RemoteBulletState { Normal = 0, Dying_Explosion = 1, Dying_ShrinkFast = 2, Dying_ShrinkSlow = 3, Dying_Fade = 4, Dying_Instant = 5 } // max=15

		public const int POST_DEATH_TRANSMITIONCOUNT = 8;
		
		public FPoint LastRemotePosition = FPoint.Zero;
		
		public Vector2 BulletVelocity;
		
		public long LastUpdateBigSeq;
		
		public RemoteBulletState RemoteState = RemoteBulletState.Normal;
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
				sbatch.DrawCircle(LastRemotePosition, Scale * BulletExtraScale * BULLET_DIAMETER / 2f, 6, Color.DeepSkyBlue, 2*Owner.PixelWidth);
				sbatch.DrawLine(LastRemotePosition, BulletPosition, Color.DeepSkyBlue, Owner.PixelWidth);
			}

#endif
		}

		public void RemoteUpdate(RemoteBulletState state, float px, float py, Vector2 veloc, Fraction fraction, float scale, long seq)
		{
			if (Math.Abs(Scale - scale) > 0.01f) SAMLog.Error("REMBULL-CHANGE-SCALE", $"Remote changed scale from {Scale} to {scale} ");
			if (fraction != Fraction) SAMLog.Error("REMBULL-CHANGE-FRAC", $"Remote changed fraction from {Fraction} to {fraction} ");
			
			Scale = scale;
			Fraction = fraction;

			BulletPosition = new FPoint(px, py);
			LastRemotePosition = BulletPosition;
			PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);

			BulletVelocity = veloc;
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);

			RemoteState = state;

			LastUpdateBigSeq = seq;
		}

		public void RemoteKill(RemoteBulletState state)
		{
			RemoteState = state;
			
			switch (state)
			{
				case RemoteBulletState.Dying_Explosion:
					if (MainGame.Inst.Profile.EffectsEnabled)
					{
						for (int i = 0; i < 8; i++)
							Manager.AddEntity(new BulletSplitter(Owner, this, (FlatAlign8)i));
					}
					Alive = false;
					break;
				case RemoteBulletState.Dying_ShrinkFast:
					AddEntityOperation(new BulletShrinkAndDieOperation(0.15f));
					break;
				case RemoteBulletState.Dying_ShrinkSlow:
					AddEntityOperation(new BulletShrinkAndDieOperation(0.35f));
					break;
				case RemoteBulletState.Dying_Fade:
					AddEntityOperation(new BulletFadeAndDieOperation(0.05f));
					break;
				case RemoteBulletState.Dying_Instant:
					Alive = false;
					break;
			}

			if (GDOwner.RemoteBulletMapping[BulletID] == this) GDOwner.RemoteBulletMapping[BulletID] = null;
		}
	}
}
