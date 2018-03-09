using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
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
					
					if (!MainGame.Inst.Profile.GetLevelData(lnode.ConnectionID).HasCompletedOrBetter(d)) return (NodeBlueprint?)lnode;
					
					mem.Push(lnode);
				}
			}

			return null;
		}

		private static INodeBlueprint Get(GraphBlueprint g, Guid id)
		{
			return g.AllNodes.FirstOrDefault(n => n.ConnectionID == id);
		}

		public static void ListUnfinishedCount(GraphBlueprint g, out int missPoints, out int missLevel)
		{
			missPoints = 0;
			missLevel = 0;

			var p = MainGame.Inst.Profile;
			
			foreach (var levelnode in g.LevelNodes)
			{
				if (!p.GetLevelData(levelnode).HasCompletedOrBetter(FractionDifficulty.DIFF_0)) { missLevel++; missPoints += FractionDifficultyHelper.GetScore(FractionDifficulty.DIFF_0); }
				if (!p.GetLevelData(levelnode).HasCompletedOrBetter(FractionDifficulty.DIFF_1)) { missLevel++; missPoints += FractionDifficultyHelper.GetScore(FractionDifficulty.DIFF_1); }
				if (!p.GetLevelData(levelnode).HasCompletedOrBetter(FractionDifficulty.DIFF_2)) { missLevel++; missPoints += FractionDifficultyHelper.GetScore(FractionDifficulty.DIFF_2); }
				if (!p.GetLevelData(levelnode).HasCompletedOrBetter(FractionDifficulty.DIFF_3)) { missLevel++; missPoints += FractionDifficultyHelper.GetScore(FractionDifficulty.DIFF_3); }
			}

		}

		public static bool IsWorldReachable(GraphBlueprint world, Guid targetid)
		{
			var supplyNodes = world.LevelNodes.Where(n => n.OutgoingPipes.Any(p => p.Target == targetid));

			return supplyNodes.Any(l => MainGame.Inst.Profile.GetLevelData(l).HasAnyCompleted());
		}

		public static int PlayerCount(LevelBlueprint lvl)
		{
			int pc = 1;
			if (lvl.AllCannons.Any(c => c.Fraction == 2)) pc++;
			if (lvl.AllCannons.Any(c => c.Fraction == 3)) pc++;
			if (lvl.AllCannons.Any(c => c.Fraction == 4)) pc++;
			return pc;
		}

		public static IEnumerable<int> ListPlayer(LevelBlueprint lvl)
		{
			yield return 1;
			if (lvl.AllCannons.Any(c => c.Fraction == 2)) yield return 2;
			if (lvl.AllCannons.Any(c => c.Fraction == 3)) yield return 3;
			if (lvl.AllCannons.Any(c => c.Fraction == 4)) yield return 4;
		}

		public static int MaxPointCount(GraphBlueprint w)
		{
			return w.LevelNodes.Count * (GDConstants.SCORE_DIFF_0 + GDConstants.SCORE_DIFF_1 + GDConstants.SCORE_DIFF_2 + GDConstants.SCORE_DIFF_3);
		}

		public static int LevelCount(GraphBlueprint w)
		{
			return w.LevelNodes.Count;
		}
	}
}
