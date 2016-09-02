using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.GameMath.FloatClasses;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Container
{
	public abstract class HUDRoundedPanel : HUDContainer
	{
		public Color Background = Color.White;

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, Background);
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			//
		}
	}
}
