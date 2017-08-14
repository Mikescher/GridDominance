using GridDominance.Shared.Resources;
using GridDominance.Levelfileformat.Blueprint;
using System;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.OverworldScreen.HUD;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Localization;
using GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities
{
	class OverworldNode_W4 : OverworldNode_Graph
	{
		public OverworldNode_W4(GDOverworldScreen scrn, FPoint pos) : base(scrn, pos, Levels.WORLD_004, GDConstants.IAB_WORLD4)
		{
			//
		}
		
		protected override void OnClickNeedsAction()
		{
			DefaultActionClickNeedsAction();
		}

		protected override void OnClickFullyLocked()
		{
			DefaultActionClickFullyLocked();
		}

		protected override void ShowPreview()
		{
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
