using System;
using System.Collections.Generic;
using System.Text;
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
		private readonly Sprite spriteBullet;

		private Cannon collisionExcluder;

		private Vector2 position;
		private Vector2 velocity;

		public Bullet(GameScreen scrn, Cannon shooter, Vector2 pos, Vector2 velo)
			: base(scrn)
		{
			position = pos;
			velocity = velo;

			spriteBullet = new Sprite(Textures.TexBullet)
			{
				Scale = Textures.DEFAULT_TEXTURE_SCALE,
				Position = position,
			};

			collisionExcluder = shooter;

		}

		public override void Update(GameTime gameTime, InputState istate)
		{
			position += gameTime.GetElapsedSeconds() * velocity;


			spriteBullet.Position = position;
		}

		public override void Draw(SpriteBatch sbatch)
		{
			sbatch.Draw(spriteBullet);
		}
	}
}
