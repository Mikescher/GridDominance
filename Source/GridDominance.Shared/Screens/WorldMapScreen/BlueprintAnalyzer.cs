using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;

namespace GridDominance.Shared.Screens.WorldMapScreen
{
	public static class BlueprintAnalyzer
	{
		public static NodeBlueprint? FindNextNode(GraphBlueprint g, Guid idnode, FractionDifficulty d)
		{
			var snode = Get(g, idnode);
			if (snode == null) return null;

			return FindNextNode(g, snode, d);
		}
		
		public static NodeBlueprint? FindNextNode(GraphBlueprint g, INodeBlueprint snode, FractionDifficulty d)
		{
			snode = Get(g, snode.ConnectionID);
			if (snode == null) return null;

			// unfinished descendants
			var descendant = FindNextUnfinishedNode(g, snode, d);
			if (descendant != null) return descendant.Value;

			// all unfinished
			var unfin = FindNextUnfinishedNode(g, g.RootNode, d);
			if (unfin != null) return unfin.Value;
			
			// none
			return null;
		}
		
		public static INodeBlueprint FindInitialNode(GraphBlueprint g)
		{
			INodeBlueprint n;

			n = FindNextUnfinishedNode(g, g.RootNode, FractionDifficulty.DIFF_0);
			if (n != null) return n;

			n = FindNextUnfinishedNode(g, g.RootNode, FractionDifficulty.DIFF_1);
			if (n != null) return n;

			n = FindNextUnfinishedNode(g, g.RootNode, FractionDifficulty.DIFF_2);
			if (n != null) return n;

			n = FindNextUnfinishedNode(g, g.RootNode, FractionDifficulty.DIFF_3);
			if (n != null) return n;

			return g.RootNode; // can happen when all completed
		}

		private static NodeBlueprint? FindNextUnfinishedNode(GraphBlueprint g, INodeBlueprint snode, FractionDifficulty d)
		{
			Stack<INodeBlueprint> mem = new Stack<INodeBlueprint>();
			mem.Push(snode);

			while (mem.Any())
			{
				var node = mem.Pop();

				foreach (var pipe in node.Pipes.OrderBy(p => p.Priority))
				{
					var lnode = Get(g, pipe.Target);
					if (!(lnode is NodeBlueprint)) continue;
					
					if (!MainGame.Inst.Profile.GetLevelData(lnode.ConnectionID).HasCompleted(d)) return (NodeBlueprint?)lnode;
					
					mem.Push(lnode);
				}
			}

			return null;
		}

		private static INodeBlueprint Get(GraphBlueprint g, Guid id)
		{
			return g.AllNodes.FirstOrDefault(n => n.ConnectionID == id);
		}
	}
}
