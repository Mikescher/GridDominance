using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
	class Bullet : GDEntity
	{
		private const float BULLET_DIAMETER = 25;

		private Sprite spriteBullet;

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
			spriteBullet = new Sprite(Textures.TexBullet)
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

		public override void Update(GameTime gameTime, InputState istate)
		{
			spriteBullet.Position = ConvertUnits.ToDisplayUnits(body.Position);
			spriteBullet.Rotation = body.Rotation;

			if (!Manager.BoundingBox.Contains(spriteBullet.Position)) Remove();
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(spriteBullet);
		}
	}
}
