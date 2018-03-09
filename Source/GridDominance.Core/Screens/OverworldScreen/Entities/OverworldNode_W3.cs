using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using System.Linq;
using GridDominance.Shared.Screens.OverworldScreen.HUD.Dialogs;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W3 : OverworldNode_Graph
	{
		public OverworldNode_W3(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_003, GDConstants.IAB_WORLD3)
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
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000001")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000004")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000009")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000025")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000007")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000017")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000003")],
				Levels.LEVELS[Guid.Parse(@"b16b00b5-0001-4000-0000-000003000023")],
			};

			Owner.HUD.AddModal(new WorldPreviewPanel(previews, Blueprint.ID, IABCode, 3), true, 0.8f, 1f);
		}
	}
}
