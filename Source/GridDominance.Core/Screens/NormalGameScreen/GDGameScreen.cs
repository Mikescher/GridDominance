using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Network.Multiplayer;
using GridDominance.Shared.Resources;
using GridDominance.Shared.SaveData;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Background;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Obstacles;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Particles;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork;
using GridDominance.Shared.Screens.NormalGameScreen.Operations;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Background;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.ViewportAdapters;

namespace GridDominance.Shared.Screens.NormalGameScreen
{
	public abstract class GDGameScreen : GameScreen
	{
		public const float GAMESPEED_SUPERSLOW = 0.25f;
		public const float GAMESPEED_SLOW      = 0.5f;
		public const float GAMESPEED_NORMAL    = 1f;
		public const float GAMESPEED_SEMIFAST  = 1.5f;
		public const float GAMESPEED_FAST      = 2f;
		public const float GAMESPEED_SUPERFAST = 4f;

		public const int MAX_BULLET_ID = 1 << 12; // 12bit = [0..4095]

		//-----------------------------------------------------------------

		public MainGame GDOwner => (MainGame)Game;
		public IGDGridBackground GDBackground => (IGDGridBackground) Background;
		public GDEntityManager GDEntities => (GDEntityManager)Entities;

		//-----------------------------------------------------------------

		protected override EntityManager CreateEntityManager() => new GDEntityManager(this);
		protected override GameBackground CreateBackground() => new SolidColorBackground(this, Color.Gainsboro);

		protected override SAMViewportAdapter CreateViewport() => new TolerantBoxingViewportAdapter(Game.Window, Graphics, GDConstants.VIEW_WIDTH, GDConstants.VIEW_HEIGHT);
		protected override DebugMinimap CreateDebugMinimap() => new StandardDebugMinimapImplementation(this, 192, 32);
		protected override FRectangle CreateMapFullBounds() => new FRectangle(0, 0, 1, 1);
		protected override float GetBaseTextureScale() => Textures.DEFAULT_TEXTURE_SCALE_F;

		//-----------------------------------------------------------------

		public bool CanPause = true;

		private bool _isPaused = false;
		public bool IsPaused
		{
			get { return _isPaused; }
			set
			{
				if (value == _isPaused) return;
				_isPaused = value;
				UpdateGameSpeed();
			}			
		}

		private GameSpeedModes _gameSpeedMode = GameSpeedModes.NORMAL;
		public GameSpeedModes GameSpeedMode
		{
			get { return _gameSpeedMode; }
			set
			{
				if (value == _gameSpeedMode) return;
				_gameSpeedMode = value;
				UpdateGameSpeed();
			}
		}

		protected Fraction fractionNeutral;
		protected Fraction fractionPlayer;
		protected Fraction fractionComputer1;
		protected Fraction fractionComputer2;
		protected Fraction fractionComputer3;
		private Fraction[] fractionIDList;

		public readonly LevelBlueprint Blueprint;
		public readonly FractionDifficulty Difficulty;
		public readonly LaserNetwork.LaserNetwork LaserNetwork;
		public          GameWrapMode WrapMode;
		public          Dictionary<byte, Cannon> CannonMap;
		public readonly RemoteBulletHint[] BulletMapping = new RemoteBulletHint[MAX_BULLET_ID];
		public readonly RemoteBullet[] RemoteBulletMapping = new RemoteBullet[MAX_BULLET_ID];
		public ushort lastBulletID = 0;

		public bool HasFinished = false;
		public bool PlayerWon = false; // [P] win or [C] win
		public float LevelTime = 0f;
		public bool IsCountdown = false;

		public readonly bool IsPreview;
		public readonly bool IsNetwork;

		private readonly bool _superslow;

		public abstract Fraction LocalPlayerFraction { get; }

		protected GDGameScreen(MainGame game, GraphicsDeviceManager gdm, LevelBlueprint bp, FractionDifficulty diff, bool prev, bool netw, bool slow)
			: base(game, gdm)
		{
			Blueprint = bp;
			Difficulty = diff;
			IsPreview = prev;
			IsNetwork = netw;

			_superslow = slow;

			LaserNetwork = new LaserNetwork.LaserNetwork(GetPhysicsWorld(), this, (GameWrapMode)bp.WrapMode);

			Initialize();
		}

