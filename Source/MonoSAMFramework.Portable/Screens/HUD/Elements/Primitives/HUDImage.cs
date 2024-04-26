using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDImage : HUDElement
	{
		public override int Depth { get; }
		
		private HUDImageAlignmentAlgorithm _imageAligment = HUDImageAlignmentAlgorithm.CENTER;
		public HUDImageAlignmentAlgorithm ImageAlignment
		{
			get { return _imageAligment; }
			set { _imageAligment = value; InvalidatePosition(); }
		}
		
		private HUDImageScaleAlgorithm _imageScale = HUDImageScaleAlgorithm.UNDERSCALE;
		public HUDImageScaleAlgorithm ImageScale
		{
			get { return _imageScale; }
			set { _imageScale = value; InvalidatePosition(); }
		}

		private TextureRegion2D _image;
		public TextureRegion2D Image
		{
			get { return _image; }
			set { _image = value; InvalidatePosition(); }
		}

		private Color _color = Color.White;
		public Color Color
		{
			get { return _color; }
			set { _color = value; }
		}

		public float RenderScaleOverride { get; set; } = 1f;

		private FRectangle imageBounds = FRectangle.Empty;

		public float Rotation = 0f;
		public float RotationSpeed = 0f; // = rot/sec

		public HUDImage(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Image == null) return;
			if (imageBounds.IsEmpty) return;
			
			sbatch.DrawCentered(Image, imageBounds.Center, imageBounds.Width * RenderScaleOverride, imageBounds.Height * RenderScaleOverride, Color, Rotation);
		}

		protected override void OnAfterRecalculatePosition()
		{
			base.OnAfterRecalculatePosition();

			imageBounds = CalculateImageRenderBounds(BoundingRectangle);
		}

		protected override void DrawDebugHUDBorders(IBatchRenderer sbatch)
		{
			sbatch.DrawRectangle(BoundingRectangle, Color.Magenta, 2f);

			sbatch.DrawRectangle(imageBounds, Color.Magenta, 1f);
			sbatch.DrawLine(imageBounds.TopLeft,  imageBounds.BottomRight, Color.Magenta, 1f);
			sbatch.DrawLine(imageBounds.TopRight, imageBounds.BottomLeft,  Color.Magenta, 1f);
		}

		public FRectangle GetRealBounds()
		{
			return imageBounds;
		}

		public FRectangle CalculateRealBounds(FRectangle tmpBounds)
		{
			return CalculateImageRenderBounds(tmpBounds);
		}

		private FRectangle CalculateImageRenderBounds(FRectangle bounds)
		{
			if (Image == null) return FRectangle.Empty;

			FSize targetSize = FSize.Empty;
			switch (ImageScale)
			{
				case HUDImageScaleAlgorithm.NO_SCALE:
				{
					targetSize = Image.Size();
				}
				break;

				case HUDImageScaleAlgorithm.STRETCH:
				{
					targetSize = bounds.Size;
				}
				break;

				case HUDImageScaleAlgorithm.SCALE_X:
				{
					var height = (Image.Height * 1f / Image.Width) * bounds.Width;
					targetSize = new FSize(bounds.Width, height);
				}
				break;

				case HUDImageScaleAlgorithm.SCALE_Y:
				{
					var width = (Image.Width * 1f / Image.Height) * bounds.Height;
					targetSize = new FSize(width, bounds.Height);
				}
				break;

				case HUDImageScaleAlgorithm.UNDERSCALE:
				{
					if (bounds.Height / bounds.Width > Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						targetSize = new FSize(bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						targetSize = new FSize(width, bounds.Height);
					}
				}
				break;
				
				case HUDImageScaleAlgorithm.OVERSCALE:
				{
					if (bounds.Height / bounds.Width < Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						targetSize = new FSize(bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						targetSize = new FSize(width, bounds.Height);
					}
				}
				break;

				default:
					SAMLog.Error("HUDI::EnumSwitch_CIRB1", "ImageScale = " + ImageScale);
					break;
			}

			FRectangle destination = FRectangle.Empty;

			switch (ImageAlignment)
			{
				case HUDImageAlignmentAlgorithm.CENTER:
					destination = FRectangle.CreateByCenter(bounds.Center, targetSize);
					break;

				case HUDImageAlignmentAlgorithm.TOPLEFT:
					destination = FRectangle.CreateByTopLeft(bounds.TopLeft, targetSize);
					break;

				case HUDImageAlignmentAlgorithm.TOPRIGHT:
					destination = FRectangle.CreateByTopRight(bounds.TopRight, targetSize);
					break;

				case HUDImageAlignmentAlgorithm.BOTTOMLEFT:
					destination = FRectangle.CreateByBottomLeft(bounds.BottomLeft, targetSize);
					break;

				case HUDImageAlignmentAlgorithm.BOTTOMRIGHT:
					destination = FRectangle.CreateByBottomRight(bounds.BottomRight, targetSize);
					break;

				default:
					SAMLog.Error("HUDI::EnumSwitch_CIRB2", "ImageAlignment = " + ImageAlignment);
					break;
			}
			
			return destination;
		}

		public override void OnInitialize()
		{
			// NOP
		}

		public override void OnRemove()
		{
			// NOP
		}

		protected override void DoUpdate(SAMTime gameTime, InputState istate)
		{
			if (RotationSpeed > 0f) Rotation = FloatMath.IncRadians(Rotation, gameTime.ElapsedSeconds * RotationSpeed * FloatMath.TAU);
		}
	}
}
