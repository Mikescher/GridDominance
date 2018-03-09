using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.LevelEditorScreen;
using GridDominance.Shared.Screens.LevelEditorScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen;
using GridDominance.Shared.SCCM.PreCalculation;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Persistance;
using MonoSAMFramework.Portable.Persistance.DataFile;
using MonoSAMFramework.Portable.Screens.HUD;

namespace GridDominance.Shared.SCCM
{
	public class SCCMLevelData : RootDataFile
	{
		public const int MaxNameLength  = 32;
		public const int MaxEntityCount = 99;

		public static readonly DSize[] SIZES = {new DSize(16, 10), new DSize(24, 15), new DSize(24, 10), new DSize(16, 20), new DSize(32, 20)};

		protected override SemVersion ArchiveVersion => SemVersion.VERSION_1_0_0;

		public DateTime LastChanged;

		public int Music = -1;
		public int AuthorUserID = -1;
		public Int64 OnlineID = -1;
		public string Name = "";
		public DSize Size = SIZES[0];
		public FlatAlign9 View = FlatAlign9.CENTER;
		public GameWrapMode Geometry = GameWrapMode.Death;

		public List<SCCMLevelElement> Elements = new List<SCCMLevelElement>();

		public int Width  => Size.Width;
		public int Height => Size.Height;
		public string Filename         => "CUSTOMLEVELDATA_{B16B00B5-0001-4001-0000-" + $"{OnlineID:X12}" + "}";
		public string FilenameUploaded => "UPLOADEDLEVELDATA_{B16B00B5-0001-4001-0000-" + $"{OnlineID:X12}" + "}";

		public SCCMLevelData(Int64 id, int authorid)
		{
			LastChanged = DateTime.UtcNow;
			OnlineID = id;
			AuthorUserID = authorid;
		}

		public SCCMLevelData() { }

