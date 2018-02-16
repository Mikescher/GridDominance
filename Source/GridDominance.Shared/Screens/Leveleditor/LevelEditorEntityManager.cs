using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.Leveleditor
{
	class LevelEditorEntityManager : EntityManager
	{
		public LevelEditorEntityManager(GameScreen screen) : base(screen)
		{
		}

		public override void DrawOuterDebug()
		{
			// NOP
		}

		protected override FRectangle RecalculateBoundingBox()
		{
			return Owner.VAdapterGame.VirtualTotalBoundingBox.AsInflated(GDConstants.TILE_WIDTH, GDConstants.TILE_WIDTH);
		}

		protected override void OnBeforeUpdate(SAMTime gameTime, InputState state)
		{
			// NOP
		}

		protected override void OnAfterUpdate(SAMTime gameTime, InputState state)
		{
			// NOP
		}
	}
}