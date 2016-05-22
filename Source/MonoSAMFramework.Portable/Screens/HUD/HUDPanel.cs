using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD
{
	public abstract class HUDPanel : HUDElement
	{
		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoDraw(SpriteBatch sbatch, Rectangle bounds)
		{
			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, Color.Magenta);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			//
		}
	}
}
