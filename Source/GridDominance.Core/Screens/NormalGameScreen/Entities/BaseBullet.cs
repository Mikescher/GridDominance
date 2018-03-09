using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public abstract class BaseBullet : GameEntity
	{
		public const float BULLET_DIAMETER = 25;
		public const float MAXIMUM_LIFETIME = 25;

		public ushort BulletID;
		
		public Fraction Fraction;
		public float BulletAlpha = 1f;
		public float BulletExtraScale = 1f;
		public FPoint BulletPosition;
		public float Scale;
		public float BulletRotation = 0f;

		public Body PhysicsBody;
		
		public bool IsDying;
		
		protected BaseBullet(GDGameScreen scrn, Fraction f, float s) : base(scrn, GDConstants.ORDER_GAME_BULLETS)
		{
			Fraction = f;
			Scale = s;
		}
	}
}
