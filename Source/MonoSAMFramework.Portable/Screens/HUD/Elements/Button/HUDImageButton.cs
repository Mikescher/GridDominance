using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDImageButton : HUDButton
	{
		public override int Depth { get; }

		#region Properties

		public TextureRegion2D Image
		{
			get { return internalIcon.Image; }
			set { internalIcon.Image = value; InvalidatePosition(); }
		}

		private int _imagePadding = 8;
		public int ImagePadding
		{
			get { return _imagePadding; }
			set { _imagePadding = value; InvalidatePosition(); }
		}

		public Color Color = Color.Transparent;

		public Color ColorPressed = Color.Transparent;

		public HUDBackgroundType BackgroundType = HUDBackgroundType.Simple;

		public float BackgoundCornerSize = 16f;

		#endregion

		public delegate void ButtonEventHandler(HUDImageButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

		public ButtonEventHandler Click { set { ButtonClick += value; } }

		private readonly HUDImage internalIcon;

		public HUDImageButton(int depth = 0)
		{
			Depth = depth;

			internalIcon = new HUDImage
			{
				Alignment = HUDAlignment.TOPLEFT,
				ImageAlignment = HUDImageAlignment.SCALE_Y,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalIcon);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();
			
			var isize = internalIcon.CalculateRealBounds(new FRectangle(0, 0, Width - 2 * ImagePadding, Height - 2 * ImagePadding)).Size;

			internalIcon.RelativePosition = new FPoint(ImagePadding, ImagePadding);
			internalIcon.Size = new FSize(isize.Width, Height - 2 * ImagePadding);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			var btnColor = IsPointerDownOnElement ? ColorPressed : Color;
			
			SimpleRenderHelper.DrawHUDBackground(sbatch, BackgroundType, bounds, btnColor, BackgoundCornerSize);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			// NOP
		}

		protected override void OnPress(InputState istate)
		{
			ButtonClick?.Invoke(this, new HUDButtonEventArgs(HUDButtonEventType.SingleClick));
		}

		protected override void OnDoublePress(InputState istate)
		{
			ButtonDoubleClick?.Invoke(this, new HUDButtonEventArgs(HUDButtonEventType.DoubleClick));
		}

		protected override void OnTriplePress(InputState istate)
		{
			ButtonTripleClick?.Invoke(this, new HUDButtonEventArgs(HUDButtonEventType.TipleClick));
		}

		protected override void OnHold(InputState istate, float holdTime)
		{
			ButtonHold?.Invoke(this, new HUDButtonEventArgs(HUDButtonEventType.Hold));
		}
	}
}
