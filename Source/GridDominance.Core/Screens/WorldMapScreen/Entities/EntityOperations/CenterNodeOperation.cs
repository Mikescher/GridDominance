using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class CenterNodeOperation : FixTimeOperation<LevelNode>
	{
		private readonly Vector2 centeringStartOffset;
		private readonly GDWorldMapScreen _screen;

		public override string Name => "LevelNode::Center";

		public CenterNodeOperation(GDWorldMapScreen screen) : base(LevelNode.CENTERING_TIME)
		{
			_screen = screen;
			centeringStartOffset = new Vector2(screen.MapViewportCenterX, screen.MapViewportCenterY);
		}

		protected override void OnProgress(LevelNode node, float progress, SAMTime gameTime, InputState istate)
		{
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			node.Owner.MapViewportCenterX = centeringStartOffset.X + p * (node.Position.X - centeringStartOffset.X);
			node.Owner.MapViewportCenterY = centeringStartOffset.Y + p * (node.Position.Y - centeringStartOffset.Y);

			if (_screen.IsDragging) Abort();
		}
	}
}
