using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W4 : OverworldNode_Graph
	{
		public OverworldNode_W4(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_004, Levels.WORLD_003, GDConstants.IAB_WORLD4)
		{
			//
		}

		protected override void OnClickDeny()
		{
			if (_ustate == WorldUnlockState.NeedsAction) { ShowPreview(); return; }

			base.OnClickDeny();
		}

		protected override void ShowPreview()
		{
			LevelBlueprint[] previews =
			{
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000001")], //TODO Choose preview levels
			};

			Owner.HUD.AddModal(new WorldPreviewPanel(previews, Blueprint.ID, IABCode, 4, PreviousWorld), true, 0.8f, 1f);
		}
	}
}