		public void PositionTo2Byte(FPoint pos, out ushort x, out ushort y)
		{
			var xmin = MapFullBounds.Left - MapFullBounds.Width / 2f;
			var xmax = MapFullBounds.Right + MapFullBounds.Width / 2f;
			var ymin = MapFullBounds.Top - MapFullBounds.Height / 2f;
			var ymax = MapFullBounds.Bottom + MapFullBounds.Height / 2f;

			x = (ushort)FloatMath.IClamp(FloatMath.Round(((pos.X - xmin) / (xmax - xmin)) * 65535), 0, 65535);
			y = (ushort)FloatMath.IClamp(FloatMath.Round(((pos.Y - ymin) / (ymax - ymin)) * 65535), 0, 65535);
		}

		public void DoubleByteToPosition(ushort bx, ushort by, out float px, out float py)
		{
			var xmin = MapFullBounds.Left - MapFullBounds.Width / 2f;
			var xmax = MapFullBounds.Right + MapFullBounds.Width / 2f;
			var ymin = MapFullBounds.Top - MapFullBounds.Height / 2f;
			var ymax = MapFullBounds.Bottom + MapFullBounds.Height / 2f;

			px = xmin + (bx / 65535f) * (xmax - xmin);
			py = ymin + (by / 65535f) * (ymax - ymin);
		}

#if DEBUG
		public Fraction GetPlayerFraction() // Only DEBUG - later there can be games w/o PlayerFraction
		{
			return fractionPlayer;
		}
		public Fraction GetComputerFraction() // Only DEBUG
		{
			return fractionComputer1;
		}
#endif

		public byte GetFractionID(Fraction f)
		{
			for (byte i = 0; i < fractionIDList.Length; i++)
			{
				if (fractionIDList[i] == f) return i;
			}

			SAMLog.Error("GDGS::GetFractionID", $"Fraction not found: {f}");
			return 0;
		}

		public Fraction GetFractionByID(byte id, out bool err)
		{
			if (id >= 0 && id < fractionIDList.Length) {err = false; return fractionIDList[id];}

			err = true;
			return fractionNeutral;
		}

		public Fraction GetFractionByID(byte id)
		{
			if (id >= 0 && id < fractionIDList.Length) return fractionIDList[id];

			SAMLog.Error("GDGS::GetFractionByID", $"Fraction not found: {id}");
			return fractionNeutral;
		}

		public Fraction GetNeutralFraction()
		{
			return fractionNeutral;
		}

		public ushort AssignBulletID(Bullet bullet)
		{
			lastBulletID = (ushort)((lastBulletID + 1) % MAX_BULLET_ID);

			if (!IsNetwork) return lastBulletID;

			for (ushort i = 0; i < MAX_BULLET_ID; i++)
			{
				var ti = (ushort)((lastBulletID + i) % MAX_BULLET_ID);
				if (BulletMapping[ti].Bullet == null || (BulletMapping[ti].State != RemoteBullet.RemoteBulletState.Normal && BulletMapping[ti].RemainingPostDeathTransmitions <= 0))
				{
					BulletMapping[ti].Bullet = bullet;
					BulletMapping[ti].State = RemoteBullet.RemoteBulletState.Normal;
					BulletMapping[ti].RemainingPostDeathTransmitions = RemoteBullet.POST_DEATH_TRANSMITIONCOUNT;

					lastBulletID = ti;
					return ti;
				}
			}

			SAMLog.Info("GDGS::TMB1", "Too many bullets+artifacts, no free fully-dead BulletID");

			for (ushort i = 0; i < MAX_BULLET_ID; i++)
			{
				var ti = (ushort)((lastBulletID + i) % MAX_BULLET_ID);
				if (BulletMapping[ti].Bullet == null || BulletMapping[ti].State != RemoteBullet.RemoteBulletState.Normal)
				{
					BulletMapping[ti].Bullet = bullet;
					BulletMapping[ti].State = RemoteBullet.RemoteBulletState.Normal;
					BulletMapping[ti].RemainingPostDeathTransmitions = RemoteBullet.POST_DEATH_TRANSMITIONCOUNT;

					lastBulletID = ti;
					return ti;
				}
			}

			SAMLog.Error("GDGS::TMB2", "Too many bullets, no free BulletID");
			BulletMapping[lastBulletID].Bullet = bullet;
			BulletMapping[lastBulletID].State = RemoteBullet.RemoteBulletState.Normal;
			BulletMapping[lastBulletID].RemainingPostDeathTransmitions = RemoteBullet.POST_DEATH_TRANSMITIONCOUNT;

			return lastBulletID;
		}
		
