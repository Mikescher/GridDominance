using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
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

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => IsVisible;
	}
}
