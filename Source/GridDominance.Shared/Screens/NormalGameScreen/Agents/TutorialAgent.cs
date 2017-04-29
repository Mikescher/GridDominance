using System;
using System.Linq;
using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.FractionController;
using GridDominance.Shared.Screens.NormalGameScreen.HUD;
using GridDominance.Shared.Screens.ScreenGame;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Agents;

namespace GridDominance.Shared.Screens.NormalGameScreen.Agents
{
	public class TutorialAgent : GameScreenAgent
	{
		private enum TutorialState
		{
			Start = 0,
			ShootFirstNeutral = 1,
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

		private TutorialState _state = TutorialState.Start;

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
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			switch (_state)
			{
				case TutorialState.Start:
					TransitionShootFirstNeutral();
					break;

				case TutorialState.ShootFirstNeutral:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void TransitionShootFirstNeutral()
		{
			_state = TutorialState.ShootFirstNeutral;

			_controller5.RechargeBarrel = false;
		}
	}
}
