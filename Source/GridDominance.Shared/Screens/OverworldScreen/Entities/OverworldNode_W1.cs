using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W1 : OverworldNode_Graph
	{
		public int ForceClickCounter = 0;

		public OverworldNode_W1(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_001, null, null)
		{
			//
		}

		protected override void OnClickDeny()
		{
			if (ForceClickCounter == 0)
			{
				Owner.HUD.ShowToast("ONW1::UNLOCK(MULTI)", L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST1), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
				ForceClickCounter++;

				MainGame.Inst.GDSound.PlayEffectError();
				AddEntityOperation(new ShakeNodeOperation());
			}
			else if (ForceClickCounter == 1)
			{
				Owner.HUD.ShowToast("ONW1::UNLOCK(MULTI)", L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST2), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
				ForceClickCounter++;

				MainGame.Inst.GDSound.PlayEffectError();

				AddEntityOperation(new ShakeNodeOperation());
			}
			else if (ForceClickCounter == 2)
			{
				Owner.HUD.ShowToast("ONW1::UNLOCK(MULTI)", L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST3), 40, FlatColors.Silver, FlatColors.Foreground, 2f);

				MainGame.Inst.Profile.SkipTutorial = true;
				MainGame.Inst.SaveProfile();
				_ustate = WorldUnlockState.Unlocked;
				return;
			}
		}
	}
}
