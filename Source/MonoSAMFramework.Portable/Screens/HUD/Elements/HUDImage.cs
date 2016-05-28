using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.MathHelper.FloatClasses;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using System;

namespace MonoSAMFramework.Portable.Screens.HUD.Elements
{
	public class HUDImage : HUDElement
	{
		public override int Depth { get; }

		public HUDImageAlignment ImageAlignment = HUDImageAlignment.UNDERSCALE;
		public TextureRegion2D Image;

		public HUDImage(int depth = 0)
		{
			Depth = depth;
		}

		protected override void DoDraw(IBatchRenderer sbatch, FRectangle bounds)
		{
			if (Image == null) return;

			FRectangle destination;
			
			destination = CalculateImageBounds(bounds).AsOffseted(bounds.TopLeft);

			sbatch.Draw(Image.Texture, destination, Image.Bounds, Color.White);
		}
		
		public FRectangle CalculateImageBounds(FRectangle bounds)
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
					var height = (Image.Width * 1f / Image.Height) * bounds.Height;
					destination = new FRectangle(0, (bounds.Height - height) / 2, bounds.Width, height);
					break;
				}
				case HUDImageAlignment.SCALE_Y:
				{
					var width = (Image.Height * 1f / Image.Width) * bounds.Width;
					destination = new FRectangle((bounds.Width - width) / 2, 0, width, bounds.Height);
					break;
				}
				case HUDImageAlignment.UNDERSCALE:
				{
					if (bounds.Height / bounds.Width > Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						destination = new FRectangle(0, (bounds.Height - height) / 2, bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						destination = new FRectangle((bounds.Width - width) / 2, 0, width, bounds.Height);
					}
					break;
				}
				case HUDImageAlignment.OVERSCALE:
				{
					if (bounds.Height / bounds.Width < Image.Height * 1f / Image.Width)
					{
						var height = (Image.Height * 1f / Image.Width) * bounds.Width;
						destination = new FRectangle(0, (bounds.Height - height) / 2, bounds.Width, height);
					}
					else
					{
						var width = (Image.Width * 1f / Image.Height) * bounds.Height;
						destination = new FRectangle((bounds.Width - width) / 2, 0, width, bounds.Height);
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

		protected override void DoUpdate(GameTime gameTime, InputState istate)
		{
			// NOP
		}
	}
}
