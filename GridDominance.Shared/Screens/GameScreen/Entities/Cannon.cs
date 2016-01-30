using System;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using GridDominance.Shared.Framework;
using MonoGame.Extended.Shapes;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
    class Cannon : GDEntity
    {
	    private const float ROTATION_SPEED = FloatMath.TAU / 2; // 3.141 rad/sec

        private const float CANNON_SIZE = GameScreen.TILE_WIDTH;

        private readonly Sprite spriteBody;
        private readonly Sprite spriteBarrel;

        private readonly int gridPositionX;
        private readonly int gridPositionY;

	    private bool isMouseDragging = false;

        private float actualRotation = 0;			// radians
        private float targetRotation = 0;	// radians

	    private Vector2 center;
	    private CircleF innerBoundings;

        public Cannon(int gridX, int gridY)
        {
            gridPositionX = gridX;
            gridPositionY = gridY;

	        center = new Vector2(gridPositionX*CANNON_SIZE + CANNON_SIZE/2, gridPositionY*CANNON_SIZE + CANNON_SIZE/2);
			innerBoundings = new CircleF(center, CANNON_SIZE / 2);

			spriteBody = new Sprite(Textures.TexCannonBody)
            {
                Scale = Textures.DEFAULT_TEXTURE_SCALE,
                Position = center,
			};

            spriteBarrel = new Sprite(Textures.TexCannonBarrel)
            {
                Scale = Textures.DEFAULT_TEXTURE_SCALE,
                Position = center,
                Origin = new Vector2(-32, 32),
            };

        }

		public override void Update(GameTime gameTime, InputState state)
		{
			if (state.IsJustDown && innerBoundings.Contains(state.PointerPosition))
			{
				isMouseDragging = true;
			}
			else if (!state.IsDown)
			{
				isMouseDragging = false;
			}
			else if (isMouseDragging && state.IsDown && !innerBoundings.Contains(state.PointerPosition))
			{
				targetRotation = FloatMath.PositiveAtan2(state.PointerPosition.Y - center.Y, state.PointerPosition.X - center.X);
			}

			if (actualRotation != targetRotation)
			{
				var radSpeed = ROTATION_SPEED*gameTime.GetElapsedSeconds();
				var diff = FloatMath.DiffRadians(actualRotation, targetRotation);

				Console.WriteLine("target := {0}", targetRotation);
				Console.WriteLine("actual := {0}", actualRotation);
				Console.WriteLine("diff   := {0}", diff);
				Console.WriteLine("speed  := {0}", radSpeed);

				if (Math.Abs(diff) <= radSpeed)
				{
					Console.WriteLine("> set");
					actualRotation = targetRotation;
				}
				else
				{
					Console.WriteLine("> move");
					actualRotation = FloatMath.AddRads(actualRotation, -FloatMath.Sign(diff) * radSpeed);
				}
				Console.WriteLine();
			}

			spriteBody.Rotation = actualRotation;
			spriteBarrel.Rotation = actualRotation;
		}

		public override void Draw(SpriteBatch sbatch)
        {
            sbatch.Draw(spriteBarrel);
			sbatch.Draw(spriteBody);
        }
    }
}
