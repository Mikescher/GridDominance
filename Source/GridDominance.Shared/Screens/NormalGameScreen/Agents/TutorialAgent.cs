using System;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Button;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Enums;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
	public class TutorialAgent : GameScreenAgent
	{
		private enum TutorialState
		{
			Start = 0,
			TargetFirstNeutral = 1,
			ShootingFirstNeutral = 2,
			TargetCenter = 3,
			BoostWhileShootingCenter = 4,
			AttackEnemy = 5,
			ChangeGameSpeed = 6,
			CaptureEnemy = 7,
			WinGame = 8,
		}

		public override bool Alive { get; } = true;

		private readonly GDGameScreen _screen;
		private readonly GDGameHUD _hud;
		private readonly Cannon _cannon1; // bottomLeft
		private readonly Cannon _cannon2; // topLeft
		private readonly Cannon _cannon3; // topCenter
		private readonly Cannon _cannon4; // topRight
		private readonly Cannon _cannon5; // BottomRight
		private readonly TutorialController _controller5; // BottomRight
		private          TutorialController _controller4; // TopRight
		private readonly Fraction _fracPlayer;
		private readonly Fraction _fracComputer;
		private readonly Fraction _fracNeutral;

		private TutorialState _state = TutorialState.Start;


		private float _s1_blinkTimer = 0f;

		private HUDInfoBox _infobox;
		private HUDTouchAnimation _anim;
		private HUDArrowAnimation _anim2;

		public TutorialAgent(GDGameScreen scrn) : base(scrn)
		{
			_screen = scrn;

			_cannon1 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 1001);
			_cannon2 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 1002);
			_cannon3 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 3000);
			_cannon4 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 2002);
			_cannon5 = scrn.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 2001);

			_controller5 = new TutorialController(_screen, _cannon5);
			_cannon5.ForceSetController(_controller5);

			_fracPlayer = _cannon1.Fraction;
			_fracNeutral = _cannon3.Fraction;
			_fracComputer = _cannon5.Fraction;

			_hud = _screen.GDGameHUD;

			_screen.GameSpeedMode = GameSpeedModes.SLOW;

#if DEBUG
			_screen.DebugDisp.AddLine(() => $"Tutorial_State = ({(int)_state}) {_state}");
