using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class RemoteBullet : GameEntity
	{
		public enum RemoteBulletState { Normal = 0, Dying_Explosion = 1, Dying_Shrink = 2, Dying_Fade = 3, Dead = 4 }

		public Fraction Fraction;

		public FPoint BulletPosition;
		public Vector2 BulletVelocity;
		public float BulletRotation = 0f;
		public float BulletAlpha = 1f;
		public float BulletExtraScale = 1f;

		public long LastUpdateBigSeq;
		
		public readonly ushort BulletID;
		public RemoteBulletState RemoteState = RemoteBulletState.Normal;
		public Body PhysicsBody;
		public float Scale;
		public readonly GDGameScreen GDOwner;

		public override FPoint Position => BulletPosition;
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Fraction.Color;

		public RemoteBullet(GDGameScreen scrn, FPoint pos, Vector2 velo, ushort id, float entityScale, Fraction frac, long seq) 
			: base(scrn, GDConstants.ORDER_GAME_BULLETS)
		{
			BulletPosition = pos;
			BulletVelocity = velo;
			Fraction = frac;
			Scale = entityScale;
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
				Scale * BulletExtraScale * Bullet.BULLET_DIAMETER,
				Scale * BulletExtraScale * Bullet.BULLET_DIAMETER,
				Fraction.Color * BulletAlpha, BulletRotation);
		}

		public void RemoteUpdate(RemoteBulletState state, float px, float py, Vector2 veloc, Fraction fraction, float scale, long seq)
		{
			if (Math.Abs(Scale - scale) > 0.01f) SAMLog.Error("REMBULL", $"Remote changed scale from {Scale} to {scale} ");
			if (fraction != Fraction) SAMLog.Error("REMBULL", $"Remote changed fraction from {Fraction} to {fraction} ");
			
			Scale = scale;
			Fraction = fraction;

			BulletPosition = new FPoint(px, py);
			PhysicsBody.Position = ConvertUnits2.ToSimUnits(BulletPosition);

			BulletVelocity = veloc;
			PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(veloc);

			RemoteState = state;

			LastUpdateBigSeq = seq;
		}
	}
}
