using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Screens.WorldMapScreen.Entities;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities.Operation;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class ShakeNodeOperation : GameEntityOperation<OverworldNode>
	{
		public const float SHAKE_TIME = 0.55f;
		public const float SHAKE_OFFSET = 16f;

		private Vector2 realPos;

		public ShakeNodeOperation() : base("OverworldNode::Shake", SHAKE_TIME)
		{

		}

		protected override void OnStart(OverworldNode node)
		{
			realPos = node.NodePos;
		}

		protected override void OnProgress(OverworldNode node, float progress, SAMTime gameTime, InputState istate)
		{
			var off = Vector2.UnitX * (FloatMath.Sin(progress * FloatMath.TAU * 6) * SHAKE_OFFSET) * (1 - FloatMath.FunctionEaseInCubic(progress));

			node.NodePos = realPos + off;
		}

		protected override void OnEnd(OverworldNode node)
		{
			node.NodePos = realPos;
		}

		protected override void OnAbort(OverworldNode node)
		{
			node.NodePos = realPos;
		}
	}
}