using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using GridDominance.Shared.Framework;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
    class Cannon : GDEntity
    {
        private const float CANNON_SIZE = GameScreen.TILE_WIDTH;

        private readonly Sprite spriteBody;
        private readonly Sprite spriteBarrel;

        private readonly int gridPositionX;
        private readonly int gridPositionY;

        private double rotation = 0; // radians
        private double targetRotation = 0; // radians

        public Cannon(GameScreen owner, int gridX, int gridY) : base(owner)
        {
            gridPositionX = gridX;
            gridPositionY = gridY;

            spriteBody = new Sprite(Textures.TexCannonBody)
            {
                Scale = Textures.DEFAULT_TEXTURE_SCALE,
                Position = new Vector2(gridPositionX * CANNON_SIZE + CANNON_SIZE / 2, gridPositionY * CANNON_SIZE + CANNON_SIZE / 2)
            };

            spriteBarrel = new Sprite(Textures.TexCannonBarrel)
            {
                Scale = Textures.DEFAULT_TEXTURE_SCALE,
                Position = new Vector2(gridPositionX * CANNON_SIZE + CANNON_SIZE / 2, gridPositionY * CANNON_SIZE + CANNON_SIZE / 2),
                Origin = new Vector2(192, 32),
            };
        }

        public override void Draw(GameTime gameTime)
        {
            Owner.EntityBatch.Draw(spriteBarrel);
            Owner.EntityBatch.Draw(spriteBody);
        }

        public override void Update(GameTime gameTime)
        {
            spriteBody.Rotation   += gameTime.GetElapsedSeconds() * FloatMath.ToRadians(90);
            spriteBarrel.Rotation += gameTime.GetElapsedSeconds() * FloatMath.ToRadians(90);
        }
    }
}
