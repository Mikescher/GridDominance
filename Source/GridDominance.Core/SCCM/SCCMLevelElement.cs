using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Screens.LevelEditorScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;

namespace GridDominance.Shared.SCCM
{
	public class SCCMLevelElement : BaseDataFile
	{
		public enum SCCMStubType{ Cannon=1, Obstacle=2, Portal=3, Wall=4 }

		public SCCMStubType StubType;

		public FPoint Cannon_Center;
		public float  Cannon_Scale;
		public float  Cannon_Rotation;
		public CannonStub.CannonStubType Cannon_CannonType;
		public CannonStub.CannonStubFraction Cannon_CannonFrac;

		public FPoint Obstacle_Center;
		public float Obstacle_Rotation;
		public float Obstacle_Width;
		public float Obstacle_Height;
		public float Obstacle_PowerFactor;
		public ObstacleStub.ObstacleStubType Obstacle_ObstacleType;

		public FPoint Portal_Center;
		public float  Portal_Normal;
		public float  Portal_Length;
		public int    Portal_Group;
		public bool   Portal_Side;

		public FPoint Wall_Point1;
		public FPoint Wall_Point2;
		public WallStub.WallStubType Wall_WallType;

		public bool NeedsSimulatedKI
		{
			get
			{
				if (StubType == SCCMStubType.Obstacle && Obstacle_ObstacleType == ObstacleStub.ObstacleStubType.BlackHole) return true;
				if (StubType == SCCMStubType.Obstacle && Obstacle_ObstacleType == ObstacleStub.ObstacleStubType.WhiteHole) return true;

				return false;
			}
		}

