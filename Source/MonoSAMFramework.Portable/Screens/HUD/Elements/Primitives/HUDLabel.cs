using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
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
			set { internalText.Alignment = value; recalcWordWrap = true; }
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; recalcWordWrap = true; }
		}

		public Color TextColor
		{
			get { return internalText.TextColor; }
			set { internalText.TextColor = value; }
		}

		public Color BackgroundColor = Color.Transparent;

		public SpriteFont Font
		{
			get { return internalText.Font; }
			set { internalText.Font = value; recalcWordWrap = true; }
		}

		public float FontSize
		{
			get { return internalText.FontSize; }
			set { internalText.FontSize = value; recalcWordWrap = true; }
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
			set { _maxWidth = value; recalcWordWrap = true; }
		}

		public FSize InnerLabelSize => internalText.Size;
		
		private bool recalcWordWrap = false;

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
			if (recalcWordWrap)
			{
				recalcWordWrap = false;

				if (MaxWidth == null)
				{
					internalText.Text = _text;
				}
				else
				{
					var sz = FontRenderHelper.MeasureStringCached(Font, _text, FontSize);

					if (sz.X < MaxWidth.Value)
					{
						internalText.Text = _text;
					}
					else
					{
						List<string> lines = new List<string>();

						var remText = _text;
						while (remText.Length > 0)
						{
							var line = "";
							while (remText.Length > 0 && remText[0] != '\n' && FontRenderHelper.MeasureStringUncached(Font, line, FontSize).X < MaxWidth.Value)
							{
								var chr = remText[0];

								if (chr != '\r' && chr != '\n') line += remText[0];
								remText = remText.Substring(1);
								if (chr == '\n') break;
							}
							lines.Add(line.Trim());
						}

						internalText.Text = string.Join(Environment.NewLine, lines);
					}
				}
			}
		}
	}
}
