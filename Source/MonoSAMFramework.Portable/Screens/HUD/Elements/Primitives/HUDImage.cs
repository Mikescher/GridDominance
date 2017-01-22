using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements.Primitives
{
	public class HUDImage : HUDElement
	{
		public override int Depth { get; }

		private HUDImageAlignment _imageAligment = HUDImageAlignment.UNDERSCALE;
		public HUDImageAlignment ImageAlignment
		{
			get { return _imageAligment; }
			set { _imageAligment = value; InvalidatePosition(); }
		}

		private TextureRegion2D _image;
		public TextureRegion2D Image
		{
			get { return _image; }
			set { _image = value; InvalidatePosition(); }
		}

		public float RenderScaleOverride { get; set; } = 1f;

		private FRectangle imageBounds = FRectangle.Empty;

		public HUDImage(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Image == null) return;
			if (imageBounds.IsEmpty) return;
			
			sbatch.DrawCentered(Image, imageBounds.Center, imageBounds.Width * RenderScaleOverride, imageBounds.Height * RenderScaleOverride, Color.White);
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
			sbatch.DrawLine(imageBounds.VectorTopLeft, imageBounds.VectorBottomRight, Color.Magenta, 1f);
			sbatch.DrawLine(imageBounds.VectorTopRight, imageBounds.VectorBottomLeft, Color.Magenta, 1f);
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

			FRectangle destination;
			switch (ImageAlignment)
			{
				case HUDImageAlignment.CENTER:
				{
					destination = Image.Bounds.ToFRectangle().AsCenteredTo(bounds.Center);
					break;
				}
				case HUDImageAlignment.TOPLEFT:
				{
					destination = new FRectangle(bounds.TopLeft, Image.Size());
					break;
				}
				case HUDImageAlignment.TOPRIGHT:
				{
					destination = new FRectangle(bounds.Right - Image.Width, bounds.Top, Image.Width, Image.Height);
					break;
				}
				case HUDImageAlignment.BOTTOMLEFT:
				{
					destination = new FRectangle(bounds.Left, bounds.Bottom - Image.Height, Image.Width, Image.Height);
					break;
				}
				case HUDImageAlignment.BOTTOMRIGHT:
				{
					destination = new FRectangle(bounds.Right - Image.Width, bounds.Bottom - Image.Height, Image.Width, Image.Height);
					break;
				}
				case HUDImageAlignment.SCALE:
				{
					destination = bounds;
					break;
				}
				case HUDImageAlignment.SCALE_X:
				{
					var height = (Image.Height * 1f / Image.Width) * bounds.Width;
					destination = new FRectangle(bounds.X, bounds.Y + (bounds.Height - height) / 2, bounds.Width, height);
					break;
				}
				case HUDImageAlignment.SCALE_Y:
				{
					var width = (Image.Width * 1f / Image.Height) * bounds.Height;
					destination = new FRectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y, width, bounds.Height);
					break;
				}
				case HUDImageAlignment.UNDERSCALE:
				{
					if (bounds.Height / bounds.Width > Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						destination = new FRectangle(bounds.X, bounds.Y + (bounds.Height - height) / 2, bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						destination = new FRectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y, width, bounds.Height);
					}
					break;
				}
				case HUDImageAlignment.OVERSCALE:
				{
					if (bounds.Height / bounds.Width < Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						destination = new FRectangle(bounds.X, bounds.Y + (bounds.Height - height) / 2, bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						destination = new FRectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y, width, bounds.Height);
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
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
			// NOP
		}
	}
}