		protected override void Configure()
		{
			RegisterConstructor(() => new SCCMLevelElement());

			RegisterProperty<SCCMLevelElement, SCCMStubType>(SemVersion.VERSION_1_0_0, "stubtype", o => o.StubType, (o, v) => o.StubType = v);

			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "cannon_center", o => o.Cannon_Center, (o, v) => o.Cannon_Center = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "cannon_scale", o => o.Cannon_Scale, (o, v) => o.Cannon_Scale = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "cannon_rotation", o => o.Cannon_Rotation, (o, v) => o.Cannon_Rotation = v);
			RegisterProperty<SCCMLevelElement, CannonStub.CannonStubType>(SemVersion.VERSION_1_0_0, "cannon_ctype", o => o.Cannon_CannonType, (o, v) => o.Cannon_CannonType = v);
			RegisterProperty<SCCMLevelElement, CannonStub.CannonStubFraction>(SemVersion.VERSION_1_0_0, "cannon_cfrac", o => o.Cannon_CannonFrac, (o, v) => o.Cannon_CannonFrac = v);

			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_center", o => o.Obstacle_Center, (o, v) => o.Obstacle_Center = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_rotation", o => o.Obstacle_Rotation, (o, v) => o.Obstacle_Rotation = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_width", o => o.Obstacle_Width, (o, v) => o.Obstacle_Width = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_height", o => o.Obstacle_Height, (o, v) => o.Obstacle_Height = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_power", o => o.Obstacle_PowerFactor, (o, v) => o.Obstacle_PowerFactor = v);
			RegisterProperty<SCCMLevelElement, ObstacleStub.ObstacleStubType>(SemVersion.VERSION_1_0_0, "obstacle_otype", o => o.Obstacle_ObstacleType, (o, v) => o.Obstacle_ObstacleType = v);

			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_center", o => o.Portal_Center, (o, v) => o.Portal_Center = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_normal", o => o.Portal_Normal, (o, v) => o.Portal_Normal = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_length", o => o.Portal_Length, (o, v) => o.Portal_Length = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_group", o => o.Portal_Group, (o, v) => o.Portal_Group = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_side", o => o.Portal_Side, (o, v) => o.Portal_Side = v);

			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "wall_p1", o => o.Wall_Point1, (o, v) => o.Wall_Point1 = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "wall_p2", o => o.Wall_Point2, (o, v) => o.Wall_Point2 = v);
			RegisterProperty<SCCMLevelElement, WallStub.WallStubType>(SemVersion.VERSION_1_0_0, "wall_wtype", o => o.Wall_WallType, (o, v) => o.Wall_WallType = v);
		}

		protected override string GetTypeName()
		{
			return "SCCM_LEVEL_ELEMENT";
		}

		public void InsertIntoBlueprint(LevelBlueprint bp, ref byte cid)
		{
			switch (StubType)
			{
				case SCCMStubType.Cannon:
					InsertCannonIntoBlueprint(bp, ref cid);
					return;

				case SCCMStubType.Obstacle:
					InsertObstacleIntoBlueprint(bp);
					return;

				case SCCMStubType.Portal:
					InsertPortalIntoBlueprint(bp);
					return;

				case SCCMStubType.Wall:
					InsertWallIntoBlueprint(bp);
					return;

				default:
					SAMLog.Error("ABTA::IIBP", "StubType: " + StubType);
					return;
			}
		}

		private void InsertWallIntoBlueprint(LevelBlueprint bp)
		{
			var center = FPoint.MiddlePoint(Wall_Point1, Wall_Point2);
			var length = (Wall_Point2 - Wall_Point1).Length();
			var degrot = (Wall_Point2 - Wall_Point1).ToDegAngle();

			switch (Wall_WallType)
			{
				case WallStub.WallStubType.Void:
					bp.BlueprintVoidWalls.Add(new VoidWallBlueprint(center.X, center.Y, length, degrot));
					break;

				case WallStub.WallStubType.Glass:
					bp.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(center.X, center.Y, length, GlassBlockBlueprint.DEFAULT_WIDTH, degrot));
					break;

				case WallStub.WallStubType.Mirror:
					bp.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(center.X, center.Y, length, GlassBlockBlueprint.DEFAULT_WIDTH, degrot));
					break;

				default:
					SAMLog.Error("ABTA::IWIBP", "Wall_WallType: " + Wall_WallType);
					return;
			}
		}

		private void InsertPortalIntoBlueprint(LevelBlueprint bp)
		{
			bp.BlueprintPortals.Add(new PortalBlueprint(Portal_Center.X, Portal_Center.Y, Portal_Length, Portal_Normal * FloatMath.RadToDeg, (short)Portal_Group, Portal_Side));
		}

		private void InsertObstacleIntoBlueprint(LevelBlueprint bp)
		{
			var abspower = Obstacle_Width * BlackHoleBlueprint.DEFAULT_POWER_FACTOR * Obstacle_PowerFactor;

			switch (Obstacle_ObstacleType)
			{
				case ObstacleStub.ObstacleStubType.BlackHole:
					bp.BlueprintBlackHoles.Add(new BlackHoleBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width, -abspower));
					break;

				case ObstacleStub.ObstacleStubType.WhiteHole:
					bp.BlueprintBlackHoles.Add(new BlackHoleBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width, +abspower));
					break;

				case ObstacleStub.ObstacleStubType.GlassBlock:
					bp.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width, Obstacle_Height, Obstacle_Rotation * FloatMath.RadToDeg));
					break;

				case ObstacleStub.ObstacleStubType.MirrorBlock:
					bp.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width, Obstacle_Height, Obstacle_Rotation * FloatMath.RadToDeg));
					break;

				case ObstacleStub.ObstacleStubType.MirrorCircle:
					bp.BlueprintMirrorCircles.Add(new MirrorCircleBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width));
					break;

				case ObstacleStub.ObstacleStubType.VoidVircle:
					bp.BlueprintVoidCircles.Add(new VoidCircleBlueprint(Obstacle_Center.X, Obstacle_Center.Y, Obstacle_Width));
					break;

				default:
					SAMLog.Error("ABTA::IOIB", "Obstacle_ObstacleType: " + Obstacle_ObstacleType);
					return;
			}
		}

		private void InsertCannonIntoBlueprint(LevelBlueprint bp, ref byte cid)
		{
			switch (Cannon_CannonType)
			{
				case CannonStub.CannonStubType.Bullet:
					bp.BlueprintCannons.Add(new CannonBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				case CannonStub.CannonStubType.Laser:
					bp.BlueprintLaserCannons.Add(new LaserCannonBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				case CannonStub.CannonStubType.Minigun:
					bp.BlueprintMinigun.Add(new MinigunBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				case CannonStub.CannonStubType.Relay:
					bp.BlueprintRelayCannon.Add(new RelayCannonBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				case CannonStub.CannonStubType.Shield:
					bp.BlueprintShieldProjector.Add(new ShieldProjectorBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				case CannonStub.CannonStubType.Trishot:
					bp.BlueprintTrishotCannon.Add(new TrishotCannonBlueprint(Cannon_Center.X, Cannon_Center.Y, Cannon_Scale * Cannon.CANNON_DIAMETER, (int)Cannon_CannonFrac, Cannon_Rotation * FloatMath.RadToDeg, cid));
					cid++;
					break;

				default:
					SAMLog.Error("ABTA::ICIB", "Cannon_CannonType: " + Cannon_CannonType);
					return;
			}
		}
	}
}