using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.GameScreen.EntityOperations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
	class Bullet : GDEntity
	{
		private const float BULLET_DIAMETER = 25;
		private const float MAXIMUM_LIEFTIME = 25;

		public Sprite SpriteBullet;

		private Cannon collisionExcluder;

		private readonly Vector2 initial_position;
		private readonly Vector2 initial_velocity;

		private Body body;

		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo)
			: base(scrn)
		{
			initial_position = pos;
			initial_velocity = velo;

			collisionExcluder = shooter;
		}

		public override void OnInitialize()
		{
			SpriteBullet = new Sprite(Textures.TexBullet)
			{
				Scale = Textures.DEFAULT_TEXTURE_SCALE,
				Position = initial_position,
			};

			body = BodyFactory.CreateCircle(Manager.PhysicsWorld, ConvertUnits.ToSimUnits(BULLET_DIAMETER /2), 1, ConvertUnits.ToSimUnits(initial_position), BodyType.Dynamic, this);
			body.LinearVelocity = ConvertUnits.ToSimUnits(initial_velocity);
			body.CollidesWith = Category.All;
			body.IsBullet = true;
			body.Restitution = 0.95f;
			body.AngularDamping = 0.5f;
			body.Friction = 0.2f;
			body.LinearDamping = 0f;
		}

		public override void OnRemove()
		{
			Manager.PhysicsWorld.RemoveBody(body);
		}

		public override void OnUpdate(GameTime gameTime, InputState istate)
		{
			SpriteBullet.Position = ConvertUnits.ToDisplayUnits(body.Position);
			SpriteBullet.Rotation = body.Rotation;

			if (Lifetime > MAXIMUM_LIEFTIME) AddEntitOperation(new BulletFadeAndDieOperation());

			if (!Manager.BoundingBox.Contains(SpriteBullet.Position)) Remove();
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(SpriteBullet);
		}
	}
}
