using System;
using System.Collections.Generic;
using System.Linq;
using GridDominance.Graphfileformat.Parser;
using GridDominance.Levelfileformat.Parser;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelGraph
	{
		private readonly GDWorldMapScreen screen;

		public readonly List<LevelNode> Nodes = new List<LevelNode>();
		public FRectangle BoundingRect;
		public FRectangle BoundingViewport;

		public LevelNode InitialNode;
		public LevelNode FinalNode;

		public LevelGraph(GDWorldMapScreen s)
		{
			screen = s;
		}

		public void Init(WorldGraphFile g)
		{
			foreach (var bpNode in g.Nodes)
			{
				var f = Levels.LEVELS[bpNode.LevelID];

				var data = MainGame.Inst.Profile.GetLevelData(f.UniqueID);
				var pos = new Vector2(bpNode.X, bpNode.Y);

				var node = new LevelNode(screen, pos, f, data);

				screen.Entities.AddEntity(node);
				Nodes.Add(node);

				if (node.OutgoingPipes.Count == 0) FinalNode = node; //TODO Better calc or even better define by file
			}

			var initNodeCandidates = Nodes.ToList();

			foreach (var bpNode in g.Nodes)
			{
				var sourcenode = Nodes.Single(n => n.Level.UniqueID == bpNode.LevelID);
				foreach (var pipe in bpNode.OutgoingPipes)
				{
					var sinknode = Nodes.Single(n => n.Level.UniqueID == pipe.Target);

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
