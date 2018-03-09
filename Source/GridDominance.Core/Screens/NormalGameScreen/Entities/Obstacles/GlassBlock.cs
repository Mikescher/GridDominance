using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.Obstacles
{
	public class GlassBlock : GameEntity
	{
		public const float CORNER_SIZE = 4f;
		public const float MARKER_WIDTH = 0.001f;
		public const float CORNER_WIDTH = 0.025f;
		public const float REFRACTION_INDEX = GlassBlockBlueprint.REFRACTION_INDEX;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		public readonly GlassBlockBlueprint Blueprint;

		private readonly float _width;
		private readonly float _height;
		private readonly float _rotation;

		private readonly FRotatedRectangle _bounds;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;

		public GlassBlock(GDGameScreen scrn, GlassBlockBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			Blueprint = blueprint;

			var pos = new FPoint(blueprint.X, blueprint.Y);

			_width = blueprint.Width;
			_height = blueprint.Height;
			_rotation = FloatMath.ToRadians(blueprint.Rotation);

			_bounds = new FRotatedRectangle(pos, _width, _height, _rotation);

			Position = pos;

			DrawingBoundingBox = _bounds.OuterSize;

			this.GDOwner().GDBackground.RegisterBlockedBlock(_bounds.WithNoRotation(), _rotation);
		}

		public override void OnInitialize(EntityManager manager)
		{
			var pw = this.GDManager().PhysicsWorld;

			var w = ConvertUnits.ToSimUnits(_width);
			var h = ConvertUnits.ToSimUnits(_height);
			var p = ConvertUnits2.ToSimUnits(Position);

			PhysicsBody = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, this);
			PhysicsFixture = FixtureFactory.AttachRectangle(w, h, 1, Vector2.Zero, PhysicsBody, this);

			var rn = new MarkerRefractionEdge { Source = this, Side = FlatAlign4.NN };
			var re = new MarkerRefractionEdge { Source = this, Side = FlatAlign4.EE };
			var rs = new MarkerRefractionEdge { Source = this, Side = FlatAlign4.SS };
			var rw = new MarkerRefractionEdge { Source = this, Side = FlatAlign4.WW };

			var rne = new MarkerRefractionCorner { Source = this, Side = FlatAlign4C.NE };
			var rse = new MarkerRefractionCorner { Source = this, Side = FlatAlign4C.SE };
			var rsw = new MarkerRefractionCorner { Source = this, Side = FlatAlign4C.SW };
			var rnw = new MarkerRefractionCorner { Source = this, Side = FlatAlign4C.NW };

			var bodyN = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rn);
			FixtureFactory.AttachRectangle(w, MARKER_WIDTH, 1, new Vector2(0, -(h - MARKER_WIDTH) / 2f), bodyN, rn);

			var bodyE = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, re);
			FixtureFactory.AttachRectangle(MARKER_WIDTH, h, 1, new Vector2(+(w - MARKER_WIDTH) / 2f, 0), bodyE, re);

			var bodyS = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rs);
			FixtureFactory.AttachRectangle(w, MARKER_WIDTH, 1, new Vector2(0, +(h - MARKER_WIDTH) / 2f), bodyS, rs);

			var bodyW = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rw);
			FixtureFactory.AttachRectangle(MARKER_WIDTH, h, 1, new Vector2(-(w - MARKER_WIDTH) / 2f, 0), bodyW, rw);

			
			var bodyNE = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rne);
			FixtureFactory.AttachRectangle(CORNER_WIDTH, CORNER_WIDTH, 1, new Vector2(+w/2, -h/2), bodyNE, rne);

			var bodySE = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rse);
			FixtureFactory.AttachRectangle(CORNER_WIDTH, CORNER_WIDTH, 1, new Vector2(+w/2, +h/2), bodySE, rse);

			var bodySW = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rsw);
			FixtureFactory.AttachRectangle(CORNER_WIDTH, CORNER_WIDTH, 1, new Vector2(-w/2, +h/2), bodySW, rsw);

			var bodyNW = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rnw);
			FixtureFactory.AttachRectangle(CORNER_WIDTH, CORNER_WIDTH, 1, new Vector2(-w/2, -h/2), bodyNW, rnw);
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
			CommonObstacleRenderer.DrawGlassBlock(sbatch, _bounds);
		}
	}
}
