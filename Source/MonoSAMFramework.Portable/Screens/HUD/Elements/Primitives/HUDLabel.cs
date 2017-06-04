using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;
using System.Collections.Generic;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDLabel : HUDContainer
	{
		public override int Depth { get; }

		protected readonly HUDRawText internalText;

		public HUDAlignment TextAlignment // TODO vertically the text is not correctly aligned - cause MeasureString includes strange whitespaces
		{
			get { return internalText.Alignment; }
			set { internalText.Alignment = value; recalcText = true; }
		}

		private int _l10nLangBuffer = -1;
		private int _l10ntext = -1;
		public int L10NText
		{
			get { return _l10ntext; }
			set { _l10ntext = value; _text = ""; recalcText = true; }
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _l10ntext = -1; _text = value; recalcText = true; }
		}

		public string DisplayText => _l10ntext >= 0 ? L10N.T(_l10ntext) : _text;

		public Color TextColor
		{
			get { return internalText.TextColor; }
			set { internalText.TextColor = value; }
		}

		public Color BackgroundColor = Color.Transparent;

		public SpriteFont Font
		{
			get { return internalText.Font; }
			set { internalText.Font = value; recalcText = true; }
		}

		public float FontSize
		{
			get { return internalText.FontSize; }
			set { internalText.FontSize = value; recalcText = true; }
		}

		public float Alpha
		{
			get { return internalText.Alpha; }
			set { internalText.Alpha = value; }
		}

		private float? _maxWidth = null; // if set Height is autom. adjusted
		public float? MaxWidth
		{
			get { return _maxWidth; }
			set { _maxWidth = value; recalcText = true; }
		}

		private HUDWordWrap _wordWrap = HUDWordWrap.WrapByCharacter;
		public HUDWordWrap WordWrap
		{
			get { return _wordWrap; }
			set { _wordWrap = value; recalcText = true; }
		}

		public FSize InnerLabelSize => internalText.Size;
		
		private bool recalcText = false;

		public HUDLabel(int depth = 0)
		{
			Depth = depth;

			internalText = new HUDRawText{ Alignment = HUDAlignment.BOTTOMLEFT };
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (BackgroundColor != Color.Transparent)
			{
				SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, BackgroundColor * Alpha);
			}
		}

		public override void OnInitialize()
		{
			AddElement(internalText);
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (_l10nLangBuffer != L10N.LANGUAGE)
			{
				_l10nLangBuffer = L10N.LANGUAGE;
				recalcText = true;
			}

			if (recalcText)
			{
				_l10nLangBuffer = L10N.LANGUAGE;
				recalcText = false;

				if (MaxWidth == null)
				{
					internalText.Text = DisplayText;
				}
				else
				{
					internalText.Text = string.Join(Environment.NewLine, FontRenderHelper.WrapLinesIntoWidth(DisplayText, Font, FontSize, MaxWidth.Value, WordWrap));
				}
			}
		}
	}
}
