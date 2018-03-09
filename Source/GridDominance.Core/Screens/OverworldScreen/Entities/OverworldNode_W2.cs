using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using System.Linq;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Dialogs;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W2 : OverworldNode_Graph
	{
		public OverworldNode_W2(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_002, GDConstants.IAB_WORLD2)
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
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000019")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000015")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000012")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000013")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000005")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000025")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000002000027")],
			};


			Owner.HUD.AddModal(new WorldPreviewPanel(previews, Blueprint.ID, IABCode, 2), true, 0.8f, 1f);
		}
	}
}
