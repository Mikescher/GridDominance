using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.SCCM;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Language;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.LevelEditorScreen.Entities
{
	public class WallStub : GameEntity, ILeveleditorStub
	{
		public enum WallStubType { Void=0, Glass=1, Mirror=2 }

		public readonly TextureRegion2D[] TypeTextures = new[]
		{
			Textures.TexBlackHoleIcon,
			Textures.TexWhiteHoleIcon,
			Textures.TexGlassBlockIcon,
			Textures.TexMirrorBlockIcon,
			Textures.TexMirrorCircleSmall,
			Textures.TexVoidCircle_FG,
		};

		private LevelEditorScreen GDOwner => (LevelEditorScreen) Owner;

		public FPoint Point1;
		public FPoint Point2;
		public WallStubType WallType;

		public override FPoint Position => FPoint.MiddlePoint(Point1, Point2);

		public override FSize DrawingBoundingBox => (Point1 - Point2).ToAbsFSize();

		public override Color DebugIdentColor => Color.Red;

		private EquatableTuple<FPoint, FPoint> _vvCacheKey;
		private FRectangle[] _vvCacheRects = null;
		
		public WallStub(GameScreen scrn, FPoint p1, FPoint p2) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			Point1   = p1;
			Point2   = p2;
			WallType = WallStubType.Void;
		}

		public WallStub(GameScreen scrn, SCCMLevelElement dat) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			Point1   = dat.Wall_Point1;
			Point2   = dat.Wall_Point2;
			WallType = dat.Wall_WallType;
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

			switch (WallType)
			{
				case WallStubType.Void:
					if (_vvCacheKey != EquatableTuple.Create(Point1, Point2))
					{
						_vvCacheKey = EquatableTuple.Create(Point1, Point2);
						_vvCacheRects = CommonWallRenderer.CreateVoidWallRenderRects(FPoint.MiddlePoint(Point1, Point2), (Point1 - Point2).Length(), (Point1 - Point2).ToAngle());
					}
					CommonWallRenderer.DrawVoidWall_BG(sbatch, (Point1 - Point2).Length(), (Point1 - Point2).ToAngle(), _vvCacheRects);
					break;

				case WallStubType.Glass:
					CommonWallRenderer.DrawGlassWall(sbatch, GetArea());
					break;

				case WallStubType.Mirror:
					CommonWallRenderer.DrawMirrorWall(sbatch, GetArea());
					break;

				default:
					SAMLog.Error("LEWS::EnumSwitch_CS_OD", "WallType = " + WallType);
					break;
			}
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			switch (WallType)
			{
				case WallStubType.Void:
					if (_vvCacheKey != EquatableTuple.Create(Point1, Point2))
					{
						_vvCacheKey = EquatableTuple.Create(Point1, Point2);
						_vvCacheRects = CommonWallRenderer.CreateVoidWallRenderRects(FPoint.MiddlePoint(Point1, Point2), (Point1 - Point2).Length(), (Point1 - Point2).ToAngle());
					}
					CommonWallRenderer.DrawVoidWall_FG(sbatch, (Point1 - Point2).Length(), (Point1 - Point2).ToAngle(), _vvCacheRects);
					break;

				case WallStubType.Glass:
					// NOP
					break;

				case WallStubType.Mirror:
					// NOP
					break;

				default:
					SAMLog.Error("LEWS::EnumSwitch_CS_ODOFL", "WallType = " + WallType);
					break;
			}
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
			return false;
		}

		public bool CollidesWith(ObstacleStub other)
		{
			return false;
		}

		public bool CollidesWith(WallStub other)
		{
			return other.Point1 == this.Point1 && other.Point2 == this.Point2;
		}

		public bool CollidesWith(PortalStub other)
		{
			return other.CollidesWith(this);
		}

		public IEnumerable<SingleAttrOption> AttrOptions
		{
			get
			{
				yield return new SingleAttrOption
				{
					Action = ChangeWallType,
					Description = L10NImpl.STR_LVLED_BTN_TYPE,
					Icon = () => TypeTextures[(int)WallType],
					Text = () => null,
					TextColor = () => FlatColors.Foreground,
				};
			}
		}
		
		private void ChangeWallType()
		{
			WallType = (WallStubType)(((int)WallType + 1) % 3);
			GDOwner.GDHUD.AttrPanel.Recreate(this);
		}

		public void Kill()
		{
			Remove();
		}

		public IFShape GetClickArea()
		{
			switch (WallType)
			{
				case WallStubType.Void:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length() + GDConstants.TILE_WIDTH, VoidWallBlueprint.DEFAULT_WIDTH + GDConstants.TILE_WIDTH, (Point2 - Point1).ToAngle());
				case WallStubType.Glass:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length() + GDConstants.TILE_WIDTH, GlassBlockBlueprint.DEFAULT_WIDTH + GDConstants.TILE_WIDTH, (Point2 - Point1).ToAngle());
				case WallStubType.Mirror:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length() + GDConstants.TILE_WIDTH, MirrorBlockBlueprint.DEFAULT_WIDTH + GDConstants.TILE_WIDTH, (Point2 - Point1).ToAngle());
				default:
					SAMLog.Error("LEWS::EnumSwitch_GA", "WallType = " + WallType);
					return default(FRotatedRectangle);
			}
		}

		public IFShape GetClickAreaP1()
		{
			return new FCircle(Point1, 24);
		}

		public IFShape GetClickAreaP2()
		{
			return new FCircle(Point2, 24);
		}

		public FRotatedRectangle GetArea()
		{
			switch (WallType)
			{
				case WallStubType.Void:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length(), VoidWallBlueprint.DEFAULT_WIDTH, (Point2 - Point1).ToAngle());
				case WallStubType.Glass:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length(), GlassBlockBlueprint.DEFAULT_WIDTH, (Point2 - Point1).ToAngle());
				case WallStubType.Mirror:
					return FRotatedRectangle.CreateByCenter(FPoint.MiddlePoint(Point1, Point2), (Point2 - Point1).Length(), MirrorBlockBlueprint.DEFAULT_WIDTH, (Point2 - Point1).ToAngle());
				default:
					SAMLog.Error("LEWS::EnumSwitch_GA", "WallType = " + WallType);
					return default(FRotatedRectangle);
			}
		}

		public bool IsClickP1(FPoint ptr)
		{
			return GDOwner.Selection == this && GetClickAreaP1().Contains(ptr) && (ptr - Point1).LengthSquared() < (ptr - Point2).LengthSquared();
		}

		public bool IsClickP2(FPoint ptr)
		{
			return GDOwner.Selection == this && GetClickAreaP2().Contains(ptr) && (ptr - Point2).LengthSquared() < (ptr - Point1).LengthSquared();
		}
	}
}
