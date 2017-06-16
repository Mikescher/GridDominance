using System;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	public class LevelNodePipe : GameEntity
	{
		private const float THICKNESS = 0.275f * GDConstants.TILE_WIDTH;
		private static readonly Color COLOR = FlatColors.Silver;

		public readonly IWorldNode NodeSource;
		public readonly IWorldNode NodeSink;

		private FPoint _orbStart;
		private FPoint _orbEnd;
		public float Length;

		private readonly FlatCurve13 curvature;

		private FRectangle? rectHorz = null;
		private FRectangle? rectVert = null;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		public LevelNodePipe(GameScreen scrn, IWorldNode start, IWorldNode end, PipeBlueprint.Orientation orientation) : base(scrn, GDConstants.ORDER_MAP_PIPE)
		{
			NodeSource = start;
			NodeSink = end;

			Position = FPoint.MiddlePoint(start.Position, end.Position);
			DrawingBoundingBox = FSize.Diff(start.Position, end.Position) + new FSize(THICKNESS, THICKNESS);

			curvature = GetCurve(start, end, orientation);

			InitCurvature();
		}

		private FlatCurve13 GetCurve(IWorldNode start, IWorldNode end, PipeBlueprint.Orientation o)
		{
			var cw     = (o == PipeBlueprint.Orientation.Clockwise);
			var ccw    = (o == PipeBlueprint.Orientation.Counterclockwise);
			var auto   = (o == PipeBlueprint.Orientation.Auto);
			var direct = (o == PipeBlueprint.Orientation.Direct);

			if (FloatMath.EpsilonEquals(start.Position.X, end.Position.X))
			{
				if (start.Position.Y < end.Position.Y)
					return FlatCurve13.DOWN;
				if (start.Position.Y > end.Position.Y)
					return FlatCurve13.UP;

				return FlatCurve13.POINT;
			}

			if (FloatMath.EpsilonEquals(start.Position.Y, end.Position.Y))
			{
				if (start.Position.X < end.Position.X)
					return FlatCurve13.RIGHT;
				if (start.Position.X > end.Position.X)
					return FlatCurve13.LEFT;

				return FlatCurve13.POINT;
			}

			if (start.Position.X < end.Position.X)
			{
				if (direct) return FlatCurve13.DIRECT;

				if (start.Position.Y < end.Position.Y)
					return (auto || cw) ? FlatCurve13.RIGHT_DOWN : FlatCurve13.DOWN_RIGHT;
				else if (start.Position.Y > end.Position.Y)
					return (auto || ccw) ? FlatCurve13.RIGHT_UP : FlatCurve13.UP_RIGHT;
			}

			if (start.Position.X > end.Position.X)
			{
				if (direct) return FlatCurve13.DIRECT;

				if (start.Position.Y < end.Position.Y)
					return (auto || ccw) ? FlatCurve13.LEFT_DOWN : FlatCurve13.DOWN_LEFT;
				else if (start.Position.Y > end.Position.Y)
					return (auto || cw) ? FlatCurve13.LEFT_UP : FlatCurve13.UP_LEFT;
			}

			throw new Exception("Invalid curvature found");
		}

		private void InitCurvature()
		{
			switch (curvature)
			{
				case FlatCurve13.RIGHT_UP:
				case FlatCurve13.RIGHT:
				case FlatCurve13.RIGHT_DOWN:
					rectHorz = new FRectangle(NodeSource.Position.X, NodeSource.Position.Y, NodeSink.Position.X - NodeSource.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.LEFT_UP:
				case FlatCurve13.LEFT:
				case FlatCurve13.LEFT_DOWN:
					rectHorz = new FRectangle(NodeSink.Position.X, NodeSource.Position.Y, NodeSource.Position.X - NodeSink.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.UP:
				case FlatCurve13.DOWN:
				case FlatCurve13.POINT:
					rectHorz = null;
					break;

				case FlatCurve13.UP_RIGHT:
				case FlatCurve13.DOWN_RIGHT:
					rectHorz = new FRectangle(NodeSource.Position.X, NodeSink.Position.Y, NodeSink.Position.X - NodeSource.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.DOWN_LEFT:
				case FlatCurve13.UP_LEFT:
					rectHorz = new FRectangle(NodeSink.Position.X, NodeSink.Position.Y, NodeSource.Position.X - NodeSink.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.DIRECT:
					rectHorz = null;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (curvature)
			{
				case FlatCurve13.RIGHT:
				case FlatCurve13.LEFT:
				case FlatCurve13.POINT:
					rectVert = null;
					break;

				case FlatCurve13.UP:
				case FlatCurve13.UP_RIGHT:
				case FlatCurve13.UP_LEFT:
					rectVert = new FRectangle(NodeSource.Position.X, NodeSink.Position.Y, 0, NodeSource.Position.Y - NodeSink.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.DOWN:
				case FlatCurve13.DOWN_RIGHT:
				case FlatCurve13.DOWN_LEFT:
					rectVert = new FRectangle(NodeSource.Position.X, NodeSource.Position.Y, 0, NodeSink.Position.Y - NodeSource.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);

					break;

				case FlatCurve13.RIGHT_UP:
				case FlatCurve13.LEFT_UP:
					rectVert = new FRectangle(NodeSink.Position.X, NodeSink.Position.Y, 0, NodeSource.Position.Y - NodeSink.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.RIGHT_DOWN:
				case FlatCurve13.LEFT_DOWN:
					rectVert = new FRectangle(NodeSink.Position.X, NodeSource.Position.Y, 0, NodeSink.Position.Y - NodeSource.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve13.DIRECT:
					rectVert = null;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var orbCenterOffset = LevelNode.DIAMETER / 2 + ConnectionOrb.DIAMETER / 2;

			switch (curvature)
			{
				case FlatCurve13.UP:
				case FlatCurve13.UP_RIGHT:
				case FlatCurve13.UP_LEFT:
					_orbStart = NodeSource.Position + new Vector2(0, -orbCenterOffset);
					break;

				case FlatCurve13.DOWN:
				case FlatCurve13.DOWN_RIGHT:
				case FlatCurve13.DOWN_LEFT:
					_orbStart = NodeSource.Position + new Vector2(0, +orbCenterOffset);
					break;

				case FlatCurve13.RIGHT_UP:
				case FlatCurve13.RIGHT:
				case FlatCurve13.RIGHT_DOWN:
					_orbStart = NodeSource.Position + new Vector2(+orbCenterOffset, 0);
					break;

				case FlatCurve13.LEFT_UP:
				case FlatCurve13.LEFT:
				case FlatCurve13.LEFT_DOWN:
					_orbStart = NodeSource.Position + new Vector2(-orbCenterOffset, 0);
					break;

				case FlatCurve13.DIRECT:
					_orbStart = NodeSource.Position + (NodeSink.Position - NodeSource.Position).WithLength(orbCenterOffset);
					break;

				case FlatCurve13.POINT:
					throw new ArgumentOutOfRangeException();

				default:
					throw new ArgumentOutOfRangeException();
			}

			_orbEnd = NodeSink.Position;
			
			Length = (_orbStart - _orbEnd).ManhattenLength();
		}

		public override void OnInitialize(EntityManager manager)
		{
			//
		}

		public override void OnRemove()
		{
			//
		}

		protected override void OnUpdate(SAMTime gameTime, InputState istate)
		{
			//
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			if (rectHorz != null) sbatch.FillRectangle(rectHorz.Value, COLOR);
			if (rectVert != null) sbatch.FillRectangle(rectVert.Value, COLOR);
		}

		public FPoint GetOrbPosition(float distance)
		{
			switch (curvature)
			{
				case FlatCurve13.UP:
				case FlatCurve13.DOWN:
				case FlatCurve13.LEFT:
				case FlatCurve13.RIGHT:
					return _orbStart + (_orbEnd - _orbStart).WithLength(distance);

				case FlatCurve13.LEFT_DOWN:
					return
						(distance < (_orbStart.X - _orbEnd.X))
						? new FPoint(_orbStart.X - distance, _orbStart.Y)
						: new FPoint(_orbEnd.X, _orbStart.Y + (distance - (_orbStart.X - _orbEnd.X)));

				case FlatCurve13.LEFT_UP:
					return
						(distance < (_orbStart.X - _orbEnd.X))
						? new FPoint(_orbStart.X - distance, _orbStart.Y)
						: new FPoint(_orbEnd.X, _orbStart.Y - (distance - (_orbStart.X - _orbEnd.X)));

				case FlatCurve13.RIGHT_DOWN:
					return
						(distance < (_orbEnd.X - _orbStart.X))
						? new FPoint(_orbStart.X + distance, _orbStart.Y)
						: new FPoint(_orbEnd.X, _orbStart.Y + (distance - (_orbEnd.X - _orbStart.X)));

				case FlatCurve13.RIGHT_UP:
					return
						(distance < (_orbEnd.X - _orbStart.X))
						? new FPoint(_orbStart.X + distance, _orbStart.Y)
						: new FPoint(_orbEnd.X, _orbStart.Y - (distance - (_orbEnd.X - _orbStart.X)));

				case FlatCurve13.DOWN_RIGHT:
					return
						(distance < (_orbEnd.Y - _orbStart.Y))
						? new FPoint(_orbStart.X, _orbStart.Y + distance)
						: new FPoint(_orbStart.X + (distance - (_orbEnd.Y - _orbStart.Y)), _orbEnd.Y);

				case FlatCurve13.DOWN_LEFT:
					return
						(distance < (_orbEnd.Y - _orbStart.Y))
						? new FPoint(_orbStart.X, _orbStart.Y + distance)
						: new FPoint(_orbStart.X - (distance - (_orbEnd.Y - _orbStart.Y)), _orbEnd.Y);


				case FlatCurve13.UP_RIGHT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new FPoint(_orbStart.X, _orbStart.Y - distance)
						: new FPoint(_orbStart.X + (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);

				case FlatCurve13.UP_LEFT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new FPoint(_orbStart.X, _orbStart.Y - distance)
						: new FPoint(_orbStart.X - (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);

				case FlatCurve13.POINT:
					return _orbStart;

				case FlatCurve13.DIRECT:
					return _orbStart + (_orbEnd - _orbStart).WithLength(distance);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
