using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDIconTextButton : HUDButton
	{
		public override int Depth { get; }

		#region Properties

		public TextureRegion2D Icon
		{
			get { return internalIcon.Image; }
			set { if (internalIcon.Image != value) { internalIcon.Image = value; InvalidatePosition(); } }
		}

		public float IconRotation
		{
			get => internalIcon.Rotation;
			set => internalIcon.Rotation = value;
		}

		public float IconRotationSpeed
		{
			get => internalIcon.RotationSpeed;
			set => internalIcon.RotationSpeed = value;
		}

		public int L10NText
		{
			get { return internalLabel.L10NText; }
			set { if (internalLabel.L10NText != value) { internalLabel.L10NText = value; InvalidatePosition(); } }
		}

		public string Text
		{
			get { return internalLabel.Text; }
			set { if (internalLabel.L10NText != -1 ||internalLabel.Text != value) { internalLabel.Text = value; InvalidatePosition(); } }
		}

		public HUDAlignment TextAlignment
		{
			get { return internalLabel.TextAlignment; }
			set { internalLabel.TextAlignment = value; }
		}

		public Color TextColor
		{
			get => internalLabel.TextColor;
			set => internalLabel.TextColor = value;
		}
		
		public SpriteFont Font
		{
			get => internalLabel.Font;
			set => internalLabel.Font = value;
		}

		public float FontSize
		{
			get { return internalLabel.FontSize; }
			set { if (internalLabel.FontSize != value) { internalLabel.FontSize = value; InvalidatePosition(); } }
		}

		private int _iconPadding = 8;
		public int IconPadding
		{
			get { return _iconPadding; }
			set { if (_iconPadding != value) { _iconPadding = value; InvalidatePosition(); } }
		}

		private int _textPadding = 0;
		public int TextPadding
		{
			get { return _textPadding; }
			set { if (_textPadding != value) { _textPadding = value; InvalidatePosition(); } }
		}

		public HUDBackgroundDefinition BackgroundNormal  = HUDBackgroundDefinition.DUMMY;
		public HUDBackgroundDefinition BackgroundPressed = HUDBackgroundDefinition.DUMMY;

		#endregion

		public delegate void ButtonEventHandler(HUDIconTextButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

		public ButtonEventHandler Click { set { ButtonClick += value; } }

		private readonly HUDImage internalIcon;
		private readonly HUDLabel internalLabel;

		public HUDIconTextButton(int depth = 0)
		{
			Depth = depth;

			internalIcon = new HUDImage
			{
				Alignment = HUDAlignment.TOPLEFT,
				ImageAlignment = HUDImageAlignment.SCALE_Y,
			};

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				TextAlignment = HUDAlignment.CENTERLEFT,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalIcon);
			AddElement(internalLabel);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();
			
			var isize = internalIcon.CalculateRealBounds(new FRectangle(0, 0, Width - 2 * IconPadding - TextPadding, Height - 2 * IconPadding)).Size;

			internalIcon.RelativePosition = new FPoint(IconPadding, IconPadding);
			internalIcon.Size = new FSize(isize.Width, Height - 2 * IconPadding);

			var labelX = 2 * IconPadding + isize.Width + TextPadding;
			internalLabel.RelativePosition = new FPoint(labelX, 0);
			internalLabel.Size = new FSize(Width - labelX, Height);
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
