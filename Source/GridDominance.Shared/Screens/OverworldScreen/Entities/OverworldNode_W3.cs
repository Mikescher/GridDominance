using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W3 : OverworldNode_Graph
	{
		public OverworldNode_W3(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_003, Levels.WORLD_002, GDConstants.IAB_WORLD3)
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
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000001")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000004")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000009")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000025")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000007")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000017")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000003")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000023")],
			};

			Owner.HUD.AddModal(new WorldPreviewPanel(previews, Blueprint.ID, IABCode, 3, PreviousWorld), true, 0.8f, 1f);
		}
	}
}
