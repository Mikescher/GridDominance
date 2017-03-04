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
	public class HUDMessageBox : HUDContainer
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

		private FSize _textPadding = new FSize(48, 12);
		public FSize TextPadding
		{
			get { return _textPadding; }
			set { _textPadding = value; innerSizeCache = FSize.Empty; }
		}

		private FSize _margin = new FSize(48, 48);
		public FSize Margin
		{
			get { return _margin; }
			set { _margin = value; innerSizeCache = FSize.Empty; }
		}

		public Color ColorBackground = Color.White;

		public HUDBackgroundType BackgoundType = HUDBackgroundType.Rounded;
		public float BackgoundCornerSize = 16f;

		public bool CloseOnClick = true;

		#endregion
		
		private readonly HUDLabel internalLabel;
		private FSize innerSizeCache = FSize.Empty;

		public HUDMessageBox(int depth = 0)
		{
			Depth = depth;

			Alignment = HUDAlignment.CENTER;

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.CENTER,
				TextAlignment = HUDAlignment.CENTER,
				FontSize = 96,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			SimpleRenderHelper.DrawHUDBackground(sbatch, BackgoundType, bounds, ColorBackground, BackgoundCornerSize);
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

				internalLabel.MaxWidth = Owner.Width - TextPadding.Width * 2 - Margin.Width * 2;
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
