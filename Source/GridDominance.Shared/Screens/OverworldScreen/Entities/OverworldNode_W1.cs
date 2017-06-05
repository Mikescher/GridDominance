using Microsoft.Xna.Framework;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.DebugTools;
using GridDominance.Shared.Screens.WorldMapScreen.Agents;
using MonoSAMFramework.Portable.Localization;
using MonoSAMFramework.Portable.ColorHelper;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W1 : OverworldNode_Graph
	{
		public int ForceClickCounter = 0;

		public OverworldNode_W1(GDOverworldScreen scrn, Vector2 pos) : base(scrn, pos, Levels.WORLD_001)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (IsUnlocked())
				DrawGridProgress(sbatch);
			else
				DrawLockSwing(sbatch);
		}

		private bool? _isUnlocked = null;
		protected override bool IsUnlocked()
		{
			if (_isUnlocked == null)
				_isUnlocked = MainGame.Inst.Profile.SkipTutorial || MainGame.Inst.Profile.GetLevelData(Levels.LEVEL_TUTORIAL).HasAnyCompleted();

			return _isUnlocked.Value;
		}

		protected override void OnClickDeny()
		{
			if (ForceClickCounter == 0)
			{
				Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST1), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
				ForceClickCounter++;

				MainGame.Inst.GDSound.PlayEffectError();
				AddEntityOperation(new ShakeNodeOperation());
			}
			else if (ForceClickCounter == 1)
			{
				Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST2), 40, FlatColors.Silver, FlatColors.Foreground, 2f);
				ForceClickCounter++;

				MainGame.Inst.GDSound.PlayEffectError();

				AddEntityOperation(new ShakeNodeOperation());
			}
			else if (ForceClickCounter == 2)
			{
				Owner.HUD.ShowToast(L10N.T(L10NImpl.STR_GLOB_UNLOCKTOAST3), 40, FlatColors.Silver, FlatColors.Foreground, 2f);

				MainGame.Inst.Profile.SkipTutorial = true;
				MainGame.Inst.SaveProfile();
				_isUnlocked = null;
				return;
			}
		}
	}
}