		public void UnassignBulletID(ushort id, Bullet bullet)
		{
			if (!IsNetwork) return;
			
			if (BulletMapping[id].Bullet == bullet)
			{
				if (BulletMapping[id].State == RemoteBullet.RemoteBulletState.Normal)
				{
					BulletMapping[id].State = RemoteBullet.RemoteBulletState.Dying_Instant;
					BulletMapping[id].RemainingPostDeathTransmitions = RemoteBullet.POST_DEATH_TRANSMITIONCOUNT;
				}
			}
		}

		public void ChangeBulletMapping(RemoteBullet.RemoteBulletState state, ushort id, Bullet bullet)
		{
			if (!IsNetwork) return;
			
			if (BulletMapping[id].Bullet == bullet)
			{
				if (BulletMapping[id].State == RemoteBullet.RemoteBulletState.Normal)
				{
					BulletMapping[id].State = state;
					BulletMapping[id].RemainingPostDeathTransmitions = RemoteBullet.POST_DEATH_TRANSMITIONCOUNT;
				}
			}
		}

		private void Initialize()
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(GDConstants.PHYSICS_CONVERSION_FACTOR);

#if DEBUG
			if (!IsPreview)
			{
				DebugUtils.CreateShortcuts(this);
				DebugDisp = DebugUtils.CreateDisplay(this);
			}
#endif

			//--------------------

