using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.Leveleditor.Entities
{
	class CannonStub : GameEntity, ILeveleditorStub
	{
		public enum CannonStubType { Bullet, Laser, Minigun, Relay, Shield, Trishot }
		public enum CannonStubFraction { N0=0, P1=1, A2=2, A3=3, A4=4 }
		public static readonly float[] SCALES = { 0.500f, 0.750f, 1.125f, 1.500f, 1.875f, 2.500f, 3.000f };

		private FPoint _position;
		private FSize _boundingBox;
		public float Scale = 1f;
		public float Rotation = 0f;
		public CannonStubType CannonType = CannonStubType.Bullet;
		public CannonStubFraction CannonFrac = CannonStubFraction.N0;

		public override FPoint Position => _position;
		public override FSize DrawingBoundingBox => new FSize(Cannon.CANNON_OUTER_DIAMETER * Scale, Cannon.CANNON_OUTER_DIAMETER * Scale);

		public override Color DebugIdentColor => Color.Red;

		public CannonStub(GameScreen scrn, FPoint pos, float scale) : base(scrn, GDConstants.ORDER_GAME_CANNON)
		{
			_position = pos;
		}

		public override void OnInitialize(EntityManager manager) { }
		public override void OnRemove() { }
		protected override void OnUpdate(SAMTime gameTime, InputState istate) { }

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			switch (CannonType)
			{
				case CannonStubType.Bullet:
					DrawBodyAndBarrel_BG(sbatch);
					break;

				case CannonStubType.Laser:
					break;

				case CannonStubType.Minigun:
					break;

				case CannonStubType.Relay:
					break;

				case CannonStubType.Shield:
					break;

				case CannonStubType.Trishot:
					break;

				default:
					SAMLog.Error("LECS::EnumSwitch_CS_OD", "CannonType = " + CannonType);
					break;
			}
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			switch (CannonType)
			{
				case CannonStubType.Bullet:
					DrawBodyAndBarrel_FG(sbatch);
					DrawCog(sbatch);
					break;

				case CannonStubType.Laser:
					break;

				case CannonStubType.Minigun:
					break;

				case CannonStubType.Relay:
					break;

				case CannonStubType.Shield:
					break;

				case CannonStubType.Trishot:
					break;

				default:
					SAMLog.Error("LECS::EnumSwitch_CS_ODOFL", "CannonType = " + CannonType);
					break;
			}
		}

		private void DrawBodyAndBarrel_BG(IBatchRenderer sbatch)
		{
			var recoil = 0;

			var barrelCenter = Position + new Vector2(Scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrelShadow,
				barrelCenter,
				Scale,
				Color.White,
				Rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBodyShadow,
				Position,
				Scale,
				Color.White,
				Rotation);
		}

		private void DrawBodyAndBarrel_FG(IBatchRenderer sbatch)
		{
			var recoil = 0;

			var barrelCenter = Position + new Vector2(Scale * (Cannon.CANNON_DIAMETER / 2f - recoil), 0).Rotate(Rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBarrel,
				barrelCenter,
				Scale,
				Color.White,
				Rotation);

			sbatch.DrawScaled(
				Textures.TexCannonBody,
				Position,
				Scale,
				Color.White,
				Rotation);
		}

		private void DrawCog(IBatchRenderer sbatch)
		{
			var health = 1;

			sbatch.DrawScaled(
				Textures.CannonCog,
				Position,
				Scale,
				FlatColors.Clouds,
				Lifetime * Cannon.BASE_COG_ROTATION_SPEED + FloatMath.RAD_POS_270);

			int aidx = (int)(health * (Textures.ANIMATION_CANNONCOG_SIZE - 1));

			if (aidx == Textures.ANIMATION_CANNONCOG_SIZE - 1)
			{
				sbatch.DrawScaled(
					Textures.CannonCog,
					Position,
					Scale,
					Fraction.FRACTION_COLORS[(int)CannonFrac],
					Lifetime * Cannon.BASE_COG_ROTATION_SPEED + FloatMath.RAD_POS_270);
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
							Position,
							Scale,
							Fraction.FRACTION_COLORS[(int)CannonFrac],
							Lifetime * Cannon.BASE_COG_ROTATION_SPEED + FloatMath.RAD_POS_270 + i * radpersegm);
					}
				}
			}
		}

		public bool CollidesWith(CannonStub other)
		{
			
			var minD = FloatMath.Max(this.Scale, other.Scale) * Cannon.CANNON_OUTER_DIAMETER/2 + FloatMath.Min(this.Scale, other.Scale) * Cannon.CANNON_DIAMETER / 2;

			return (Position - other.Position).LengthSquared() < minD * minD;
		}

		public void Kill()
		{
			Remove();
		}
	}
}
