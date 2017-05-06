using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class ScreenShakeAndCenterOperation : GameEntityOperation<LevelNode>
	{
		private readonly Vector2 centeringStartOffset;
		private readonly float rot;

		public ScreenShakeAndCenterOperation(LevelNode node, GameScreen screen) : base("LevelNode::CenterShake", LevelNode.SHAKE_TIME)
		{
			centeringStartOffset = new Vector2(screen.MapViewportCenterX, screen.MapViewportCenterY);

			if ((centeringStartOffset - node.Position).LengthSquared() < 0.1f)
				rot = FloatMath.RAD_POS_090;
			else
				rot = (centeringStartOffset - node.Position).ToAngle() + FloatMath.RAD_POS_090;
		}

		protected override void OnStart(LevelNode node)
		{
			//
		}

		protected override void OnProgress(LevelNode node, float progress, SAMTime gameTime, InputState istate)
		{
			var off = new Vector2(0, FloatMath.Sin(progress * FloatMath.TAU * 3) * 32 * (1 - FloatMath.FunctionEaseInCubic(progress))).Rotate(rot);
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			node.Owner.MapViewportCenterX = centeringStartOffset.X + p * (node.Position.X - centeringStartOffset.X) + off.X;
			node.Owner.MapViewportCenterY = centeringStartOffset.Y + p * (node.Position.Y - centeringStartOffset.Y) + off.Y;
		}

		protected override void OnEnd(LevelNode node)
		{
			node.Owner.MapViewportCenterX = node.Position.X;
			node.Owner.MapViewportCenterY = node.Position.Y;
		}

		protected override void OnAbort(LevelNode node)
		{
			//
		}
	}
}
