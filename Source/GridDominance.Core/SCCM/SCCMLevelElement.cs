using GridDominance.Shared.Screens.Leveleditor.Entities;
using MonoSAMFramework.Portable.GameMath.Geometry;
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
		public float Obstacle_Power;
		public ObstacleStub.ObstacleStubType Obstacle_ObstacleType;

		public FPoint Portal_Center;
		public float  Portal_Rotation;
		public float  Portal_Length;
		public int    Portal_Group;
		public bool   Portal_Side;

		public FPoint Wall_Point1;
		public FPoint Wall_Point2;
		public WallStub.WallStubType Wall_WallType;

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
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "obstacle_power", o => o.Obstacle_Power, (o, v) => o.Obstacle_Power = v);
			RegisterProperty<SCCMLevelElement, ObstacleStub.ObstacleStubType>(SemVersion.VERSION_1_0_0, "obstacle_otype", o => o.Obstacle_ObstacleType, (o, v) => o.Obstacle_ObstacleType = v);

			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_center", o => o.Portal_Center, (o, v) => o.Portal_Center = v);
			RegisterProperty<SCCMLevelElement>(SemVersion.VERSION_1_0_0, "portal_rotation", o => o.Portal_Rotation, (o, v) => o.Portal_Rotation = v);
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
	}
}