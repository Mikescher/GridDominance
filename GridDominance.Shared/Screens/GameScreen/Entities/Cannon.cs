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
		
		private const float CANNON_DIAMETER = 96;

		private readonly Sprite spriteBody;
        private readonly Sprite spriteBarrel;

	    private bool isMouseDragging = false;

        private float actualRotation = 0;	// radians
        private float targetRotation = 0;	// radians

	    private Vector2 center;
	    private CircleF innerBoundings;

        public Cannon(int posX, float posY)
        {
	        center = new Vector2(posX, posY);
			innerBoundings = new CircleF(center, CANNON_DIAMETER/2);

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

		public override void Update(GameTime gameTime, InputState istate)
		{
			UpdateRotation(gameTime, istate);
		}

	    private void UpdateRotation(GameTime gameTime, InputState istate)
	    {
		    if (istate.IsJustDown && innerBoundings.Contains(istate.PointerPosition))
		    {
			    isMouseDragging = true;
		    }
		    else if (!istate.IsDown)
		    {
			    isMouseDragging = false;
		    }
		    else if (isMouseDragging && istate.IsDown && !innerBoundings.Contains(istate.PointerPosition))
		    {
			    targetRotation = FloatMath.PositiveAtan2(istate.PointerPosition.Y - center.Y, istate.PointerPosition.X - center.X);
		    }

		    // ReSharper disable once CompareOfFloatsByEqualityOperator
		    if (actualRotation != targetRotation)
		    {
			    var radSpeed = ROTATION_SPEED*gameTime.GetElapsedSeconds();
			    var diff = FloatMath.DiffRadians(actualRotation, targetRotation);

			    actualRotation = Math.Abs(diff) <= radSpeed ? targetRotation : FloatMath.AddRads(actualRotation, -FloatMath.Sign(diff)*radSpeed);
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
