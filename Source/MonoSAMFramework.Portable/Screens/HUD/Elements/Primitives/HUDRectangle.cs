using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDRectangle : HUDElement
	{
		public override int Depth { get; }

		public Color Color = Color.Transparent;

		public HUDRectangle(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Color == Color.Transparent) return;

			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, Color);
		}

		public override void OnInitialize()
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			//
		}
	}
}
