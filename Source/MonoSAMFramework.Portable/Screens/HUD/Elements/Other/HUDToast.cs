using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Font;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDToast : HUDContainer
	{
		public const float MOVEMENT_DELTA = 256f;
		public const float PAD_BOTTOM = 16f;
		public const float PAD_VERT   = 12f;

		public override int Depth => 9 * 1000 * 1000;

		#region Properties

		public string Text
		{
			get { return internalLabel.Text; }
			set { if (internalLabel.Text != value) { internalLabel.Text = value; innerSizeCache = FSize.Empty; } }
		}
		
		public Color TextColor
		{
			get => internalLabel.TextColor;
			set => internalLabel.TextColor = value;
		}

		public SAMFont Font
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
			get => internalLabel.Alpha;
			set => internalLabel.Alpha = value;
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

		public HUDBackgroundDefinition Background = HUDBackgroundDefinition.DUMMY;

		public bool CloseOnClick = true;

		#endregion
		
		private readonly HUDLabel internalLabel;
		private FSize innerSizeCache = FSize.Empty;

		private float _lifetime = 0;
		private float _toastTime;

		public readonly DeltaLimitedFloat PositionY;

		public readonly string ToastID;

		public HUDToast(string tid, float lifetime, float py)
		{
			ToastID = tid;

			_toastTime = lifetime;

			PositionY = new DeltaLimitedFloat(py, MOVEMENT_DELTA);
			Alignment = HUDAlignment.CENTER;

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTER,
				FontSize = 96,
				Background = HUDBackgroundDefinition.NONE,
			};
		}

		public static HUDToast Copy(HUDToast other)
		{
			var t = new HUDToast(other.ToastID, other._toastTime, other.PositionY.TargetValue);

			t._lifetime = other._lifetime;
			t.Alignment = other.Alignment;
			t.RelativePosition = other.RelativePosition;
			t.PositionY.FullSet(t.PositionY);
			t.Text = other.Text;
			t.TextColor = other.TextColor;
			t.Font = other.Font;
			t.FontSize = other.FontSize;
			t.Alpha = other.Alpha;
			t.WordWrap = other.WordWrap;
			t.TextPadding = other.TextPadding;
			t.MaxWidth = other.MaxWidth;
			t.Background = other.Background;
			t.CloseOnClick = other.CloseOnClick;

			return t;
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			HUDRenderHelper.DrawAlphaBackground(sbatch, bounds, Background, Alpha);
		}

		public override void OnRemove()
		{
			//
		}

		public void Reset(string text, Color background, Color foreground, float lifetime)
		{
			if (Text != text) Text = text;
			if (Background.Color != background) Background = HUDBackgroundDefinition.CreateSimpleBlur(background, FontSize / 4f);
			if (TextColor != foreground) TextColor = foreground;

			_toastTime =  lifetime  / 0.85f;
			_lifetime  = _toastTime * 0.15f;
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (internalLabel.InnerLabelSize != innerSizeCache)
			{
				innerSizeCache = internalLabel.InnerLabelSize;

				Size = innerSizeCache.AsInflated(TextPadding * 2);
				internalLabel.Size = innerSizeCache;

				internalLabel.MaxWidth = MaxWidth - TextPadding.Width * 2;
			}

			if (!MonoSAMGame.IsInitializationLag) _lifetime += gameTime.ElapsedSeconds;

			PositionY.Update(gameTime);
			if (FloatMath.EpsilonInequals(PositionY.ActualValue, RelativePosition.Y))
			{
				RelativePosition = new FPoint(0, PositionY.ActualValue);
			}

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
			if (CloseOnClick)
			{
				_lifetime = FloatMath.Max(_lifetime, _toastTime * 0.85f);
			}
		}
	}
}
