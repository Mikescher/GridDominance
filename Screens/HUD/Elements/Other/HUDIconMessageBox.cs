using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Container;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Other
{
	public class HUDIconMessageBox : HUDContainer
	{
		public override int Depth { get; }

		#region Properties

		public int L10NText
		{
			get { return internalLabel.L10NText; }
			set { internalLabel.L10NText = value; innerSizeCache = FSize.Empty; }
		}

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

		private FSize _textPadding = new FSize(48, 24);
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

		private FSize _iconPadding = new FSize(24, 24);
		public FSize IconPadding
		{
			get { return _iconPadding; }
			set { _iconPadding = value; innerSizeCache = FSize.Empty; }
		}

		public FSize IconSize
		{
			get { return internalImage.Size; }
			set { internalImage.Size = value; innerSizeCache = FSize.Empty; }
		}

		public TextureRegion2D Icon
		{
			get { return internalImage.Image; }
			set { internalImage.Image = value; }
		}

		public Color IconColor
		{
			get { return internalImage.Color; }
			set { internalImage.Color = value; }
		}

		public HUDBackgroundDefinition Background = HUDBackgroundDefinition.DUMMY;

		public bool CloseOnClick = true;

		public float Rotation = 0f;
		public float RotationSpeed = 0f;

		#endregion
		
		private readonly HUDImage internalImage;
		private readonly HUDLabel internalLabel;

		private FSize innerSizeCache = FSize.Empty;

		public HUDIconMessageBox(int depth = 0)
		{
			Depth = depth;

			Alignment = HUDAlignment.CENTER;

			internalImage = new HUDImage
			{
				Alignment = HUDAlignment.CENTERLEFT,
				ImageAlignment = HUDImageAlignmentAlgorithm.CENTER,
				ImageScale     = HUDImageScaleAlgorithm.UNDERSCALE,
				Size = new FSize(96, 96),
			};

			internalLabel = new HUDLabel
			{
				Alignment = HUDAlignment.CENTERLEFT,
				TextAlignment = HUDAlignment.CENTER,
				FontSize = 96,
			};
		}

		public override void OnInitialize()
		{
			AddElement(internalLabel);
			AddElement(internalImage);
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			HUDRenderHelper.DrawBackground(sbatch, bounds, Background);
		}

		public override void OnRemove()
		{
			//
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			Rotation += RotationSpeed * gameTime.ElapsedSeconds;
			internalImage.Rotation = Rotation;

			if (internalLabel.InnerLabelSize != innerSizeCache)
			{
				innerSizeCache = internalLabel.InnerLabelSize;

				float sw = IconPadding.Width + IconSize.Width + TextPadding.Width + innerSizeCache.Width + TextPadding.Width;
				float sh = FloatMath.Max(IconSize.Height + 2 * IconPadding.Height, innerSizeCache.Height + 2 * TextPadding.Height);
				Size = new FSize(sw, sh);

				internalLabel.Size = innerSizeCache;
				internalLabel.RelativePosition = new FPoint(IconPadding.Width + IconSize.Width + TextPadding.Width, 0);

				internalImage.RelativePosition = new FPoint(IconPadding.Width, 0);
				
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
