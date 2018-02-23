using System;
using System.Collections.Generic;
using System.IO;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Leveleditor;
using GridDominance.Shared.Screens.Leveleditor.Entities;
using GridDominance.Shared.Screens.NormalGameScreen;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;

namespace GridDominance.Shared.SCCM
{
	public class SCCMLevelData : RootDataFile
	{
		public const int MaxNameLength = 32;

		public static readonly DSize[] SIZES = {new DSize(16, 10), new DSize(24, 15), new DSize(24, 10), new DSize(16, 20), new DSize(32, 20)};

		protected override SemVersion ArchiveVersion => SemVersion.VERSION_1_0_0;

		public DateTime LastChanged;

		public Int64 OnlineID = -1;
		public string Name = "";
		public DSize Size = SIZES[0];
		public FlatAlign9 View = FlatAlign9.CENTER;
		public GameWrapMode Geometry = GameWrapMode.Death;

		public List<SCCMLevelElement> Elements = new List<SCCMLevelElement>();

		public int Width  => Size.Width;
		public int Height => Size.Height;
		public string Filename => "CUSTOMLEVELDATA_{c27c11c6-0001-4001-0000-" + $"{OnlineID:000000000000}" + "}";

		public SCCMLevelData(int id)
		{
			LastChanged = DateTime.UtcNow;
			OnlineID = id;
		}

		public SCCMLevelData()
		{

		}

		protected override void Configure()
		{
			RegisterConstructor(() => new SCCMLevelData());

			RegisterProperty<SCCMLevelData>(SemVersion.VERSION_1_0_0,               "lastChanged", o => o.LastChanged, (o, v) => o.LastChanged = v);
			RegisterProperty<SCCMLevelData>(SemVersion.VERSION_1_0_0,               "online_id",   o => o.OnlineID,    (o, v) => o.OnlineID    = v);
			RegisterProperty<SCCMLevelData>(SemVersion.VERSION_1_0_0,               "level_name",  o => o.Name,        (o, v) => o.Name        = v);
			RegisterProperty<SCCMLevelData>(SemVersion.VERSION_1_0_0,               "size",        o => o.Size,        (o, v) => o.Size        = v);
			RegisterProperty<SCCMLevelData, FlatAlign9>(SemVersion.VERSION_1_0_0,   "view",        o => o.View,        (o, v) => o.View        = v);
			RegisterProperty<SCCMLevelData, GameWrapMode>(SemVersion.VERSION_1_0_0, "geometry",    o => o.Geometry,    (o, v) => o.Geometry    = v);

			RegisterPropertyList<SCCMLevelData, SCCMLevelElement>(SemVersion.VERSION_1_0_0, "elements", () => new SCCMLevelElement(), o => o.Elements, (o, v) => o.Elements = v);
		}

		public void ApplyToLevelEditor(LevelEditorScreen scrn)
		{
			foreach (var old in scrn.GetEntities<ILeveleditorStub>()) old.Kill();

			foreach (var e in Elements)
			{
				switch (e.StubType)
				{
					case SCCMLevelElement.SCCMStubType.Cannon:
						scrn.Entities.AddEntity(new CannonStub(scrn, e));
						break;
					case SCCMLevelElement.SCCMStubType.Obstacle:
						scrn.Entities.AddEntity(new ObstacleStub(scrn, e));
						break;
					case SCCMLevelElement.SCCMStubType.Portal:
						scrn.Entities.AddEntity(new PortalStub(scrn, e));
						break;
					case SCCMLevelElement.SCCMStubType.Wall:
						scrn.Entities.AddEntity(new WallStub(scrn, e));
						break;
					default: 
						SAMLog.Warning("SCCMLD::EnumSwitch_ATLE", "e.StubType: " + e.StubType);
						break;
				}
			}
		}

		public bool UpdateAndSave(LevelEditorScreen scrn)
		{
			Elements.Clear();

			foreach (var stub in scrn.GetEntities<ILeveleditorStub>())
			{
				if (stub is CannonStub cannonStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Cannon,

						Cannon_Center     = cannonStub.Center,
						Cannon_Scale      = cannonStub.Scale,
						Cannon_Rotation   = cannonStub.Rotation,
						Cannon_CannonType = cannonStub.CannonType,
						Cannon_CannonFrac = cannonStub.CannonFrac,
					});
				}
				else if (stub is PortalStub portalStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Portal,

						Portal_Center   = portalStub.Center,
						Portal_Rotation = portalStub.Rotation,
						Portal_Length   = portalStub.Length,
						Portal_Group    = portalStub.Group,
						Portal_Side     = portalStub.Side
					});
				}
				else if (stub is ObstacleStub obstacleStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Portal,

						Obstacle_Center       = obstacleStub.Center,
						Obstacle_Rotation     = obstacleStub.Rotation,
						Obstacle_Width        = obstacleStub.Width,
						Obstacle_Height       = obstacleStub.Height,
						Obstacle_Power        = obstacleStub.Power,
						Obstacle_ObstacleType = obstacleStub.ObstacleType
					});
				}
				else if (stub is WallStub wallStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Portal,

						Wall_Point1 = wallStub.Point1,
						Wall_Point2 = wallStub.Point2,
						Wall_WallType = wallStub.WallType,
					});
				}
				else
				{
					SAMLog.Warning("SCCMLD::EnumSwitch_UAS", "typeof(stub): " + stub?.GetType());
				}
			}

			return SaveToDisk();
		}

		private bool SaveToDisk()
		{
			var sdata = SerializeToString();

			try
			{
				FileHelper.Inst.WriteData(Filename, sdata);
			}
			catch (IOException e)
			{
				if (e.Message.Contains("Disk full"))
				{
					MainGame.Inst.DispatchBeginInvoke(() =>
					{
						MainGame.Inst.ShowToast("SCCMLD::OOM", L10N.T(L10NImpl.STR_ERR_OUTOFMEMORY), 32, FlatColors.Flamingo, FlatColors.Foreground, 3f);
					});
					return false;
				}
				else
				{
					SAMLog.Error("SCCMLD::WRITE", e);
					return false;
				}
			}

#if DEBUG
			SAMLog.Debug($"SCCM saved ({sdata.Length})");

			try
			{
				var p = new SCCMLevelData();
				p.DeserializeFromString(sdata);
				var sdata2 = p.SerializeToString();

				if (sdata2 != sdata)
				{
					SAMLog.Error("SCCMLD:Serialization_mismatch", "Serialization test mismatch", $"Data_1:\n{sdata}\n\n----------------\n\nData_2:\n{sdata2}");
				}
			}
			catch (Exception e)
			{
				SAMLog.Error("SCCMLD:Serialization-Ex", "Serialization test mismatch", e.ToString());
			}
#endif

			return true;
		}

		protected override string GetTypeName()
		{
			return "SCCM_LEVEL_DATA";
		}
	}
}
