using System;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.RenderHelper;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDCharacterControl : HUDContainer
	{
		public override int Depth { get; }

		#region Properties

		private char _character = ' ';
		public char Character
		{
			get { return _character; }
			set { internalLabel.Text = (_character = value).ToString(); }
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

		private int _textPadding = 0;
		public int TextPadding
		{
			get { return _textPadding; }
			set { _textPadding = value; InvalidatePosition(); }
		}

		public Color BackgroundColor = Color.Transparent;
		public HUDBackgroundType BackgoundType = HUDBackgroundType.Simple;
		public float BackgoundCornerSize = 16f;

		public float BorderWidth = 0f;
		public Color BorderColor = Color.Transparent;

		#endregion

		private readonly HUDLabel internalLabel;

		public HUDCharacterControl(int depth = 0)
		{
			Depth = depth;

			internalLabel = new HUDLabel
			{
				Text = "",
				
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTER,
				RelativePosition = new FPoint(0, 0),
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();

			internalLabel.RelativePosition = new FPoint(0, 0);
			internalLabel.Size = new FSize(Width - 2 * TextPadding, Height - 2 * TextPadding);
			internalLabel.FontSize = Height - 2 * TextPadding;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawHUDBackground(sbatch, BackgoundType, bounds, BackgroundColor, BackgoundCornerSize);

			if (BorderWidth > 0f && BorderColor.A > 0)
				sbatch.DrawRectangle(bounds, BorderColor, BorderWidth);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			// NOP
		}
	}
}
