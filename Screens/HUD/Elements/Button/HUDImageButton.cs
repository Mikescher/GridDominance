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

		public HUDImageAlignment ImageAlignment
		{
			get => internalIcon.ImageAlignment;
			set => internalIcon.ImageAlignment = value;
		}

		public float ImageRotationSpeed
		{
			get => internalIcon.RotationSpeed;
			set => internalIcon.RotationSpeed = value;
		}

		public float ImageRotation
		{
			get => internalIcon.Rotation;
			set => internalIcon.Rotation = value;
		}

		public Color ImageColor
		{
			get => internalIcon.Color;
			set => internalIcon.Color = value;
		}

		public HUDBackgroundDefinition BackgroundNormal = HUDBackgroundDefinition.DUMMY;
		public HUDBackgroundDefinition BackgroundPressed = HUDBackgroundDefinition.DUMMY;

		#endregion

		public delegate void ButtonEventHandler(HUDImageButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

		public ButtonEventHandler Click { set => ButtonClick += value; }

		private readonly HUDImage internalIcon;

		public HUDImageButton(int depth = 0)
		{
			Depth = depth;

			internalIcon = new HUDImage
			{
				Alignment = HUDAlignment.CENTER,
				RelativePosition = FPoint.Zero,
				ImageAlignment = HUDImageAlignment.UNDERSCALE_CENTER,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalIcon);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();
			
			internalIcon.RelativePosition = FPoint.Zero;
			internalIcon.Alignment = HUDAlignment.CENTER;

			internalIcon.Size = new FSize(Width - 2 * ImagePadding, Height - 2 * ImagePadding);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (IsPointerDownOnElement)
				HUDRenderHelper.DrawBackground(sbatch, bounds, BackgroundPressed);
			else
				HUDRenderHelper.DrawBackground(sbatch, bounds, BackgroundNormal);
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
