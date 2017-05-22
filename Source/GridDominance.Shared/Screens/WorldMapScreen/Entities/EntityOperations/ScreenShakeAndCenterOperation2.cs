using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities.EntityOperations
{
	class ScreenShakeAndCenterOperation2 : GameEntityOperation<WarpNode>
	{
		public const float SHAKE_OFFSET = 16f;

		private readonly Vector2 centeringStartOffset;
		private readonly float rot;

		public ScreenShakeAndCenterOperation2(WarpNode node, GameScreen screen) : base("WarpNode::CenterShake", LevelNode.SHAKE_TIME)
		{
			centeringStartOffset = new Vector2(screen.MapViewportCenterX, screen.MapViewportCenterY);

			if ((centeringStartOffset - node.Position).LengthSquared() < 0.1f)
				rot = FloatMath.RAD_POS_000;
			else
				rot = (centeringStartOffset - node.Position).ToAngle() + FloatMath.RAD_POS_090;
		}

		protected override void OnStart(WarpNode node)
		{
			//
		}

		protected override void OnProgress(WarpNode node, float progress, SAMTime gameTime, InputState istate)
		{
			var off = (Vector2.UnitX * (FloatMath.Sin(progress * FloatMath.TAU * 6) * SHAKE_OFFSET) * (1 - FloatMath.FunctionEaseInCubic(progress))).Rotate(rot);
			var p = FloatMath.FunctionEaseInOutQuad(progress);

			node.Owner.MapViewportCenterX = centeringStartOffset.X + p * (node.Position.X - centeringStartOffset.X) + off.X;
			node.Owner.MapViewportCenterY = centeringStartOffset.Y + p * (node.Position.Y - centeringStartOffset.Y) + off.Y;
		}

		protected override void OnEnd(WarpNode node)
		{
			node.Owner.MapViewportCenterX = node.Position.X;
			node.Owner.MapViewportCenterY = node.Position.Y;
		}

		protected override void OnAbort(WarpNode node)
		{
			//
		}
	}
}
