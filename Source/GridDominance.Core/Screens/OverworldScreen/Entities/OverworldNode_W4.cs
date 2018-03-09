using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using System.Linq;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Dialogs;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W4 : OverworldNode_Graph
	{
		public OverworldNode_W4(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_004, GDConstants.IAB_WORLD4)
		{
			//
		}

		protected override void OnClick_OpenAndUnlocked()
		{
			DefaultAction_OpenAndUnlocked();
		}

		protected override void OnClick_ReachableButMustBeBought()
		{
			DefaultAction_ReachableButMustBeBought();
		}

		protected override void OnClick_UnreachableButCanBeBought()
		{
			DefaultAction_UnreachableButCanBeBought();
		}

		protected override void OnClick_UnreachableAndFullyLocked()
		{
			DefaultAction_UnreachableAndFullyLocked();
		}

		public override void ShowPreview()
		{
			if (Owner.HUD.Enumerate().Any(e => e is WorldPreviewPanel)) return;

			LevelBlueprint[] previews =
			{
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000018")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000008")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000011")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000027")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000013")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000015")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000004000025")],
			};

			Owner.HUD.AddModal(new WorldPreviewPanel(previews, Blueprint.ID, IABCode, 4), true, 0.8f, 1f);
		}
	}
}
