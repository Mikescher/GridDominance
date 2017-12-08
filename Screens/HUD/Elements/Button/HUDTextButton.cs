﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Button
{
	public class HUDTextButton : HUDButton
	{
		public override int Depth { get; }

		#region Properties

		public int L10NText
		{
			get { return internalLabel.L10NText; }
			set { internalLabel.L10NText = value; InvalidatePosition(); }
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
		
		private int _textPadding = 0;
		public int TextPadding
		{
			get { return _textPadding; }
			set { _textPadding = value; InvalidatePosition(); }
		}

		public HUDBackgroundDefinition BackgroundNormal = HUDBackgroundDefinition.DUMMY;
		public HUDBackgroundDefinition BackgroundPressed = HUDBackgroundDefinition.DUMMY;

		public float BackgroundCornerSize = 16f;

		#endregion

		public delegate void ButtonEventHandler(HUDTextButton sender, HUDButtonEventArgs e);

		public event ButtonEventHandler ButtonClick;
		public event ButtonEventHandler ButtonDoubleClick;
		public event ButtonEventHandler ButtonTripleClick;
		public event ButtonEventHandler ButtonHold;

		public ButtonEventHandler Click { set { ButtonClick += value; } }

		private readonly HUDLabel internalLabel;

		public HUDTextButton(int depth = 0)
		{
			Depth = depth;

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.TOPLEFT,
				TextAlignment = HUDAlignment.CENTERLEFT,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();
			
			internalLabel.RelativePosition = new FPoint(TextPadding, 0);
			internalLabel.Size = new FSize(Width - 2*TextPadding, Height);
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