			fractionNeutral = Fraction.CreateNeutralFraction();
			fractionPlayer = Fraction.CreatePlayerFraction(fractionNeutral);
			fractionComputer1 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_01, fractionNeutral, Difficulty, _superslow);
			fractionComputer2 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_02, fractionNeutral, Difficulty, _superslow);
			fractionComputer3 = Fraction.CreateComputerFraction(Fraction.COLOR_COMPUTER_03, fractionNeutral, Difficulty, _superslow);

			LoadLevelFromBlueprint();
		}

		private void LoadLevelFromBlueprint()
		{
			Fraction[] fracList = { fractionNeutral, fractionPlayer, fractionComputer1, fractionComputer2, fractionComputer3 };

			//----------------------------------------------------------------

			MapFullBounds = new FRectangle(0, 0, Blueprint.LevelWidth, Blueprint.LevelHeight);
			MapViewportCenterX = Blueprint.LevelViewX;
			MapViewportCenterY = Blueprint.LevelViewY;
			WrapMode = (GameWrapMode)Blueprint.WrapMode;

			if (WrapMode == GameWrapMode.Donut || WrapMode == GameWrapMode.Reflect)
			{
				VAdapterGame.ChangeVirtualSize(GDConstants.VIEW_WIDTH + GDConstants.TILE_WIDTH, GDConstants.VIEW_HEIGHT + GDConstants.TILE_WIDTH);
				MapViewportCenterX = Blueprint.LevelViewX;
				MapViewportCenterY = Blueprint.LevelViewY;
			}

			if (MainGame.Inst.Profile.EffectsEnabled && WrapMode == GameWrapMode.Donut)
			{
				Entities.AddEntity(new DonutParticleEmitter(this, Blueprint, FlatAlign4.NN));
				Entities.AddEntity(new DonutParticleEmitter(this, Blueprint, FlatAlign4.EE));
				Entities.AddEntity(new DonutParticleEmitter(this, Blueprint, FlatAlign4.SS));
				Entities.AddEntity(new DonutParticleEmitter(this, Blueprint, FlatAlign4.WW));
			}

			if (MainGame.Inst.Profile.EffectsEnabled)
				Background = new GDCellularBackground(this, Blueprint);
			else
				Background = new GDStaticGridBackground(this, WrapMode);

			//----------------------------------------------------------------

			var cannonList = new List<Cannon>();
			var portalList = new List<Portal>();
			var fractionList = new List<Fraction>();
			var laserworld = false;

			fractionList.Add(fractionNeutral);

			foreach (var bPrint in Blueprint.BlueprintCannons)
			{
				var e = new BulletCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);
				
				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintMinigun)
			{
				var e = new MinigunCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);

				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintRelayCannon)
			{
				var e = new RelayCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);

				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintTrishotCannon)
			{
				var e = new TrishotCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);

				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintVoidWalls)
			{
				var e = new VoidWall(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintVoidCircles)
			{
				var e = new VoidCircle(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintGlassBlocks)
			{
				var e = new GlassBlock(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintBlackHoles)
			{
				var e = new BlackHole(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintPortals)
			{
				var e = new Portal(this, bPrint);
				Entities.AddEntity(e);
				portalList.Add(e);
			}

			foreach (var bPrint in Blueprint.BlueprintLaserCannons)
			{
				var e = new LaserCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);
				laserworld = true;

				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintShieldProjector)
			{
				var e = new ShieldProjectorCannon(this, bPrint, fracList);
				Entities.AddEntity(e);
				cannonList.Add(e);
				laserworld = true;

				if (!fractionList.Contains(e.Fraction)) fractionList.Add(e.Fraction);
			}

			foreach (var bPrint in Blueprint.BlueprintMirrorBlocks)
			{
				var e = new MirrorBlock(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintMirrorCircles)
			{
				var e = new MirrorCircle(this, bPrint);
				Entities.AddEntity(e);
			}

			foreach (var bPrint in Blueprint.BlueprintBackgroundText)
			{
				var e = new BackgroundText(this, bPrint);
				Entities.AddEntity(e);
			}

			//----------------------------------------------------------------

			if (laserworld) Entities.AddEntity(new LaserRenderer(this, LaserNetwork, Blueprint));
			
			AddEdgeMarker();

			//----------------------------------------------------------------

			foreach (var cannon in cannonList)
				cannon.OnAfterLevelLoad();


			foreach (var portal in portalList)
				portal.OnAfterLevelLoad(portalList);

			CannonMap = cannonList.ToDictionary(p => p.BlueprintCannonID, p => p);

			foreach (var f in fracList) if (!fractionList.Contains(f)) fractionList.Add(f);
			fractionIDList = fractionList.ToArray();
			
			//----------------------------------------------------------------

			if (!IsPreview && (Blueprint.LevelWidth > GDConstants.VIEW_WIDTH || Blueprint.LevelHeight > GDConstants.VIEW_HEIGHT) ) AddAgent(new GameDragAgent());
		}

		private void AddEdgeMarker()
		{
			var mw = MapFullBounds.Width;
			var mh = MapFullBounds.Height;
			var ex = 2 * GDConstants.TILE_WIDTH;

			var rn = new FRectangle(-ex, -ex, mw + 2 * ex, ex);
			var re = new FRectangle(+mw, -ex, ex,          mh + 2 * ex);
			var rs = new FRectangle(-ex, +mh, mw + 2 * ex, ex);
			var rw = new FRectangle(-ex, -ex, ex,          mh + 2 * ex);

			var dn = new MarkerCollisionBorder { Side = FlatAlign4.NN };
			var de = new MarkerCollisionBorder { Side = FlatAlign4.EE };
			var ds = new MarkerCollisionBorder { Side = FlatAlign4.SS };
			var dw = new MarkerCollisionBorder { Side = FlatAlign4.WW };

			var bn = BodyFactory.CreateBody(GetPhysicsWorld(), ConvertUnits2.ToSimUnits(rn.Center), 0, BodyType.Static, dn);
			var be = BodyFactory.CreateBody(GetPhysicsWorld(), ConvertUnits2.ToSimUnits(re.Center), 0, BodyType.Static, de);
			var bs = BodyFactory.CreateBody(GetPhysicsWorld(), ConvertUnits2.ToSimUnits(rs.Center), 0, BodyType.Static, ds);
			var bw = BodyFactory.CreateBody(GetPhysicsWorld(), ConvertUnits2.ToSimUnits(rw.Center), 0, BodyType.Static, dw);

			var fn = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rn.Width), ConvertUnits.ToSimUnits(rn.Height), 1, Vector2.Zero, bn, dn);
			var fe = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(re.Width), ConvertUnits.ToSimUnits(re.Height), 1, Vector2.Zero, be, de);
			var fs = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rs.Width), ConvertUnits.ToSimUnits(rs.Height), 1, Vector2.Zero, bs, ds);
			var fw = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(rw.Width), ConvertUnits.ToSimUnits(rw.Height), 1, Vector2.Zero, bw, dw);
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
#if DEBUG
			DebugDisp.IsEnabled = DebugSettings.Get("DebugTextDisplay");
			DebugDisp.Scale = 0.75f;