		protected override void Configure()
		{
			RegisterConstructor(() => new SCCMLevelData());

			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "music",       o => o.Music,        (o, v) => o.Music        = v);
			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "lastChanged", o => o.LastChanged,  (o, v) => o.LastChanged  = v);
			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "online_id",   o => o.OnlineID,     (o, v) => o.OnlineID     = v);
			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "level_name",  o => o.Name,         (o, v) => o.Name         = v);
			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "size",        o => o.Size,         (o, v) => o.Size         = v);
			RegisterProperty<SCCMLevelData, FlatAlign9>(  SemVersion.VERSION_1_0_0, "view",        o => o.View,         (o, v) => o.View         = v);
			RegisterProperty<SCCMLevelData, GameWrapMode>(SemVersion.VERSION_1_0_0, "geometry",    o => o.Geometry,     (o, v) => o.Geometry     = v);
			RegisterProperty<SCCMLevelData>(              SemVersion.VERSION_1_0_0, "author",      o => o.AuthorUserID, (o, v) => o.AuthorUserID = v);

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
						SAMLog.Error("SCCMLD::EnumSwitch_ATLE", "e.StubType: " + e.StubType);
						break;
				}
			}
		}

		public bool UpdateAndSave(LevelEditorScreen scrn)
		{
			Update(scrn);

			return SaveToDisk();
		}

		private void Update(LevelEditorScreen scrn)
		{
			Elements.Clear();

			LastChanged = DateTime.UtcNow;

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

						Portal_Center = portalStub.Center,
						Portal_Normal = portalStub.Normal,
						Portal_Length = portalStub.Length,
						Portal_Group  = portalStub.Group,
						Portal_Side   = portalStub.Side
					});
				}
				else if (stub is ObstacleStub obstacleStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Obstacle,

						Obstacle_Center       = obstacleStub.Center,
						Obstacle_Rotation     = obstacleStub.Rotation,
						Obstacle_Width        = obstacleStub.Width,
						Obstacle_Height       = obstacleStub.Height,
						Obstacle_PowerFactor  = obstacleStub.Power,
						Obstacle_ObstacleType = obstacleStub.ObstacleType
					});
				}
				else if (stub is WallStub wallStub)
				{
					Elements.Add(new SCCMLevelElement
					{
						StubType = SCCMLevelElement.SCCMStubType.Wall,

						Wall_Point1   = wallStub.Point1,
						Wall_Point2   = wallStub.Point2,
						Wall_WallType = wallStub.WallType,
					});
				}
				else
				{
					SAMLog.Error("SCCMLD::EnumSwitch_UAS", "typeof(stub): " + stub?.GetType());
				}
			}
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

		public void Delete()
		{
			FileHelper.Inst.DeleteDataIfExist(Filename);
		}

		public bool ValidateWithToasts(GameHUD hud)
		{
			if (Elements.Count > MaxEntityCount)
			{
				hud.ShowToast("SCCMLD::VWT_1", L10N.T(L10NImpl.STR_LVLED_ERR_TOOMANYENTS), 64, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				return false;
			}

			if (string.IsNullOrWhiteSpace(Name))
			{
				hud.ShowToast("SCCMLD::VWT_2", L10N.T(L10NImpl.STR_LVLED_ERR_NONAME), 64, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				return false;
			}

			var hasPlayer = Elements
				.Where(e => e.StubType == SCCMLevelElement.SCCMStubType.Cannon)
				.Where(e => e.Cannon_CannonType != CannonStub.CannonStubType.Relay)
				.Where(e => e.Cannon_CannonType != CannonStub.CannonStubType.Shield)
				.Any(e => e.Cannon_CannonFrac == CannonStub.CannonStubFraction.P1);

			var hasEnemy = Elements
				.Where(e => e.StubType == SCCMLevelElement.SCCMStubType.Cannon)
				.Where(e => e.Cannon_CannonType != CannonStub.CannonStubType.Relay)
				.Where(e => e.Cannon_CannonType != CannonStub.CannonStubType.Shield)
				.Any(e => e.Cannon_CannonFrac == CannonStub.CannonStubFraction.A2 || e.Cannon_CannonFrac == CannonStub.CannonStubFraction.A3 || e.Cannon_CannonFrac == CannonStub.CannonStubFraction.A4);

			if (!hasPlayer)
			{
				hud.ShowToast("SCCMLD::VWT_3", L10N.T(L10NImpl.STR_LVLED_ERR_NOPLAYER), 64, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				return false;
			}

			if (!hasEnemy)
			{
				hud.ShowToast("SCCMLD::VWT_4", L10N.T(L10NImpl.STR_LVLED_ERR_NOENEMY), 64, FlatColors.Flamingo, FlatColors.Foreground, 3f);
				return false;
			}

			return true;
		}

		public LevelBlueprint CompileToBlueprint(GameHUD hud)
		{
			var bp = new LevelBlueprint();

			bp.UniqueID    = Guid.Parse($"b16b00b5-0001-4001-0000-{OnlineID:000000000000}");
			bp.WrapMode    = (byte)Geometry;
			bp.LevelWidth  = Width * 64;
			bp.LevelHeight = Height * 64;
			bp.LevelViewX  = CalculateViewCenter().X;
			bp.LevelViewY  = CalculateViewCenter().Y;
			bp.Name        = "0-" + OnlineID;
			bp.FullName    = Name;
			bp.KIType      = GetKIType();
			bp.CustomMusic = Music;

			byte cannonID = 0;

			foreach (var e in Elements)
			{
				e.InsertIntoBlueprint(bp, ref cannonID);
			}

			try
			{
				bp.ValidOrThrow();

				BlueprintPreprocessor.ProcessLevel(bp);

				return bp;
			}
			catch (Exception e)
			{
				MonoSAMGame.CurrentInst.DispatchBeginInvoke(() =>
				{
					hud.ShowToast("SCCMLD::CTB_1", L10N.T(L10NImpl.STR_LVLED_ERR_COMPILERERR), 64, FlatColors.Flamingo, FlatColors.Foreground, 3f);
					SAMLog.Error("SCMMLD_CTBERR", "LevelBlueprint compiled to invalid", $"Exception: {e}\n\n\nData: {SerializeToString()}");
				});
				return null;
			}
		}

		private byte GetKIType()
		{
			byte ki = LevelBlueprint.KI_TYPE_PRECALC;

			if (Elements.Any(e => e.NeedsSimulatedKI)) ki = LevelBlueprint.KI_TYPE_PRESIMULATE;

			return ki;
		}

		private FPoint CalculateViewCenter()
		{
			var size64 = Size * 64f;

			if (Size == SIZES[0]) return size64.CenterPoint;

			switch (View)
			{
				case FlatAlign9.TOP:
					return new FPoint(size64.Width / 2, 5 * 64);

				case FlatAlign9.TOPRIGHT:
					return new FPoint(size64.Width - 8 * 64, 5 * 64);

				case FlatAlign9.RIGHT:
					return new FPoint(size64.Width - 8 * 64, size64.Height / 2f);

				case FlatAlign9.BOTTOMRIGHT:
					return new FPoint(size64.Width - 8 * 64, size64.Height - 5 * 64);

				case FlatAlign9.BOTTOM:
					return new FPoint(size64.Width/2, size64.Height - 5 * 64);

				case FlatAlign9.BOTTOMLEFT:
					return new FPoint(8 * 64, size64.Height - 5 * 64);

				case FlatAlign9.LEFT:
					return new FPoint(8 * 64, size64.Height/2);

				case FlatAlign9.TOPLEFT:
					return new FPoint(8 * 64, 5 * 64);

				case FlatAlign9.CENTER:
					return size64.CenterPoint;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
