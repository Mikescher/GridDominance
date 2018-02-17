using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.Common
{
	public static class CommonCannonRenderer
	{
		public static void DrawBulletCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			DrawBodyAndBarrel_BG(sbatch, position, scale, rotation, barrelRecoil);
		}

		public static void DrawBulletCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil, float cogRotation, float health, Color fracColor)
		{
			DrawBodyAndBarrel_FG(sbatch, position, scale, rotation, barrelRecoil);
			DrawCog(sbatch, position, scale, cogRotation, health, fracColor);
		}

		private static void DrawBodyAndBarrel_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			var recoil = (1 - barrelRecoil) * Cannon.BARREL_RECOIL_LENGTH;

			var barrelCenter = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter,
				scale,
				Color.White,
				rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBodyShadow,
				position,
				scale,
				Color.White,
				rotation);
		}

		private static void DrawBodyAndBarrel_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			var recoil = (1 - barrelRecoil) * Cannon.BARREL_RECOIL_LENGTH;

			var barrelCenter = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter,
				scale,
				Color.White,
				rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBody,
				position,
				scale,
				Color.White,
				rotation);
		}

		private static void DrawCog(IBatchRenderer sbatch, FPoint position, float scale, float cannonCogRotation, float health, Color fracColor)
		{
			if (health > 0.99) health = 1f;

			sbatch.DrawScaled(
				Textures.CannonCog,
				position,
				scale,
				FlatColors.Clouds,
				cannonCogRotation + FloatMath.RAD_POS_270);

			int aidx = (int)(health * (Textures.ANIMATION_CANNONCOG_SIZE - 1));

			if (aidx == Textures.ANIMATION_CANNONCOG_SIZE - 1)
			{
				sbatch.DrawScaled(
					Textures.CannonCog,
					position,
					scale,
					fracColor,
					cannonCogRotation + FloatMath.RAD_POS_270);
			}
			else
			{
				int aniperseg = Textures.ANIMATION_CANNONCOG_SIZE / Textures.ANIMATION_CANNONCOG_SEGMENTS;
				float radpersegm = (FloatMath.RAD_POS_360 * 1f / Textures.ANIMATION_CANNONCOG_SEGMENTS);
				for (int i = 0; i < Textures.ANIMATION_CANNONCOG_SEGMENTS; i++)
				{
					if (aidx >= aniperseg * i)
					{
						var iidx = aidx - aniperseg * i;
						if (iidx > aniperseg + Textures.ANIMATION_CANNONCOG_OVERLAP) iidx = aniperseg + Textures.ANIMATION_CANNONCOG_OVERLAP;

						sbatch.DrawScaled(
							Textures.AnimCannonCog[iidx],
							position,
							scale,
							fracColor,
							cannonCogRotation + FloatMath.RAD_POS_270 + i * radpersegm);
					}
				}
			}
		}
	}
}
