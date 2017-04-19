using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.LogProtocol;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelGraph
	{
		private readonly GDWorldMapScreen screen;

		public readonly List<LevelNode> Nodes = new List<LevelNode>();
		public List<LevelNode> RootNodes = new List<LevelNode>();
		public FRectangle BoundingRect;
		public FRectangle BoundingViewport;

		public RootNode InitialNode;

		public LevelGraph(GDWorldMapScreen s)
		{
			screen = s;
		}

		public void Init(GraphBlueprint g)
		{
			InitEntities(g);

			InitPipes(g);

			InitEnabled();

			InitViewport();
		}

		private void InitEntities(GraphBlueprint g)
		{
			foreach (var bpNode in g.Nodes)
			{
				LevelBlueprint f;
				if (Levels.LEVELS.TryGetValue(bpNode.LevelID, out f))
				{
					var data = MainGame.Inst.Profile.GetLevelData(f.UniqueID);
					var pos = new Vector2(bpNode.X, bpNode.Y);

					var node = new LevelNode(screen, pos, f, data);

					screen.Entities.AddEntity(node);
					Nodes.Add(node);
				}
				else
				{
					SAMLog.Error("LevelGraph", $"Cannot find id {bpNode.LevelID:B} for graph");
				}
			}

			InitialNode = new RootNode(screen, new Vector2(g.RootNode.X, g.RootNode.Y));
			screen.Entities.AddEntity(InitialNode);
		}

		private void InitPipes(GraphBlueprint g)
		{
			foreach (var pipe in g.RootNode.OutgoingPipes)
			{
				var sinknode = Nodes.FirstOrDefault(n => n.Level.UniqueID == pipe.Target);

				if (sinknode == null)
				{
					SAMLog.Error("LevelGraph", $"Cannot find node with id {pipe.Target:B} in graph for pipe sink");
					continue;
				}

				InitialNode.CreatePipe(sinknode, pipe.PipeOrientation);
			}

			RootNodes = Nodes.ToList();
			foreach (var bpNode in g.Nodes)
			{
				var sourcenode = Nodes.FirstOrDefault(n => n.Level.UniqueID == bpNode.LevelID);
				if (sourcenode == null)
				{
					SAMLog.Error("LevelGraph", $"Cannot find node with id {bpNode.LevelID:B} in graph for pipe source");
					continue;
				}

				foreach (var pipe in bpNode.OutgoingPipes)
				{
					var sinknode = Nodes.FirstOrDefault(n => n.Level.UniqueID == pipe.Target);

					if (sinknode == null)
					{
						SAMLog.Error("LevelGraph", $"Cannot find node with id {pipe.Target:B} in graph for pipe sink");
						continue;
					}

					sourcenode.CreatePipe(sinknode, pipe.PipeOrientation);

					RootNodes.Remove(sinknode);
				}
			}
		}

		private void InitEnabled()
		{
			Stack<LevelNode> nstack = new Stack<LevelNode>();
			foreach (var n in RootNodes)
			{
				n.NodeEnabled = true;
				if (n.LevelData.HasAnyCompleted())
				{
					foreach (var nextnode in n.NextLinkedNodes)
					{
						nstack.Push(nextnode);
					}
				}
			}
			foreach (var nextnode in InitialNode.NextLinkedNodes)
			{
				nstack.Push(nextnode);
			}


			while (nstack.Any())
			{
				var n = nstack.Pop();

				n.NodeEnabled = true;
				if (n.LevelData.HasAnyCompleted())
				{
					foreach (var nextnode in n.NextLinkedNodes)
					{
						if (!nextnode.NodeEnabled) nstack.Push(nextnode);
					}
				}
			}
		}

		private void InitViewport()
		{
			BoundingRect = FRectangle.CreateOuter(Nodes.Select(n => n.DrawingBoundingRect));

			BoundingViewport = BoundingRect
				.AsInflated(LevelNode.DIAMETER, LevelNode.DIAMETER)
				.SetRatioOverfitKeepCenter(GDConstants.VIEW_WIDTH * 1f / GDConstants.VIEW_HEIGHT);
		}

	}
}
