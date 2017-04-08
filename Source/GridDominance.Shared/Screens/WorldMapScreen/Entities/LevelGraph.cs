using System.Collections.Generic;
using System.Linq;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelGraph
	{
		private readonly GDWorldMapScreen screen;

		private readonly List<LevelNode> nodes = new List<LevelNode>();
		public FRectangle BoundingRect;
		public FRectangle BoundingViewport;

		public LevelGraph(GDWorldMapScreen s)
		{
			screen = s;
		}

		public void Init()
		{
			var n1 = AddLevelNode(4, 10, Levels.LEVEL_001);
			var n2 = AddLevelNode(10, 10, Levels.LEVEL_002);
			var n3 = AddLevelNode(22, 10, Levels.LEVEL_003);
			var n4 = AddLevelNode(16, 16, Levels.LEVEL_003);
			var n5 = AddLevelNode(28, 10, Levels.LEVEL_003);
			var n6 = AddLevelNode(34, 10, Levels.LEVEL_003);
			var n7 = AddLevelNode(40, 10, Levels.LEVEL_003);

			n1.NextLinkedNodes.Add(n2);
			n2.NextLinkedNodes.Add(n3);
			n2.NextLinkedNodes.Add(n4);
			n3.NextLinkedNodes.Add(n5);
			n5.NextLinkedNodes.Add(n6);
			n6.NextLinkedNodes.Add(n7);

			foreach (var n in nodes) n.CreatePipes();

			BoundingRect = FRectangle.CreateOuter(nodes.Select(n => n.DrawingBoundingRect));

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
			nodes.Add(node);

			return node;
		}
	}
}
