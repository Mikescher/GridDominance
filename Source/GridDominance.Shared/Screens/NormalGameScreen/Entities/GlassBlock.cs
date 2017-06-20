using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.GameMath.Geometry.Alignment;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class GlassBlock : GameEntity
	{
		public const float CORNER_SIZE = 4f;
		private const float MARKER_WIDTH = 0.001f;
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

		public override void OnInitialize(EntityManager manager) //TODO prevent strange refraction effects in corner (perhaps prevent refraction in corners all together)
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

			var bodyN = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rn);
			FixtureFactory.AttachRectangle(w, MARKER_WIDTH, 1, new Vector2(0, -(h - MARKER_WIDTH) / 2f), bodyN, rn);

			var bodyE = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, re);
			FixtureFactory.AttachRectangle(MARKER_WIDTH, h, 1, new Vector2(+(w - MARKER_WIDTH) / 2f, 0), bodyE, re);

			var bodyS = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rs);
			FixtureFactory.AttachRectangle(w, MARKER_WIDTH, 1, new Vector2(0, +(h - MARKER_WIDTH) / 2f), bodyS, rs);

			var bodyW = BodyFactory.CreateBody(pw, p, _rotation, BodyType.Static, rw);
			FixtureFactory.AttachRectangle(MARKER_WIDTH, h, 1, new Vector2(-(w - MARKER_WIDTH) / 2f, 0), bodyW, rw);
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
			SimpleRenderHelper.Draw9Patch(
				sbatch, 
				_bounds, 
				Color.White, Color.White, Color.White, 
				Textures.TexGlassEdge, Textures.TexGlassCorner, Textures.TexGlassFill, 
				CORNER_SIZE);
		}
	}
}
