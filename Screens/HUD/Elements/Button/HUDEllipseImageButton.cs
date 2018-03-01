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
	public class HUDEllipseImageButton : HUDButton
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
		
		protected FSize OverrideEllipseSize = FSize.Empty;

		public Color BackgroundNormal = Color.MidnightBlue;
		public Color BackgroundPressed = Color.Magenta;

		#endregion

		public delegate void ButtonEventHandler(HUDEllipseImageButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

		public ButtonEventHandler Click { set => ButtonClick += value; }

		private readonly HUDImage internalIcon;

		public HUDEllipseImageButton(int depth = 0)
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
		
		protected override bool IsCursorOnButton(InputState istate)
		{
			var ellipseSize = Size;
			if (!OverrideEllipseSize.IsEmpty) ellipseSize = OverrideEllipseSize;


			var relativePoint = BoundingRectangle.Center - istate.HUDPointerPosition;


			if (ellipseSize.IsQuadratic)
			{
				return relativePoint.LengthSquared() <= (ellipseSize.Width / 2) * (ellipseSize.Width / 2);
			}
			else
			{
				// http://math.stackexchange.com/a/76463/126706

				var a = (relativePoint.X * relativePoint.X) / (ellipseSize.Width * ellipseSize.Width);
				var b = (relativePoint.Y * relativePoint.Y) / (ellipseSize.Height * ellipseSize.Height);

				return a + b <= 1;
			}
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (OverrideEllipseSize.IsEmpty)
			{
				if (IsPointerDownOnElement)
					sbatch.DrawStretched(StaticTextures.MonoCircle, bounds, BackgroundPressed);
				else
					sbatch.DrawStretched(StaticTextures.MonoCircle, bounds, BackgroundNormal);
			}
			else
			{
				if (IsPointerDownOnElement)
					sbatch.DrawStretched(StaticTextures.MonoCircle, FRectangle.CreateByCenter(bounds.Center, OverrideEllipseSize), BackgroundPressed);
				else
					sbatch.DrawStretched(StaticTextures.MonoCircle, FRectangle.CreateByCenter(bounds.Center, OverrideEllipseSize), BackgroundNormal);
			}
		}
		
		protected override void DrawDebugHUDBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta);

			if (OverrideEllipseSize.IsEmpty)
			{
				sbatch.DrawEllipse(BoundingRectangle, 90, Color.Magenta, 2f);
			}
			else
			{
				sbatch.DrawEllipse(BoundingRectangle.AsResized(OverrideEllipseSize), 90, Color.Magenta, 2f);
			}
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
