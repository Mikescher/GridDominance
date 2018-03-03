using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.OverworldScreen
{
	class OverworldEntityManager : EntityManager
	{
		public OverworldEntityManager(GameScreen screen) : base(screen)
		{
		}

		public override void DrawOuterDebug()
		{
			// NOP
		}

		protected override FRectangle RecalculateBoundingBox()
		{
			return Owner.VAdapterGame.VirtualTotalBoundingBox;
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
