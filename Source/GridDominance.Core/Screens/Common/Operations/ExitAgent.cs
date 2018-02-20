using System;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.UpdateAgents;

namespace GridDominance.Shared.Screens.Common.Agents
{
	class ExitAgent : SAMUpdateOp<GameScreen>
	{
		private float _lastBackClick = -9999f;

		public override string Name => "ExitAgent";

		public ExitAgent()
		{
		}

		protected override void OnUpdate(GameScreen screen, SAMTime gameTime, InputState istate)
		{
			bool trigger = false;
			if (istate.IsKeyExclusiveJustDown(SKeys.AndroidBack))
			{
				istate.SwallowKey(SKeys.AndroidBack, InputConsumer.ScreenAgent);
				trigger = true;
			}
			else if (istate.IsKeyExclusiveJustDown(SKeys.Backspace))
			{
				istate.SwallowKey(SKeys.Backspace, InputConsumer.ScreenAgent);
				trigger = true;
			}

			if (trigger)
			{
				var delta = gameTime.TotalElapsedSeconds - _lastBackClick;

				if (delta < 2f)
				{
					MainGame.Inst.CleanExit();
					return;
				}
				else
				{
					screen.HUD.ShowToast(null, L10N.T(L10NImpl.STR_GLOB_EXITTOAST), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
				}

				_lastBackClick = gameTime.TotalElapsedSeconds;
			}
		}

	}
}
