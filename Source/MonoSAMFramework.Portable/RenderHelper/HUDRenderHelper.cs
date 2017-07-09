using System;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public static class HUDRenderHelper
	{
		public static void DrawBackground(IBatchRenderer sbatch, FRectangle bounds, HUDBackgroundDefinition def)
		{
			if (def.Color == Color.Transparent) return;
			if (bounds.IsEmpty) return;

			switch (def.Type)
			{
				case HUDBackgroundDefinitionType.None:
					// Do nothing
					break;
				case HUDBackgroundDefinitionType.Simple:
					SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, def.Color);
					break;
				case HUDBackgroundDefinitionType.SimpleOutline:
					SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, def.Color);
					SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, def.OutlineThickness, def.OutlineColor);
					break;
				case HUDBackgroundDefinitionType.Rounded:
					SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, def.Color, def.RoundedCornerTL, def.RoundedCornerTR, def.RoundedCornerBL, def.RoundedCornerBR, def.CornerSize);
					break;
				case HUDBackgroundDefinitionType.RoundedBlur:
					FlatRenderHelper.DrawRoundedBlurPanel(sbatch, bounds, def.Color, def.RoundedCornerTL, def.RoundedCornerTR, def.RoundedCornerBL, def.RoundedCornerBR, def.CornerSize);
					break;
				case HUDBackgroundDefinitionType.SimpleBlur:
					FlatRenderHelper.DrawSimpleBlurPanel(sbatch, bounds, def.Color, def.CornerSize);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void DrawAlphaBackground(IBatchRenderer sbatch, FRectangle bounds, HUDBackgroundDefinition def, float alpha)
		{
			if (def.Color == Color.Transparent) return;
			if (bounds.IsEmpty) return;
			if (FloatMath.IsZero(alpha)) return;

			if (FloatMath.IsOne(alpha)) { DrawBackground(sbatch, bounds, def); return; }

			switch (def.Type)
			{
				case HUDBackgroundDefinitionType.None:
					// Do nothing
					break;
				case HUDBackgroundDefinitionType.Simple:
					SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, def.Color * alpha);
					break;
				case HUDBackgroundDefinitionType.SimpleOutline:
					SimpleRenderHelper.DrawSimpleRect(sbatch, bounds, def.Color * alpha);
					SimpleRenderHelper.DrawSimpleRectOutline(sbatch, bounds, def.OutlineThickness, def.OutlineColor * alpha);
					break;
				case HUDBackgroundDefinitionType.Rounded:
					SimpleRenderHelper.DrawRoundedRect(sbatch, bounds, def.Color * alpha, def.RoundedCornerTL, def.RoundedCornerTR, def.RoundedCornerBL, def.RoundedCornerBR, def.CornerSize);
					break;
				case HUDBackgroundDefinitionType.RoundedBlur:
					FlatRenderHelper.DrawRoundedAlphaBlurPanel(sbatch, bounds, def.Color, def.CornerSize, alpha);
					break;
				case HUDBackgroundDefinitionType.SimpleBlur:
					FlatRenderHelper.DrawSimpleAlphaBlurPanel(sbatch, bounds, def.Color, def.CornerSize, alpha);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}
}
