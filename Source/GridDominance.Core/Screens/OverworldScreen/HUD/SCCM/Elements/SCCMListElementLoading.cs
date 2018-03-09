using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM.Elements
{
	class SCCMListElementLoading : SCCMListElement
	{
		public SCCMListElementLoading()
		{

		}

		public override void OnInitialize()
		{
			AddElement(new HUDLambdaLabel
			{
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(222, Height),
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTERLEFT,

				FontSize = Height - 10,
				Font = Textures.HUDFontBold,
				TextColor = Color.White,

				Lambda = () => L10N.T(L10NImpl.STR_SCCM_LOADING) + new string('.', ((int)(MonoSAMGame.CurrentTime.TotalElapsedSeconds*2))%4),
			});
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Silver);
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);
		}
	}
}
