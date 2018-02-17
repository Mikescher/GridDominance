using System.Collections.Generic;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
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

		private LevelEditorScreen GDOwner => (LevelEditorScreen) Owner;

		public FPoint CannonPosition;
		private FSize _boundingBox;
		public float Scale = 1f;
		public float Rotation = 0f;
		public CannonStubType CannonType = CannonStubType.Bullet;
		public CannonStubFraction CannonFrac = CannonStubFraction.N0;

		public override FPoint Position => CannonPosition;
		public override FSize DrawingBoundingBox => new FSize(Cannon.CANNON_OUTER_DIAMETER * Scale, Cannon.CANNON_OUTER_DIAMETER * Scale);

		public override Color DebugIdentColor => Color.Red;

		public CannonStub(GameScreen scrn, FPoint pos, float scale) : base(scrn, GDConstants.ORDER_GAME_CANNON)
		{
			CannonPosition = pos;
		}

		public override void OnInitialize(EntityManager manager) { }
		public override void OnRemove() { }
		protected override void OnUpdate(SAMTime gameTime, InputState istate) { }

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (GDOwner.Selection == this) sbatch.DrawCentered(Textures.TexPixel, Position, Cannon.CANNON_OUTER_DIAMETER * Scale, Cannon.CANNON_OUTER_DIAMETER * Scale, Color.Black*0.333f);

			switch (CannonType)
			{
				case CannonStubType.Bullet:
					CommonCannonRenderer.DrawBulletCannon_BG(sbatch, Position, Scale, Rotation, 1);
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
					CommonCannonRenderer.DrawBulletCannon_FG(sbatch, Position, Scale, Rotation, 1, Lifetime * Cannon.BASE_COG_ROTATION_SPEED, 1, Fraction.FRACTION_COLORS[(int)CannonFrac]);
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

		public bool CollidesWith(CannonStub other)
		{
			var minD = FloatMath.Max(this.Scale, other.Scale) * Cannon.CANNON_OUTER_DIAMETER/2 + FloatMath.Min(this.Scale, other.Scale) * Cannon.CANNON_DIAMETER / 2;

			return (Position - other.Position).LengthSquared() < minD * minD;
		}

		public IEnumerable<SingleAttrOption> AttrOptions
		{
			get
			{
				yield return new SingleAttrOption
				{
					Action = ChangeFrac,
					Description = L10NImpl.STR_LVLED_BTN_FRAC,
					Icon = () => Textures.TexFractionBlob,
					IconColor = () => Fraction.FRACTION_COLORS[(int)CannonFrac],
					Text = () => Fraction.FRACTION_STRINGS[(int)CannonFrac],
					TextColor = () => FlatColors.Foreground,
				};
			}
		}

		private void ChangeFrac()
		{
			CannonFrac = (CannonStubFraction)(((int)CannonFrac + 1) % 5);
		}

		public void Kill()
		{
			Remove();
		}

		public bool IsClicked(FPoint ptr)
		{
			var minD = this.Scale * Cannon.CANNON_DIAMETER / 2;

			return (Position - ptr).LengthSquared() < minD * minD;
		}
	}
}
