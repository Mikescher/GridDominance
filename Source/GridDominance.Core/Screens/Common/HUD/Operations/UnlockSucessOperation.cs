using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common.HUD.Dialogs;
using GridDominance.Shared.Screens.OverworldScreen;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.Common.HUD.Operations
{
	class UnlockSucessOperation : FixTimeOperation<UnlockPanel>
	{
		public override string Name => "UnlockSucessOperation";

		public UnlockSucessOperation() : base(0.33f * 9)
		{

		}

		protected override void OnStart(UnlockPanel element)
		{
			//
		}

		protected override void OnProgress(UnlockPanel element, float progress, SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < (int)(progress*9); i++)
			{
				if (i >= 8) continue;

				element.CharDisp[i].Background = element.CharDisp[i].Background.WithColor(FlatColors.Emerald);
			}
		}

		protected override void OnEnd(UnlockPanel element)
		{
			element.Remove();

			foreach (var w in Levels.WORLDS) MainGame.Inst.Profile.PurchasedWorlds.Add(w.Key);
			MainGame.Inst.Profile.PurchasedWorlds.Add(Levels.WORLD_ID_MULTIPLAYER);
			MainGame.Inst.SaveProfile();

			MainGame.Inst.SetOverworldScreenCopy(element.HUD.Screen as GDOverworldScreen);

			element.HUD.ShowToast("UNLCK::SUCC", L10N.T(L10NImpl.STR_GLOB_UNLOCKSUCCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 3f);
			
			MainGame.Inst.GDSound.PlayEffectGameWon();
		}

		protected override void OnAbort(UnlockPanel owner)
		{
			OnEnd(owner);
		}
	}
}