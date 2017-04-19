using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Screens.Entities.MouseArea;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class RootNode : GameEntity, IWorldNode
	{
		public const float DIAMETER = 3f * GDConstants.TILE_WIDTH;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor => Color.SandyBrown;

		private GameEntityMouseArea clickAreaThis;

		public readonly List<LevelNode> NextLinkedNodes = new List<LevelNode>();
		public readonly List<LevelNodePipe> OutgoingPipes = new List<LevelNodePipe>();

		public RootNode(GDWorldMapScreen scrn, Vector2 pos) : base(scrn, GDConstants.ORDER_MAP_NODE)
		{
			Position = pos;
			DrawingBoundingBox = new FSize(DIAMETER, DIAMETER);
		}

		public override void OnInitialize(EntityManager manager)
		{
			clickAreaThis = AddClickMouseArea(FRectangle.CreateByCenter(0, 0, DIAMETER, DIAMETER), OnClick);
		}

		public void CreatePipe(LevelNode target, PipeBlueprint.Orientation orientation)
		{
			NextLinkedNodes.Add(target);

			var p = new LevelNodePipe(Owner, this, target, orientation);
			OutgoingPipes.Add(p);
			Manager.AddEntity(p);
		}

		public override void OnRemove()
		{
			throw new NotImplementedException();
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			throw new NotImplementedException();
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			// Gray bg
			// grayblue outline
			// black center
			// fire particle spawner (circular) in center
			throw new NotImplementedException();
		}
	}
}
