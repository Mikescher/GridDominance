using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMListElementPlayable : SCCMListElement
	{
		public SCCMListElementPlayable(SCCMLevelData d)
		{

		}

		public override void OnInitialize()
		{

		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, FlatColors.Amethyst);
			SimpleRenderHelper.DrawRoundedRectOutline(sbatch, bounds, Color.Black, 8, HUD.PixelWidth);
		}
	}
}
