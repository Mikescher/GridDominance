using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDInfoBox : HUDContainer
	{
		public override int Depth { get; }

		#region Properties

		public string Text
		{
			get { return internalLabel.Text; }
			set { internalLabel.Text = value; innerSizeCache = FSize.Empty; }
		}
		
		public Color TextColor
		{
			get => internalLabel.TextColor;
			set => internalLabel.TextColor = value;
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

		private int _maxWidth = 10000;
		public int MaxWidth
		{
			get { return _maxWidth; }
			set { _maxWidth = value; innerSizeCache = FSize.Empty; }
		}

		public HUDBackgroundDefinition Background = HUDBackgroundDefinition.DUMMY;

		public bool CloseOnClick = true;

		#endregion
		
		private readonly HUDLabel internalLabel;
		private FSize innerSizeCache = FSize.Empty;

		public HUDInfoBox(int depth = 0)
		{
			Depth = depth;

			Alignment = HUDAlignment.CENTER;

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTER,
				FontSize = 96,
				Background = HUDBackgroundDefinition.NONE,
			};
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

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (internalLabel.InnerLabelSize != innerSizeCache)
			{
				innerSizeCache = internalLabel.InnerLabelSize;

				Size = innerSizeCache.AsInflated(TextPadding * 2);
				internalLabel.Size = innerSizeCache;

				internalLabel.MaxWidth = MaxWidth - TextPadding.Width * 2;
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
