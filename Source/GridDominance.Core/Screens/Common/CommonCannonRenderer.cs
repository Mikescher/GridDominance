using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
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
		#region Bullet

		public static void DrawBulletCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			DrawBulletBodyAndBarrel_BG(sbatch, position, scale, rotation, barrelRecoil);
		}

		public static void DrawBulletCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil, float cogRotation, float health, Color fracColor)
		{
			DrawBulletBodyAndBarrel_FG(sbatch, position, scale, rotation, barrelRecoil);
			DrawCog(sbatch, position, scale, cogRotation, health, fracColor);
		}

		private static void DrawBulletBodyAndBarrel_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
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

		private static void DrawBulletBodyAndBarrel_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
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

		#endregion

		#region Laser

		public static void DrawLaserCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation)
		{
			DrawLaserBodyAndBarrel_BG(sbatch, position, scale, rotation, false);
		}

		public static void DrawLaserCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, bool neutral, float health, float coreRotation, float corePulse, int coreImage, Color fracColor)
		{
			DrawLaserBodyAndBarrel_FG(sbatch, position, scale, rotation, false);
			DrawLaserCore(sbatch, position, scale, neutral, health, coreRotation, corePulse, coreImage, fracColor);
		}

		private static void DrawLaserBodyAndBarrel_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, bool isshield)
		{
			var barrelCenter = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f), 0).Rotate(rotation);

			sbatch.DrawScaled(
				isshield ? Textures.TexShieldBarrelShadow : Textures.TexLaserBarrelShadow,
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

		private static void DrawLaserBodyAndBarrel_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, bool isshield)
		{
			var barrelCenter = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f), 0).Rotate(rotation);

			sbatch.DrawScaled(
				isshield ? Textures.TexShieldBarrel : Textures.TexLaserBarrel,
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

		private static void DrawLaserCore(IBatchRenderer sbatch, FPoint position, float scale, bool neutral, float health, float coreRotation, float corePulse, int coreImage, Color fracColor)
		{
			sbatch.DrawScaled(
				Textures.TexCannonCoreShadow[coreImage],
				position,
				scale * corePulse,
				Color.White,
				coreRotation);

			if (!neutral && health > 0)
			{
				sbatch.DrawScaled(
					Textures.TexCannonCore[coreImage],
					position,
					scale * corePulse * FloatMath.Sqrt(health),
					fracColor,
					coreRotation);
			}
		}

		#endregion

		#region Minigun

		public static void DrawMinigunCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			DrawBulletBodyAndBarrel_BG(sbatch, position, scale, rotation, barrelRecoil);
		}

		public static void DrawMinigunCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil, float cogRotation, float health, Color fracColor)
		{
			DrawBulletBodyAndBarrel_FG(sbatch, position, scale, rotation, barrelRecoil);
			DrawDoubleRect(sbatch, position, scale, health, cogRotation, fracColor);
		}

		private static void DrawDoubleRect(IBatchRenderer sbatch, FPoint position, float scale, float health, float cogRotation, Color fracColor)
		{
			if (health > 0.99) health = 1f;

			sbatch.DrawCentered(
				Textures.TexPixel,
				position,
				scale * Cannon.MINIGUNSTRUCT_DIAMETER,
				scale * Cannon.MINIGUNSTRUCT_DIAMETER,
				FlatColors.Clouds,
				FloatMath.RAD_POS_000 - cogRotation);

			sbatch.DrawCentered(
				Textures.TexPixel,
				position,
				scale * Cannon.MINIGUNSTRUCT_DIAMETER,
				scale * Cannon.MINIGUNSTRUCT_DIAMETER,
				FlatColors.Clouds,
				FloatMath.RAD_POS_000 + cogRotation);

			if (health < 0.01)
			{
				// nothing
			}
			else if (health < 1)
			{
				var r = FRectangle.CreateByCenter(position, health * scale * Cannon.MINIGUNSTRUCT_DIAMETER, health * scale * Cannon.MINIGUNSTRUCT_DIAMETER);

				sbatch.FillRectangleRot(r, fracColor * (1 - health), FloatMath.RAD_POS_000 + cogRotation);
				sbatch.FillRectangleRot(r, fracColor * (1 - health), FloatMath.RAD_POS_000 - cogRotation);

				sbatch.DrawRectangleRot(r, fracColor, FloatMath.RAD_POS_000 + cogRotation, 2f);
				sbatch.DrawRectangleRot(r, fracColor, FloatMath.RAD_POS_000 - cogRotation, 2f);
			}
			else
			{
				var r = FRectangle.CreateByCenter(position, scale * Cannon.MINIGUNSTRUCT_DIAMETER, scale * Cannon.MINIGUNSTRUCT_DIAMETER);

				sbatch.DrawRectangleRot(r, fracColor, FloatMath.RAD_POS_000 + cogRotation, 2f);
				sbatch.DrawRectangleRot(r, fracColor, FloatMath.RAD_POS_000 - cogRotation, 2f);
			}
		}

		#endregion

		#region Relay

		public static void DrawRelayCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			DrawBulletBodyAndBarrel_BG(sbatch, position, scale, rotation, barrelRecoil);
		}

		public static void DrawRelayCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil, Color fracColor)
		{
			DrawBulletBodyAndBarrel_FG(sbatch, position, scale, rotation, barrelRecoil);
			DrawSingleDot(sbatch, position, scale, fracColor);
		}

		private static void DrawSingleDot(IBatchRenderer sbatch, FPoint position, float scale, Color fracColor)
		{
			sbatch.DrawCentered(
				Textures.TexCircle,
				position,
				scale * Cannon.CANNON_DIAMETER * 0.5f,
				scale * Cannon.CANNON_DIAMETER * 0.5f,
				fracColor);
		}

		#endregion

		#region Shield

		public static void DrawShieldCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation)
		{
			DrawLaserBodyAndBarrel_BG(sbatch, position, scale, rotation, true);
		}

		public static void DrawShieldCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, bool neutral, float health, float coreRotation, float satellites, Color fracColor)
		{
			DrawLaserBodyAndBarrel_FG(sbatch, position, scale, rotation, true);
			DrawShieldCore(sbatch, position, scale, neutral, health, coreRotation, satellites, fracColor);
		}

		private static void DrawShieldCore(IBatchRenderer sbatch, FPoint position, float scale, bool neutral, float health, float coreRotation, float satellites, Color fracColor)
		{
			if (satellites <= 0)
			{
				sbatch.DrawCentered(
					Textures.TexCircle,
					position,
					scale * Cannon.CANNON_DIAMETER * 0.5f,
					scale * Cannon.CANNON_DIAMETER * 0.5f,
					FlatColors.Clouds);

				if (!neutral && health > 0)
				{
					sbatch.DrawCentered(
						Textures.TexCircle,
						position,
						scale * Cannon.CANNON_DIAMETER * 0.5f * health,
						scale * Cannon.CANNON_DIAMETER * 0.5f * health,
						fracColor);
				}
			}
			else
			{
				if (neutral)
				{
					sbatch.DrawCentered(
						Textures.TexCircle,
						position,
						scale * Cannon.CANNON_DIAMETER * 0.5f,
						scale * Cannon.CANNON_DIAMETER * 0.5f,
						FlatColors.Clouds);
				}
				else
				{
					var p1 = position + new Vector2(scale * Cannon.CANNON_DIAMETER * 0.2f * satellites, 0).Rotate(coreRotation + FloatMath.RAD_POS_000);
					var p2 = position + new Vector2(scale * Cannon.CANNON_DIAMETER * 0.2f * satellites, 0).Rotate(coreRotation + FloatMath.RAD_POS_120);
					var p3 = position + new Vector2(scale * Cannon.CANNON_DIAMETER * 0.2f * satellites, 0).Rotate(coreRotation + FloatMath.RAD_POS_240);

					var ws = scale * Cannon.CANNON_DIAMETER * 0.5f;
					var we = scale * Cannon.CANNON_DIAMETER * 0.5f / 2f;

					var diam = ws + (we - ws) * satellites;

					sbatch.DrawCentered(Textures.TexCircle, p1, diam, diam, fracColor);
					sbatch.DrawCentered(Textures.TexCircle, p2, diam, diam, fracColor);
					sbatch.DrawCentered(Textures.TexCircle, p3, diam, diam, fracColor);
				}
			}
		}

		#endregion

		#region Trishot

		public static void DrawTrishotCannon_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			DrawTrishotBodyAndBarrel_BG(sbatch, position, scale, rotation, barrelRecoil);
		}

		public static void DrawTrishotCannon_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil, float cogRotation, float health, Color fracColor)
		{
			DrawTrishotBodyAndBarrel_FG(sbatch, position, scale, rotation, barrelRecoil);
			DrawCog(sbatch, position, scale, cogRotation, health, fracColor);
		}

		private static void DrawTrishotBodyAndBarrel_BG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			var recoil = (1 - barrelRecoil) * Cannon.BARREL_RECOIL_LENGTH;

			var barrelCenter1 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation - Cannon.TRISHOT_BARREL_ANGLE);
			var barrelCenter2 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation);
			var barrelCenter3 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation + Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter1,
				scale,
				Color.White,
				rotation - Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter2,
				scale,
				Color.White,
				rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter3,
				scale,
				Color.White,
				rotation + Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBodyShadow,
				position,
				scale,
				Color.White,
				rotation);
		}

		private static void DrawTrishotBodyAndBarrel_FG(IBatchRenderer sbatch, FPoint position, float scale, float rotation, float barrelRecoil)
		{
			var recoil = (1 - barrelRecoil) * Cannon.BARREL_RECOIL_LENGTH;

			var barrelCenter1 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation - Cannon.TRISHOT_BARREL_ANGLE);
			var barrelCenter2 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation);
			var barrelCenter3 = position + new Vector2(scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(rotation + Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter1,
				scale,
				Color.White,
				rotation - Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter2,
				scale,
				Color.White,
				rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter3,
				scale,
				Color.White,
				rotation + Cannon.TRISHOT_BARREL_ANGLE);

			sbatch.DrawScaled(
				Textures.TexCannonBody,
				position,
				scale,
				Color.White,
				rotation);
		}

		#endregion
	}
}
