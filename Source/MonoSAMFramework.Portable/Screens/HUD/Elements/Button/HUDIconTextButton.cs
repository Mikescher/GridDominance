using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDIconTextButton : HUDButton
	{
		public override int Depth { get; }

		#region Properties

		public TextureRegion2D Icon
		{
			get { return internalIcon.Image; }
			set { internalIcon.Image = value; InvalidatePosition(); }
		}

		public string Text
		{
			get { return internalLabel.Text; }
			set { internalLabel.Text = value; InvalidatePosition(); }
		}

		public HUDAlignment TextAlignment
		{
			get { return internalLabel.TextAlignment; }
			set { internalLabel.TextAlignment = value; }
		}

		public Color TextColor
		{
			get { return internalLabel.TextColor; }
			set { internalLabel.TextColor = value; }
		}
		
		public SpriteFont Font
		{
			get { return internalLabel.Font; }
			set { internalLabel.Font = value; }
		}

		public float FontSize
		{
			get { return internalLabel.FontSize; }
			set { internalLabel.FontSize = value; InvalidatePosition(); }
		}

		private int _iconPadding = 8;
		public int IconPadding
		{
			get { return _iconPadding; }
			set { _iconPadding = value; InvalidatePosition(); }
		}

		private int _textPadding = 0;
		public int TextPadding
		{
			get { return _textPadding; }
			set { _textPadding = value; InvalidatePosition(); }
		}

		public Color Color = Color.Transparent;

		public Color ColorPressed = Color.Transparent;

		public HUDButtonBackground BackgoundType = HUDButtonBackground.Simple;

		public float BackgoundCornerScale = 1f;

		#endregion

		public delegate void ButtonEventHandler(HUDIconTextButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

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
			var btnColor = IsPointerDownOnElement ? ColorPressed : Color;

			if (btnColor == Color.Transparent) return;

			switch (BackgoundType)
			{
				case HUDButtonBackground.None:
					// Do nothing
					break;
				case HUDButtonBackground.Simple:
					SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, btnColor);
					break;
				case HUDButtonBackground.Rounded:
					SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, btnColor, BackgoundCornerScale);
					break;
				case HUDButtonBackground.RoundedBlur:
					FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, btnColor, BackgoundCornerScale);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(GameTime gameTime, InputState istate)
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
