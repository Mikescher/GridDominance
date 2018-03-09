using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.OverworldScreen.HUD.SCCM
{
	class SCCMListScrollbar : HUDContainer
	{
		public override int Depth => 0;

		public SCCMListPresenter Presenter;

		public int ScrollPosition = 0;
		public int ScrollMax      = 0;
		public int ScrollPageSize = 1;

		private HUDImageButton _btn1;
		private HUDImageButton _btn2;

		public SCCMListScrollbar()
		{

		}

		public override void OnInitialize()
		{
			AddElement(_btn1 = new HUDImageButton
			{
				Alignment = HUDAlignment.TOPCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(Width, 64),

				Image = Textures.TexTriangle,
				ImageRotation = FloatMath.RAD_POS_000,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = (s, a) => DoScroll(-1),
			});

			AddElement(_btn2 = new HUDImageButton
			{
				Alignment = HUDAlignment.BOTTOMCENTER,
				RelativePosition = new FPoint(0, 0),
				Size = new FSize(Width, 64),

				Image = Textures.TexTriangle,
				ImageRotation = FloatMath.RAD_POS_180,
				ImagePadding = 4,

				BackgroundNormal = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonHUD, Color.Black, HUD.PixelWidth),
				BackgroundPressed = HUDBackgroundDefinition.CreateSimpleOutline(FlatColors.ButtonPressedHUD, Color.Black, HUD.PixelWidth),

				Click = (s, a) => DoScroll(+1),
			});
		}
		
		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => IsVisible;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => IsVisible;

		private void DoScroll(int delta)
		{
			Presenter?.Scroll(delta);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, FlatColors.Asbestos);
			SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, HUD.PixelWidth, Color.Black);

			if (ScrollMax > ScrollPageSize)
			{
				var yrange = Height - 64 - 64 - 32;
				var ypos = yrange * (ScrollPosition * 1f / (ScrollMax - ScrollPageSize));

				SimpleRenderHelper.DrawSimpleRect(sbatch, FRectangle.CreateByTopLeft(bounds.Left, bounds.Top+64+ypos, bounds.Width, 32), FlatColors.MidnightBlue);
			}
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (Presenter!=null && IsPointerDownOnElement && !_btn1.IsPointerDownOnElement && !_btn2.IsPointerDownOnElement)
			{
				var yrange = Height - 64 - 64 - 32;
				var scroll = (istate.HUDPointerPosition.Y - (Top+64+16)) / yrange;

				var offset = FloatMath.Round(Presenter.MaxOffset * scroll);

				Presenter.SetOffset(offset);
			}
		}
	}
}
