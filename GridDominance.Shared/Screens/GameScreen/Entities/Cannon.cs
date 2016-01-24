using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
    class Cannon : GDEntity
    {
        private readonly Sprite spriteBody;

        private readonly int gridPositionX;
        private readonly int gridPositionY;

        public Cannon(GameScreen owner, int gridX, int gridY) : base(owner)
        {
            gridPositionX = gridX;
            gridPositionY = gridY;

            spriteBody = new Sprite(Textures.TexDebugCannonBody)
            {
                Position = new Vector2(gridX*100 + 50, gridY*100 + 50),
                Scale = new Vector2(100f / Textures.TexDebugCannonBody.Width, 100f / Textures.TexDebugCannonBody.Height)
            };
        }

        public override void Draw(GameTime gameTime)
        {
            Owner.EntityBatch.Draw(spriteBody);
        }

        public override void Update(GameTime gameTime)
        {
            spriteBody.Rotation += gameTime.GetElapsedSeconds() * FloatMath.ToRadians(90);
        }
    }
}
