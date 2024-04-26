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
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDLabel : HUDContainer
	{
		public override int Depth { get; }

		protected readonly HUDRawText internalText;

		public HUDAlignment TextAlignment
		{
			get { return internalText.Alignment; }
			set { internalText.Alignment = value; recalcText = true; }
		}

		private int _l10ntext = -1;
		public int L10NText
		{
			get { return _l10ntext; }
			set { if (_l10ntext != value) { _l10ntext = value; _text = ""; recalcText = true; } }
		}

		private string _text = "";
		public string Text
		{
			get { return _text; }
			set { if (_l10ntext != -1 || _text != value) { _l10ntext = -1; _text = value; recalcText = true; } }
		}

		public string DisplayText => _l10ntext >= 0 ? L10N.T(_l10ntext) : _text;

		public Color TextColor
		{
			get { return internalText.TextColor; }
			set { internalText.TextColor = value; }
		}

		public HUDBackgroundDefinition Background = HUDBackgroundDefinition.NONE;

		public SpriteFont Font
		{
			get { return internalText.Font; }
			set { internalText.Font = value; recalcText = true; }
		}

		private float _manualFontSize;
		public float FontSize
		{
			get { return internalText.FontSize; }
			set { internalText.FontSize = _manualFontSize = value; recalcText = true; }
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

		private FSize _sizeBuffer = FSize.MaxValue;
		private int _l10nLangBuffer = -1;

		public bool AutoSize = false;           // auto set size to innerlabel-size
		public bool AutoFontSizeShrink = false; // if text too long (in x) reduce font size enough for text to fit (never increase font size)

		public FSize InnerLabelSize => internalText.Size;

		private bool recalcText = false;

		public HUDLabel(int depth = 0)
		{
			Depth = depth;

			internalText = new HUDRawText{ Alignment = HUDAlignment.BOTTOMLEFT };
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			HUDRenderHelper.DrawAlphaBackground(sbatch, bounds, Background, Alpha);
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
			
			if (_sizeBuffer != Size)
			{
				_sizeBuffer = Size;
				recalcText = true;
			}

			if (recalcText)
			{
				_l10nLangBuffer = L10N.LANGUAGE;
				recalcText = false;

				if (MaxWidth == null)
				{
					internalText.Text = DisplayText;

					if (!AutoSize && AutoFontSizeShrink)
					{
						var w = FontRenderHelper.MeasureStringCached(internalText.Font, internalText.Text, _manualFontSize).Width;
						if (w > Width && Width>0)
						{
							internalText.FontSize = _manualFontSize * (Width / w);
						}
						else if (!FloatMath.EpsilonEquals(_manualFontSize, internalText.FontSize, 0.001f))
						{
							internalText.FontSize = _manualFontSize;
						}
					}
				}
				else
				{
					internalText.Text = string.Join(Environment.NewLine, FontRenderHelper.WrapLinesIntoWidth(DisplayText, Font, FontSize, MaxWidth.Value, WordWrap));
				}
			}

			if (AutoSize) Size = internalText.Size;
		}
	}
}
