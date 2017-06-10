using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.HUD.Operations;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using GridDominance.Shared.Screens.WorldMapScreen.HUD;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Localization;

namespace GridDominance.Shared.Screens.Common.HUD.HUDOperations
{
	class UnlockSucessOperation : HUDTimedElementOperation<UnlockPanel>
	{
		public override string Name => "UnlockSucessOperation";

		public UnlockSucessOperation() : base(0.33f * 9)
		{

		}

		protected override void OnStart(UnlockPanel element)
		{
			//
		}

		protected override void OnProgress(UnlockPanel element, float progress, InputState istate)
		{
			for (int i = 0; i < (int)(progress*9); i++)
			{
				if (i >= 8) continue;

				element.CharDisp[i].BackgroundColor = FlatColors.Emerald;
			}
		}

		protected override void OnEnd(UnlockPanel element)
		{
			element.Remove();

			MainGame.Inst.SetOverworldScreen();
			
			element.HUD.ShowToast(L10N.T(L10NImpl.STR_GLOB_UNLOCKSUCCESS), 40, FlatColors.Emerald, FlatColors.Foreground, 3f);

			foreach (var w in Levels.WORLDS) MainGame.Inst.Profile.PurchasedWorlds.Add(w.Key);
			MainGame.Inst.SaveProfile();
			
			MainGame.Inst.GDSound.PlayEffectGameWon();
		}
	}
}