#endif
			if (!IsPaused && !HasFinished) LevelTime += gameTime.RealtimeElapsedSeconds;

			TestForGameEndingCondition();
		}

		public void FastForward(float sendertime)
		{
			var delta = LevelTime - sendertime;
			LevelTime = sendertime;

			if (delta < 1 / 30f) return; // idgaf

			Entities.Update(new SAMTime(delta, MonoSAMGame.CurrentTime.TotalElapsedSeconds), InputStateMan.GetCurrentState());
		}

		protected override void OnDrawGame(IBatchRenderer sbatch)
		{
			//
		}

		protected override void OnDrawHUD(IBatchRenderer sbatch)
		{
			//
		}

		protected abstract void TestForGameEndingCondition();

		protected void EndGameConvert(Fraction f)
		{
			foreach (var cannon in Entities.Enumerate().OfType<Cannon>())
			{
				if (cannon.Fraction != f && !cannon.Fraction.IsNeutral)
				{
					cannon.SetFractionAndHealth(f, 0.1f);
				}
			}
		}

		public World GetPhysicsWorld()
		{
			return GDEntities.PhysicsWorld;
		}

		public LaserNetwork.LaserNetwork GetLaserNetwork()
		{
			return LaserNetwork;
		}

		public override void Resize(int width, int height)
		{
			base.Resize(width, height);

#if DEBUG
			var newQuality = Textures.GetPreferredQuality(Game.GraphicsDevice);
			if (newQuality != Textures.TEXTURE_QUALITY)
			{
				Textures.ChangeQuality(Game.Content, newQuality);
			}
#endif
		}

		public void UpdateGameSpeed()
		{
			if (IsPaused)
			{
				GameSpeed = 0f;
			}
			else
			{
				switch (GameSpeedMode)
				{
					case GameSpeedModes.SUPERSLOW:
						GameSpeed = GAMESPEED_SUPERSLOW;
						break;
					case GameSpeedModes.SLOW:
						GameSpeed = GAMESPEED_SLOW;
						break;
					case GameSpeedModes.NORMAL:
						GameSpeed = GAMESPEED_NORMAL;
						break;
					case GameSpeedModes.FAST:
						GameSpeed = GAMESPEED_FAST;
						break;
					case GameSpeedModes.SUPERFAST:
						GameSpeed = GAMESPEED_SUPERFAST;
						break;
					default:
						SAMLog.Error("GDGS::EnumSwitch_UGSS", "value: " + GameSpeedMode);
						break;
				}
			}
		}

		public abstract void RestartLevel(bool updateSpeed);
		public abstract void ReplayLevel(FractionDifficulty diff);
		public abstract void ShowScorePanel(LevelBlueprint lvl, PlayerProfile profile, HashSet<FractionDifficulty> newDifficulties, bool playerHasWon, int addPoints, int time);
		public abstract void ExitToMap(bool updateSpeed);
		public abstract AbstractFractionController CreateController(Fraction f, Cannon cannon);
	}
}
