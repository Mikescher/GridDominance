using System;
using System.Collections.Generic;
using System.Globalization;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.LevelEditorScreen.Entities
{
	public class PortalStub : GameEntity, ILeveleditorStub
	{
		private LevelEditorScreen GDOwner => (LevelEditorScreen) Owner;

		public static readonly float[] ROTS =
		{
			(0 * 4 + 0) * (FloatMath.TAU/16), (0 * 4 + 1) * (FloatMath.TAU/16), (0 * 4 + 2) * (FloatMath.TAU/16), (0 * 4 + 3) * (FloatMath.TAU/16),
			(1 * 4 + 0) * (FloatMath.TAU/16), (1 * 4 + 1) * (FloatMath.TAU/16), (1 * 4 + 2) * (FloatMath.TAU/16), (1 * 4 + 3) * (FloatMath.TAU/16),
			(2 * 4 + 0) * (FloatMath.TAU/16), (2 * 4 + 1) * (FloatMath.TAU/16), (2 * 4 + 2) * (FloatMath.TAU/16), (2 * 4 + 3) * (FloatMath.TAU/16),
			(3 * 4 + 0) * (FloatMath.TAU/16), (3 * 4 + 1) * (FloatMath.TAU/16), (3 * 4 + 2) * (FloatMath.TAU/16), (3 * 4 + 3) * (FloatMath.TAU/16),
		};

		public static readonly string[] ROT_STR =
		{
			"E", "SEE", "SE", "SSE", "S", "SSW", "SW", "SWW", "W", "NWW", "NW", "NNW", "N", "NNE", "NE", "NEE"
		};

		public FPoint Center;
		public float Normal;
		public float Length;
		public int   Group;
		public bool  Side;

		public override FPoint Position => Center;
		public override FSize DrawingBoundingBox => new FSize(Length, Length);

		public override Color DebugIdentColor => Color.Red;

		private EquatableTuple<float, float, FPoint> _ppCacheKey;
		private FPoint _ppCacheP2 = FPoint.NaN;
		private FRectangle[] _ppCacheRects = null;

		public PortalStub(GameScreen scrn, FPoint c, float len, float rot) : base(scrn, GDConstants.ORDER_GAME_PORTAL)
		{
			Center   = c;
			Normal   = rot;
			Length   = len;
			Group    = 1;
			Side     = false;
		}

		public PortalStub(GameScreen scrn, SCCMLevelElement dat) : base(scrn, GDConstants.ORDER_GAME_PORTAL)
		{
			Center   = dat.Portal_Center;
			Normal   = dat.Portal_Normal;
			Length   = dat.Portal_Length;
			Group    = dat.Portal_Group;
			Side     = dat.Portal_Side;
		}

		public override void OnInitialize(EntityManager manager) { }
		public override void OnRemove() { }
		protected override void OnUpdate(SAMTime gameTime, InputState istate) { }

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (GDOwner.Selection == this)
			{
				sbatch.FillShape(GetArea().AsInflated(GDConstants.TILE_WIDTH/2, GDConstants.TILE_WIDTH/2), Color.Black * 0.333f);
			}

			if (_ppCacheKey != EquatableTuple.Create(Length, Normal, Position))
			{
				_ppCacheKey = EquatableTuple.Create(Length, Normal, Position);
				_ppCacheRects = CommonObstacleRenderer.CreatePortalRenderRects(Position, Vector2.UnitX.Rotate(Normal), Vector2.UnitX.Rotate(Normal).RotateWithLength(FloatMath.RAD_POS_090, Length / 2f), Length);
			}

			CommonObstacleRenderer.DrawPortal(sbatch, _ppCacheRects, Portal.COLORS[Group], Normal);
		}

#if DEBUG
		protected override void DrawDebugBorders(IBatchRenderer sbatch)
		{
			base.DrawDebugBorders(sbatch);

			sbatch.DrawShape(GetArea(), Color.Magenta, 1 * GDOwner.PixelWidth);
			sbatch.DrawShape(GetClickArea(), Color.LightGoldenrodYellow, 1 * GDOwner.PixelWidth);
		}
#endif

		public bool CollidesWith(CannonStub other)
		{
			return this.GetArea().Overlaps(other.GetOuterArea());
		}

		public bool CollidesWith(ObstacleStub other)
		{
			switch (other.ObstacleType)
			{
				case ObstacleStub.ObstacleStubType.BlackHole:
				case ObstacleStub.ObstacleStubType.WhiteHole:
					return false;

				case ObstacleStub.ObstacleStubType.GlassBlock:
				case ObstacleStub.ObstacleStubType.MirrorBlock:
				case ObstacleStub.ObstacleStubType.MirrorCircle:
					return this.GetArea().Overlaps(other.GetArea());

				case ObstacleStub.ObstacleStubType.VoidVircle:
					return false;

				default:
					SAMLog.Error("LEPS::EnumSwitch_CW_OS", "other.ObstacleType = " + other.ObstacleType);
					return true;
			}
		}

		public bool CollidesWith(WallStub other)
		{
			return false;
		}

		public bool CollidesWith(PortalStub other)
		{
			return other.GetShortenedArea().Overlaps(this.GetShortenedArea());
		}

		public IEnumerable<SingleAttrOption> AttrOptions
		{
			get
			{
				yield return new SingleAttrOption
				{
					Action = ChangeGroup,
					Description = L10NImpl.STR_LVLED_BTN_CHANNEL,
					Icon = () => Textures.TexPixel,
					IconScale = 0.4f,
					IconColor = () => Portal.COLORS[Group],
				};

				yield return new SingleAttrOption
				{
					Action = ChangeSide,
					Description = L10NImpl.STR_LVLED_BTN_DIR,
					Icon = () => null,
					Text = () => Side ? "I" : "O",
					TextColor = () => FlatColors.Foreground,
				};

				yield return new SingleAttrOption
				{
					Action = ChangeLen,
					Description = L10NImpl.STR_LVLED_BTN_LEN,
					Icon = () => null,
					Text = () => Convert.ToString(FloatMath.Round(Length / (GDConstants.TILE_WIDTH / 2f))/2f, CultureInfo.InvariantCulture),
					TextColor = () => FlatColors.Foreground,
				};

				yield return new SingleAttrOption
				{
					Action = ChangeRot,
					Description = L10NImpl.STR_LVLED_BTN_ROT,
					Icon = () => null,
					Text = () => ROT_STR[FloatMath.Max(0, ROTS.IndexOf(Normal))],
					TextColor = () => FlatColors.Foreground,
				};
			}
		}

		private void ChangeLen()
		{
			float nextlen = (GDConstants.TILE_WIDTH / 2f) * (FloatMath.Round(Length / (GDConstants.TILE_WIDTH / 2f)) + 1);

			if (nextlen > GDConstants.TILE_WIDTH*4) nextlen = (GDConstants.TILE_WIDTH / 2f);

			if (GDOwner.CanInsertPortalStub(Position, nextlen, Normal, this) != null)
			{
				Length = nextlen;
			}
			else
			{
				Length = (GDConstants.TILE_WIDTH / 2f);
			}
		}

		private void ChangeRot()
		{
			var idxCurr = ROTS.IndexOf(Normal);

			for (int i = 1; i < ROTS.Length; i++)
			{
				var idxTest = (idxCurr + i) % ROTS.Length;
				if (GDOwner.CanInsertPortalStub(Position, Length, ROTS[idxTest], this) != null)
				{
					Normal = ROTS[idxTest];
					return;
				}
			}
		}

		private void ChangeSide()
		{
			Side = !Side;
		}

		private void ChangeGroup()
		{
			Group = 1 + (Group) % 8;
		}

		public void Kill()
		{
			Remove();
		}

		public IFShape GetClickArea()
		{
			return FRotatedRectangle.CreateByCenter(Center, Length + GDConstants.TILE_WIDTH, Portal.WIDTH + GDConstants.TILE_WIDTH, Normal + FloatMath.RAD_NEG_090);
		}

		public FRotatedRectangle GetArea()
		{
			return FRotatedRectangle.CreateByCenter(Center, Length, Portal.WIDTH, Normal + FloatMath.RAD_NEG_090);
		}

		public FRotatedRectangle GetShortenedArea()
		{
			return FRotatedRectangle.CreateByCenter(Center, Length - 17, Portal.WIDTH, Normal + FloatMath.RAD_NEG_090);
		}
	}
}
