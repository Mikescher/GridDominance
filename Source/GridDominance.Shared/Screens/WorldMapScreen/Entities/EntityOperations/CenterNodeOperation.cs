using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class CenterNodeOperation : GameEntityOperation<LevelNode>
	{
		private readonly Vector2 centeringStartOffset;
		private readonly GDWorldMapScreen _screen;

		public CenterNodeOperation(GDWorldMapScreen screen) : base("LevelNode::Center", LevelNode.CENTERING_TIME)
		{
			_screen = screen;
			centeringStartOffset = new Vector2(screen.MapViewportCenterX, screen.MapViewportCenterY);
		}

		protected override void OnStart(LevelNode node)
		{
			//
		}

		protected override void OnProgress(LevelNode node, float progress, SAMTime gameTime, InputState istate)
		{
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			node.Owner.MapViewportCenterX = centeringStartOffset.X + p * (node.Position.X - centeringStartOffset.X);
			node.Owner.MapViewportCenterY = centeringStartOffset.Y + p * (node.Position.Y - centeringStartOffset.Y);

			if (_screen.IsDragging) Abort();
		}

		protected override void OnEnd(LevelNode node)
		{
			//
		}

		protected override void OnAbort(LevelNode node)
		{
			//
		}
	}
}
