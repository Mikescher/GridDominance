using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Font;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDRawText : HUDElement
	{
		public override int Depth { get; }

		private string _textCache = "";
		private float _fsizeCache = -1;

		private string _text = "";
		public string Text
		{
			get { return _text; }
			set { if (_text != value) { _text = value; InvalidatePosition(); } }
		}

		public float Alpha = 1f;

		public Color TextColor = Color.Magenta;

		private float _fontScale = 1f;
		private float _fontVOffset = 0f;
		private float _fontSize = 16;
		public float FontSize
		{
			get { return _fontSize; }
			set { if (_fontSize != value) {_fontSize = value; InvalidatePosition(); } }
		}

		public SAMFont Font;

		public HUDRawText(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Alignment == HUDAlignment.CENTER || Alignment == HUDAlignment.CENTERLEFT || Alignment == HUDAlignment.CENTERRIGHT)
			{
				sbatch.DrawString(
					Font, 
					_textCache, 
					new FPoint(bounds.Left, bounds.Top + _fontVOffset), 
					TextColor * Alpha, 
					0,
					FPoint.Zero, 
					_fontScale, 
					SpriteEffects.None, 
					0);
			}
			else
			{
				sbatch.DrawString(
					Font, 
					_textCache, 
					bounds.TopLeft,
					TextColor * Alpha, 
					0,
					FPoint.Zero, 
					_fontScale, 
					SpriteEffects.None, 
					0);
			}
		}

		public override void OnInitialize()
		{
			if (Font == null) Font = HUD.DefaultFont;
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			// NOP
		}

		protected override void OnBeforeRecalculatePosition()
		{
			base.OnBeforeRecalculatePosition();

			bool fschanged = false;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (_fsizeCache != _fontSize)
			{
				_fontScale = FontRenderHelper.GetFontScale(Font, FontSize);
				
				_fsizeCache = _fontSize;

				fschanged = true;
			}

			if (_textCache != _text || fschanged)
			{
				try
				{
					Size = (Font.MeasureString(_text) * _fontScale).ToFSize();
				}
				catch (Exception e)
				{
					
					SAMLog.Error("RAWTXT:Measure", $"Measure string failed for text: '{_text}' (prev: '{_textCache}')" + "\n" + e);
					return;
				}

				_textCache = _text;
			}

			_fontVOffset = FontRenderHelper.GetFontVCenterOffset(Font) * _fontScale;
		}

		protected override void DrawDebugHUDBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta, 1f);
			sbatch.DrawLine(BoundingRectangle.TopLeft,  BoundingRectangle.BottomRight, Color.Magenta, 1f);
			sbatch.DrawLine(BoundingRectangle.TopRight, BoundingRectangle.BottomLeft,  Color.Magenta, 1f);
		}
	}
}
