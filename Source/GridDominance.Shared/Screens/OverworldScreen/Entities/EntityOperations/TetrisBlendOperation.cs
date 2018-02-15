using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.UpdateAgents.Impl;

namespace GridDominance.Shared.Screens.OverworldScreen.Entities.EntityOperations
{
	class TetrisBlendOperation : FixTimeOperation<OverworldNode_SCCM>
	{
		public override string Name => "Tetris::Blend";

		private List<Color> _colors;

		public TetrisBlendOperation(float duration) : base(duration)
		{

		}

		protected override void OnStart(OverworldNode_SCCM node)
		{
			_colors = node.Blocks.Select(b => b.Item2).ToList();
		}
		
		protected override void OnProgress(OverworldNode_SCCM node, float progress, SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < _colors.Count; i++)
			{
				node.Blocks[i].Item2 = ColorMath.Blend(_colors[i], Color.White, progress);
			}
		}

		protected override void OnEnd(OverworldNode_SCCM node)
		{
			foreach (var b in node.Blocks) b.Item2 = Color.White;
		}

		protected override void OnAbort(OverworldNode_SCCM node)
		{
			OnEnd(node);
		}
	}
}
