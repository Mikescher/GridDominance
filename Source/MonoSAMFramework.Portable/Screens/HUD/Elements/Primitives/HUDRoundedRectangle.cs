using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDRoundedRectangle : HUDElement
	{
		public override int Depth { get; }

		protected override bool isClickable() => false;

		public Color Color = Color.Transparent;
		public bool RoundCornerTL = true;
		public bool RoundCornerTR = true;
		public bool RoundCornerBL = true;
		public bool RoundCornerBR = true;

		public HUDRoundedRectangle(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Color == Color.Transparent) return;

			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, Color, RoundCornerTL, RoundCornerTR, RoundCornerBL, RoundCornerBR);
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
