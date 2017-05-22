using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDToast : HUDContainer
	{
		public override int Depth => 9 * 1000 * 1000;

		#region Properties

		public string Text
		{
			get { return internalLabel.Text; }
			set { internalLabel.Text = value; innerSizeCache = FSize.Empty; }
		}
		
		public Color TextColor
		{
			get { return internalLabel.TextColor; }
			set { internalLabel.TextColor = value; }
		}

		public SpriteFont Font
		{
			get { return internalLabel.Font; }
			set { internalLabel.Font = value; innerSizeCache = FSize.Empty; }
		}

		public float FontSize
		{
			get { return internalLabel.FontSize; }
			set { internalLabel.FontSize = value; innerSizeCache = FSize.Empty; }
		}

		public float Alpha
		{
			get { return internalLabel.Alpha; }
			set { internalLabel.Alpha = value; }
		}

		public HUDWordWrap WordWrap
		{
			get { return internalLabel.WordWrap; }
			set { internalLabel.WordWrap = value; innerSizeCache = FSize.Empty; }
		}

		private FSize _textPadding = new FSize(48, 12);
		public FSize TextPadding
		{
			get { return _textPadding; }
			set { _textPadding = value; innerSizeCache = FSize.Empty; }
		}

		private float _maxWidth = 10000;
		public float MaxWidth
		{
			get { return _maxWidth; }
			set { _maxWidth = value; innerSizeCache = FSize.Empty; }
		}

		public Color ColorBackground = Color.White;

		public HUDBackgroundType BackgroundType = HUDBackgroundType.Rounded;
		public float BackgoundCornerSize = 16f;

		public bool CloseOnClick = true;

		#endregion
		
		private readonly HUDLabel internalLabel;
		private FSize innerSizeCache = FSize.Empty;

		private float _lifetime = 0;
		private readonly float _toastTime;

		public HUDToast(float lifetime)
		{
			_toastTime = lifetime;

			Alignment = HUDAlignment.CENTER;

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTER,
				FontSize = 96,
				BackgroundColor = Color.Transparent,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawAlphaHUDBackground(sbatch, BackgroundType, bounds, ColorBackground, BackgoundCornerSize, Alpha);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (internalLabel.InnerLabelSize != innerSizeCache)
			{
				innerSizeCache = internalLabel.InnerLabelSize;

				Size = innerSizeCache + TextPadding * 2;
				internalLabel.Size = innerSizeCache;

				internalLabel.MaxWidth = MaxWidth - TextPadding.Width * 2;
			}

			_lifetime += gameTime.ElapsedSeconds;

			var p = _lifetime / _toastTime;

			if (p < 0.15f)
			{
				var sp = (p - 0) / 0.15f;

				Alpha = sp;
			}
			else if (p < 0.85f)
			{
				var sp = (p - 0.15f) / 0.70f;

				Alpha = 1f;
			}
			else if (p < 1.00f)
			{
				var sp = (p - 0.85f) / 0.15f;

				Alpha = 1 - sp;
			}
			else
			{
				Remove();
			}

		}

		protected override bool OnPointerUp(FPoint relPositionPoint, InputState istate) => true;
		protected override bool OnPointerDown(FPoint relPositionPoint, InputState istate) => true;

		protected override void OnPointerClick(FPoint relPositionPoint, InputState istate)
		{
			if (CloseOnClick) Remove();
		}
	}
}