#endif
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			if (_state >= TutorialState.TargetFirstNeutral) _cannon1.CannonHealth.SetForce(1);
			if (_state >= TutorialState.TargetCenter)      _cannon2.CannonHealth.SetForce(1);
			if (_state >= TutorialState.AttackEnemy)       _cannon3.CannonHealth.SetForce(1);

			_s1_blinkTimer += gameTime.ElapsedSeconds;

			switch (_state)
			{
				case TutorialState.Start:
					Transition_1_ShootFirstNeutral();
					break;

				case TutorialState.TargetFirstNeutral:
					if (_cannon2.Fraction != _fracNeutral)
					{
						_cannon2.CannonHealth.SetForce(0f);
						_cannon2.SetFraction(_fracNeutral);
					}
					_infobox.ColorBackground = ColorMath.Blend(FlatColors.Concrete, FlatColors.Orange, FloatMath.PercSin(_s1_blinkTimer*4f));
					if (FloatMath.DiffRadiansAbs(_cannon1.Rotation.ActualValue, FloatMath.RAD_POS_270) < FloatMath.RAD_POS_060) Transition_2_ShootingFirstNeutral();
					break;

				case TutorialState.ShootingFirstNeutral:
					_infobox.ColorBackground = FlatColors.Concrete;
					if (FloatMath.IsOne(_cannon2.CannonHealth.ActualValue)) Transition_3_TargetCenter();
					break;

				case TutorialState.TargetCenter:
					if (FloatMath.DiffRadiansAbs(_cannon2.Rotation.ActualValue, FloatMath.RAD_000) < FloatMath.RAD_POS_060) Transition_4_BoostWhileShootingCenter();
					break;

				case TutorialState.BoostWhileShootingCenter:
					_cannon3.ForceResetBarrelCharge(); // no shooting enemy for now
					_cannon4.ForceResetBarrelCharge(); // no shooting me    for now
					_cannon3.Rotation.Set(FloatMath.RAD_POS_090);
					var cap3 = _cannon3.Fraction == _fracPlayer && FloatMath.IsOne(_cannon3.CannonHealth.ActualValue);
					var cap4 = _cannon4.Fraction == _fracComputer && FloatMath.IsOne(_cannon4.CannonHealth.ActualValue);
					if (cap3 && _cannon4.Fraction == _fracComputer) _cannon4.CannonHealth.Set(1);
					if (cap3 && cap4) Transition_5_AttackEnemy();
					break;

				case TutorialState.AttackEnemy:
					_cannon4.CannonHealth.SetForce(1);
					if (FloatMath.DiffRadiansAbs(_cannon3.Rotation.ActualValue, FloatMath.RAD_000) < FloatMath.RAD_POS_060) Transition_6_ChangeGameSpeed();
					break;

				case TutorialState.ChangeGameSpeed:
					_cannon4.CannonHealth.SetForce(1);
					if (_screen.GameSpeedMode > GameSpeedModes.SUPERSLOW) Transition_7_CaptureEnemy();
					break;

				case TutorialState.CaptureEnemy:
					_infobox.ColorBackground = ColorMath.Blend(FlatColors.Alizarin, FlatColors.SunFlower, FloatMath.PercSin(_s1_blinkTimer * 6f));
					if (FloatMath.IsOne(_cannon4.CannonHealth.ActualValue) && _cannon4.Fraction == _fracPlayer) Transition_8_WinGame();
					break;

				case TutorialState.WinGame:
					_infobox.ColorBackground = FlatColors.Concrete;
					_infobox.ColorBackground = ColorMath.Blend(FlatColors.Emerald, FlatColors.Concrete, FloatMath.PercSin(_s1_blinkTimer * 4f));
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Transition_1_ShootFirstNeutral()
		{
			_state = TutorialState.TargetFirstNeutral;

			_controller5.RechargeBarrel = false;

			SetInfoBox("Drag to rotate your own cannons");

			AddTouchAnimation(_cannon1, _cannon2);
		}

		private void Transition_2_ShootingFirstNeutral()
		{
			_state = TutorialState.ShootingFirstNeutral;

			_infobox.Remove();
			SetInfoBox("Shoot it until it becomes your cannon");

			_anim?.Remove();
		}

		private void Transition_3_TargetCenter()
		{
			_state = TutorialState.TargetCenter;

			_cannon5.RotateTo(_cannon4);

			if (FloatMath.DiffRadiansAbs(_cannon2.Rotation.ActualValue, FloatMath.RAD_POS_090) < FloatMath.RAD_POS_060)
			{
				_cannon2.Rotation.Set(FloatMath.RAD_POS_180);
			}

			SetInfoBox("Now capture the next cannon");

			AddTouchAnimation(_cannon2, _cannon3);
		}

		private void Transition_4_BoostWhileShootingCenter()
		{
			_state = TutorialState.BoostWhileShootingCenter;

			_controller5.RechargeBarrel = true;

			SetInfoBox("Keep shooting at the first cannon to increase its fire rate");

			_anim?.Remove();
		}

		private void Transition_5_AttackEnemy()
		{
			_state = TutorialState.AttackEnemy;

			_controller4 = new TutorialController(_screen, _cannon4);
			_cannon4.ForceSetController(_controller4);

			_cannon4.RotateTo(_cannon3);
			_controller4.RechargeBarrel = true;

			_cannon1.RotateTo(_cannon2);
			_cannon2.RotateTo(_cannon3);

			SetInfoBox("The enemy has captured a cannon. Attack him!");

			AddTouchAnimation(_cannon3, _cannon4);
		}

		private void Transition_6_ChangeGameSpeed()
		{
			_state = TutorialState.ChangeGameSpeed;
			
			_screen.GameSpeedMode = GameSpeedModes.SUPERSLOW;

			SetInfoBox("Speed up the Game with the bottom left button.");

			_anim?.Remove();

			_hud.AddElement(_anim2 = new HUDArrowAnimation(_hud.BtnSpeed.CenterPos + new Vector2(60, -40), 150 * FloatMath.DegreesToRadians));
		}

		private void Transition_7_CaptureEnemy()
		{
			_state = TutorialState.CaptureEnemy;

			_screen.GameSpeedMode = GameSpeedModes.NORMAL;

			_cannon3.RotateTo(_cannon4);
			_controller5.RechargeBarrel = false;

			SetInfoBox("Now capture the enemy cannon");

			_anim2.Remove();
		}

		private void Transition_8_WinGame()
		{
			_state = TutorialState.WinGame;

			_cannon1.RotateTo(_cannon2);
			_cannon2.RotateTo(_cannon3);
			_cannon3.RotateTo(_cannon4);

			_controller5.RechargeBarrel = true;

			SetInfoBox("Win the game by capturing all enemy cannons");
		}

		private void SetInfoBox(string text)
		{
			_infobox?.Remove();

			_hud.AddElement(_infobox = new HUDInfoBox
			{
				Text = text,
				Alignment = HUDAlignment.ABSOLUTE_BOTHCENTERED,
				RelativePosition = _screen.TranslateGameToHUDCoordinates(8 * GDConstants.TILE_WIDTH, 9 * GDConstants.TILE_WIDTH),
				FontSize = 40,
				Font = Textures.HUDFontRegular,
				TextColor = Color.Black,
				ColorBackground = FlatColors.Concrete,
				Alpha = 1f,
				TextPadding = new FSize(8, 8),
				BackgroundType = HUDBackgroundType.RoundedBlur,
				MaxWidth = 8 * GDConstants.TILE_WIDTH,
				BackgoundCornerSize = 4f,
				WordWrap = HUDWordWrap.WrapByWordTrusted,
			});
		}

		private void AddTouchAnimation(Cannon c1, Cannon c2)
		{
			_anim?.Remove();

			var p1 = _screen.TranslateGameToHUDCoordinates(c1.Position.X, c1.Position.Y);
			var p2 = _screen.TranslateGameToHUDCoordinates(c2.Position.X, c2.Position.Y);

			_anim = new HUDTouchAnimation(p1, p2);

			_hud.AddElement(_anim);
		}
	}
}
