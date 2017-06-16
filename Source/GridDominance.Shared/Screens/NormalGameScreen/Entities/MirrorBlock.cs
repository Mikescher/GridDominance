using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.ColorHelper;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class MirrorBlock : GameEntity
	{
		public const float CORNER_SIZE = 16f;
		
		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		private readonly float _width;
		private readonly float _height;
		private readonly float _rotation;

		private readonly FRectangle _bounds;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;

		public MirrorBlock(GDGameScreen scrn, MirrorBlockBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			var pos = new FPoint(blueprint.X, blueprint.Y);

			_width = blueprint.Width;
			_height = blueprint.Height;
			_rotation = FloatMath.ToRadians(blueprint.Rotation);

			_bounds = FRectangle.CreateByCenter(pos, _width, _height);

			Position = pos;

			DrawingBoundingBox = new FSize(_width, _height);

			this.GDOwner().GDBackground.RegisterBlockedBlock(_bounds, _rotation);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), _rotation, BodyType.Static);

			PhysicsFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(_width), ConvertUnits.ToSimUnits(_height), 1, Vector2.Zero, PhysicsBody, this);
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
				Color.White, Color.White, FlatColors.Concrete,
				Textures.TexMirrorBlockEdge, Textures.TexMirrorBlockCorner, Textures.TexPixel,
				CORNER_SIZE,
				_rotation);
		}
	}
}
