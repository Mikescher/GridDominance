using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelfileformat.Parser;
using GridDominance.Levelformat.Parser;
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
		public FRectangle BoundingRect;
		public FRectangle BoundingViewport;

		public LevelNode InitialNode;

		public LevelGraph(GDWorldMapScreen s)
		{
			screen = s;
		}

		public void Init(WorldGraphFile g)
		{
			foreach (var bpNode in g.Nodes)
			{
				LevelFile f;
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

			var initNodeCandidates = Nodes.ToList();

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

					initNodeCandidates.Remove(sinknode);
				}
			}

			InitialNode = initNodeCandidates.First(); //TODO Better calc or even better define by file

			BoundingRect = FRectangle.CreateOuter(Nodes.Select(n => n.DrawingBoundingRect));

			BoundingViewport = BoundingRect
				.AsInflated(LevelNode.DIAMETER, LevelNode.DIAMETER)
				.SetRatioOverfitKeepCenter(GDConstants.VIEW_WIDTH * 1f / GDConstants.VIEW_HEIGHT);
		}

		private LevelNode AddLevelNode(float x, float y, LevelFile f)
		{
			var data = MainGame.Inst.Profile.GetLevelData(f.UniqueID);
			var pos = new Vector2(GDConstants.TILE_WIDTH * (x + 0.5f), GDConstants.TILE_WIDTH * (y + 0.5f));

			var node = new LevelNode(screen, pos, f, data);

			screen.Entities.AddEntity(node);
			Nodes.Add(node);

			return node;
		}
	}
}
