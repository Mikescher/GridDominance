using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public class GDWorldMapEntityManager : EntityManager
	{
		public GDWorldMapEntityManager(GameScreen screen) : base(screen)
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

		protected override void OnAfterUpdate(SAMTime gameTime, InputState state)
		{
			// NOP
		}

		protected override void OnBeforeUpdate(SAMTime gameTime, InputState state)
		{
			// NOP
		}
	}
}
