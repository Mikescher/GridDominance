using System;
using System.Linq;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Entities.Cannons;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.NormalGameScreen.HUD.Elements;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.HUD.Elements.Other;
using MonoSAMFramework.Portable.Screens.HUD.Enums;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Operations
{
	public class TutorialAgent : SAMUpdateOp<GDGameScreen>
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

		private GDGameScreen _screen;
		private GDGameHUD _hud;
		private Cannon _cannon1; // bottomLeft
		private Cannon _cannon2; // topLeft
		private Cannon _cannon3; // topCenter
		private Cannon _cannon4; // topRight
		private Cannon _cannon5; // BottomRight
		private TutorialController _controller5; // BottomRight
		private TutorialController _controller4; // TopRight
		private Fraction _fracPlayer;
		private Fraction _fracComputer;
		private Fraction _fracNeutral;

		private TutorialState _state = TutorialState.Start;


		private float _s1_blinkTimer = 0f;

		private HUDInfoBox _infobox;
		private HUDTouchAnimation _anim;
		private HUDArrowAnimation _anim2;

		public override string Name => "TutorialAgent";

		public TutorialAgent()
		{
			//
		}

		protected override void OnInit(GDGameScreen screen)
		{
			_screen = screen;

			_cannon1 = screen.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 11);
			_cannon2 = screen.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 12);
			_cannon3 = screen.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 30);
			_cannon4 = screen.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 22);
			_cannon5 = screen.GetEntities<Cannon>().Single(c => c.BlueprintCannonID == 21);

			_controller5 = new TutorialController(_screen, _cannon5);
			_cannon5.ForceSetController(_controller5);

			_fracPlayer = _cannon1.Fraction;
			_fracNeutral = _cannon3.Fraction;
			_fracComputer = _cannon5.Fraction;

			_hud = (GDGameHUD)_screen.HUD;

			_screen.GameSpeedMode = GameSpeedModes.SLOW;

#if DEBUG
			_screen.DebugDisp.AddLine(() => $"Tutorial_State = ({(int)_state}) {_state}");
#endif
		}

		protected override void OnUpdate(GDGameScreen screen, SAMTime gameTime, InputState istate)
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
					_infobox.Background = _infobox.Background.WithColor(ColorMath.Blend(FlatColors.Concrete, FlatColors.Orange, FloatMath.PercSin(_s1_blinkTimer*4f)));
					if (FloatMath.DiffRadiansAbs(_cannon1.Rotation.ActualValue, FloatMath.RAD_POS_270) < FloatMath.RAD_POS_060) Transition_2_ShootingFirstNeutral();
					break;

				case TutorialState.ShootingFirstNeutral:
					_infobox.Background = _infobox.Background.WithColor(FlatColors.Concrete);
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
					_infobox.Background = _infobox.Background.WithColor(ColorMath.Blend(FlatColors.Alizarin, FlatColors.SunFlower, FloatMath.PercSin(_s1_blinkTimer * 6f)));
					if (FloatMath.IsOne(_cannon4.CannonHealth.ActualValue) && _cannon4.Fraction == _fracPlayer) Transition_8_WinGame();
					break;

				case TutorialState.WinGame:
					_infobox.Background = _infobox.Background.WithColor(ColorMath.Blend(FlatColors.Emerald, FlatColors.Concrete, FloatMath.PercSin(_s1_blinkTimer * 4f)));
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Transition_1_ShootFirstNeutral()
		{
			_state = TutorialState.TargetFirstNeutral;

			_controller5.RechargeBarrel = false;

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO1));

			AddTouchAnimation(_cannon1, _cannon2);
		}

		private void Transition_2_ShootingFirstNeutral()
		{
			_state = TutorialState.ShootingFirstNeutral;

			_infobox.Remove();
			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO2));

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

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO3));

			AddTouchAnimation(_cannon2, _cannon3);
		}

		private void Transition_4_BoostWhileShootingCenter()
		{
			_state = TutorialState.BoostWhileShootingCenter;

			_controller5.RechargeBarrel = true;

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO4));

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

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO5));

			AddTouchAnimation(_cannon3, _cannon4);
		}

		private void Transition_6_ChangeGameSpeed()
		{
			_state = TutorialState.ChangeGameSpeed;
			
			_screen.GameSpeedMode = GameSpeedModes.SUPERSLOW;

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO6));

			_anim?.Remove();

			_hud.AddElement(_anim2 = new HUDArrowAnimation(_hud.BtnSpeed.Center + new Vector2(60, -40), 150 * FloatMath.DegreesToRadians));
		}

		private void Transition_7_CaptureEnemy()
		{
			_state = TutorialState.CaptureEnemy;

			_screen.GameSpeedMode = GameSpeedModes.NORMAL;

			_cannon3.RotateTo(_cannon4);
			_controller5.RechargeBarrel = false;

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO7));

			_anim2.Remove();
		}

		private void Transition_8_WinGame()
		{
			_state = TutorialState.WinGame;

			_cannon1.RotateTo(_cannon2);
			_cannon2.RotateTo(_cannon3);
			_cannon3.RotateTo(_cannon4);

			_controller5.RechargeBarrel = true;

			SetInfoBox(L10N.T(L10NImpl.STR_TUT_INFO8));
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
				Alpha = 1f,
				TextPadding = new FSize(8, 8),
				MaxWidth = 8 * GDConstants.TILE_WIDTH,
				WordWrap = HUDWordWrap.WrapByWordTrusted,

				Background = HUDBackgroundDefinition.CreateRoundedBlur(FlatColors.Concrete, 4f),
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
