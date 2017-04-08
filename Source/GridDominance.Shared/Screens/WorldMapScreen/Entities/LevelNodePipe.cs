using System;
using GridDominance.Graphfileformat.Parser;
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
		private static readonly Color COLOR_INACTIVE = FlatColors.Silver;
		private static readonly Color COLOR_ACTIVE   = FlatColors.Silver;

		public readonly LevelNode NodeSource;
		public readonly LevelNode NodeSink;

		private Vector2 _orbStart;
		private Vector2 _orbEnd;
		public float Length;

		private readonly FlatCurve12 curvature;

		private FRectangle? rectHorz = null;
		private FRectangle? rectVert = null;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		public LevelNodePipe(GameScreen scrn, LevelNode start, LevelNode end, WGPipe.Orientation orientation) : base(scrn, -2)
		{
			NodeSource = start;
			NodeSink = end;

			Position = (start.Position + end.Position) / 2;
			DrawingBoundingBox = FSize.Diff(start.Position, end.Position) + new FSize(THICKNESS, THICKNESS);

			curvature = GetCurve(start, end, orientation);

			InitCurvature();
		}

		private FlatCurve12 GetCurve(LevelNode start, LevelNode end, WGPipe.Orientation o)
		{
			var cw   = (o == WGPipe.Orientation.Clockwise);
			var ccw  = (o == WGPipe.Orientation.Counterclockwise);
			var auto = (o == WGPipe.Orientation.Auto);

			if (FloatMath.EpsilonEquals(start.Position.X, end.Position.X))
			{
				if (start.Position.Y < end.Position.Y)
					return FlatCurve12.DOWN;
				if (start.Position.Y > end.Position.Y)
					return FlatCurve12.UP;

				return FlatCurve12.POINT;
			}

			if (FloatMath.EpsilonEquals(start.Position.Y, end.Position.Y))
			{
				if (start.Position.X < end.Position.X)
					return FlatCurve12.RIGHT;
				if (start.Position.X > end.Position.X)
					return FlatCurve12.LEFT;

				return FlatCurve12.POINT;
			}

			if (start.Position.X < end.Position.X)
			{
				if (start.Position.Y < end.Position.Y)
					return (auto || cw) ? FlatCurve12.RIGHT_DOWN : FlatCurve12.DOWN_RIGHT;
				else if (start.Position.Y > end.Position.Y)
					return (auto || ccw) ? FlatCurve12.RIGHT_UP : FlatCurve12.UP_RIGHT;
			}

			if (start.Position.X > end.Position.X)
			{
				if (start.Position.Y < end.Position.Y)
					return (auto || ccw) ? FlatCurve12.LEFT_DOWN : FlatCurve12.DOWN_LEFT;
				else if (start.Position.Y > end.Position.Y)
					return (auto || cw) ? FlatCurve12.LEFT_UP : FlatCurve12.UP_LEFT;
			}

			throw new Exception("Invalid curvature found");
		}

		private void InitCurvature()
		{
			switch (curvature)
			{
				case FlatCurve12.RIGHT_UP:
				case FlatCurve12.RIGHT:
				case FlatCurve12.RIGHT_DOWN:
					rectHorz = new FRectangle(NodeSource.Position.X, NodeSource.Position.Y, NodeSink.Position.X - NodeSource.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve12.LEFT_UP:
				case FlatCurve12.LEFT:
				case FlatCurve12.LEFT_DOWN:
					rectHorz = new FRectangle(NodeSink.Position.X, NodeSource.Position.Y, NodeSource.Position.X - NodeSink.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve12.UP:
				case FlatCurve12.DOWN:
				case FlatCurve12.POINT:
					rectHorz = null;
					break;

				case FlatCurve12.UP_RIGHT:
				case FlatCurve12.DOWN_RIGHT:
					rectHorz = new FRectangle(NodeSource.Position.X, NodeSink.Position.Y, NodeSink.Position.X - NodeSource.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve12.DOWN_LEFT:
				case FlatCurve12.UP_LEFT:
					rectHorz = new FRectangle(NodeSink.Position.X, NodeSink.Position.Y, NodeSource.Position.X - NodeSink.Position.X, 0).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (curvature)
			{
				case FlatCurve12.RIGHT:
				case FlatCurve12.LEFT:
				case FlatCurve12.POINT:
					rectVert = null;
					break;

				case FlatCurve12.UP:
				case FlatCurve12.UP_RIGHT:
				case FlatCurve12.UP_LEFT:
					rectVert = new FRectangle(NodeSource.Position.X, NodeSink.Position.Y, 0, NodeSource.Position.Y - NodeSink.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve12.DOWN:
				case FlatCurve12.DOWN_RIGHT:
				case FlatCurve12.DOWN_LEFT:
					rectVert = new FRectangle(NodeSource.Position.X, NodeSource.Position.Y, 0, NodeSink.Position.Y - NodeSource.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);

					break;

				case FlatCurve12.RIGHT_UP:
				case FlatCurve12.LEFT_UP:
					rectVert = new FRectangle(NodeSink.Position.X, NodeSink.Position.Y, 0, NodeSource.Position.Y - NodeSink.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				case FlatCurve12.RIGHT_DOWN:
				case FlatCurve12.LEFT_DOWN:
					rectVert = new FRectangle(NodeSink.Position.X, NodeSource.Position.Y, 0, NodeSink.Position.Y - NodeSource.Position.Y).AsInflated(THICKNESS / 2, THICKNESS / 2);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			var orbCenterOffset = LevelNode.DIAMETER / 2 + ConnectionOrb.DIAMETER / 2;

			switch (curvature)
			{
				case FlatCurve12.UP:
				case FlatCurve12.UP_RIGHT:
				case FlatCurve12.UP_LEFT:
					_orbStart = NodeSource.Position + new Vector2(0, -orbCenterOffset);
					break;

				case FlatCurve12.DOWN:
				case FlatCurve12.DOWN_RIGHT:
				case FlatCurve12.DOWN_LEFT:
					_orbStart = NodeSource.Position + new Vector2(0, +orbCenterOffset);
					break;

				case FlatCurve12.RIGHT_UP:
				case FlatCurve12.RIGHT:
				case FlatCurve12.RIGHT_DOWN:
					_orbStart = NodeSource.Position + new Vector2(+orbCenterOffset, 0);
					break;

				case FlatCurve12.LEFT_UP:
				case FlatCurve12.LEFT:
				case FlatCurve12.LEFT_DOWN:
					_orbStart = NodeSource.Position + new Vector2(-orbCenterOffset, 0);
					break;

				case FlatCurve12.POINT:
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
			var c = NodeSource.LevelData.CompletionCount > 0 ? COLOR_ACTIVE : COLOR_INACTIVE;

			if (rectHorz != null) sbatch.FillRectangle(rectHorz.Value, c);
			if (rectVert != null) sbatch.FillRectangle(rectVert.Value, c);
		}

		public Vector2 GetOrbPosition(float distance)
		{
			switch (curvature)
			{
				case FlatCurve12.UP:
				case FlatCurve12.DOWN:
				case FlatCurve12.LEFT:
				case FlatCurve12.RIGHT:
					return _orbStart + (_orbEnd - _orbStart).WithLength(distance);

				case FlatCurve12.LEFT_DOWN:
					return
						(distance < (_orbStart.X - _orbEnd.X))
						? new Vector2(_orbStart.X - distance, _orbStart.Y)
						: new Vector2(_orbEnd.X, _orbStart.Y + (distance - (_orbStart.X - _orbEnd.X)));

				case FlatCurve12.LEFT_UP:
					return
						(distance < (_orbStart.X - _orbEnd.X))
						? new Vector2(_orbStart.X - distance, _orbStart.Y)
						: new Vector2(_orbEnd.X, _orbStart.Y - (distance - (_orbStart.X - _orbEnd.X)));

				case FlatCurve12.RIGHT_DOWN:
					return
						(distance < (_orbEnd.X - _orbStart.X))
						? new Vector2(_orbStart.X + distance, _orbStart.Y)
						: new Vector2(_orbEnd.X, _orbStart.Y + (distance - (_orbEnd.X - _orbStart.X)));

				case FlatCurve12.RIGHT_UP:
					return
						(distance < (_orbEnd.X - _orbStart.X))
						? new Vector2(_orbStart.X + distance, _orbStart.Y)
						: new Vector2(_orbEnd.X, _orbStart.Y - (distance - (_orbEnd.X - _orbStart.X)));

				case FlatCurve12.DOWN_RIGHT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new Vector2(_orbStart.X, _orbStart.Y + distance)
						: new Vector2(_orbEnd.X + (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);

				case FlatCurve12.DOWN_LEFT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new Vector2(_orbStart.X, _orbStart.Y + distance)
						: new Vector2(_orbEnd.X - (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);


				case FlatCurve12.UP_RIGHT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new Vector2(_orbStart.X, _orbStart.Y - distance)
						: new Vector2(_orbStart.X + (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);

				case FlatCurve12.UP_LEFT:
					return
						(distance < (_orbStart.Y - _orbEnd.Y))
						? new Vector2(_orbStart.X, _orbStart.Y - distance)
						: new Vector2(_orbStart.X - (distance - (_orbStart.Y - _orbEnd.Y)), _orbEnd.Y);
					
				case FlatCurve12.POINT:
					return _orbStart;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
