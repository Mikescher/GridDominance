using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.RenderHelper;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class GlassBlock : GameEntity
	{
		public const float CORNER_SIZE = 4f;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		private readonly float _width;
		private readonly float _height;

		private readonly FRectangle _bounds;

		public Body PhysicsBody;
		
		public GlassBlock(GDGameScreen scrn, GlassBlockBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			var pos = new Vector2(blueprint.X, blueprint.Y);

			_width = blueprint.Width;
			_height = blueprint.Height;

			_bounds = FRectangle.CreateByCenter(pos, _width, _height);

			Position = pos;

			DrawingBoundingBox = new FSize(_width, _height);

			this.GDOwner().GDBackground.RegisterBlockedBlock(_bounds);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateRectangle(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(_width), ConvertUnits.ToSimUnits(_height), 1, ConvertUnits.ToSimUnits(Position), 0, BodyType.Static, this);

			PhysicsBody.Friction = 0;
			PhysicsBody.Restitution = 0;
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
			SimpleRenderHelper.DrawDef9Rect(sbatch, _bounds, Color.White, Color.White, Color.White, Textures.TexGlassEdge, Textures.TexGlassCorner, Textures.TexGlassFill, CORNER_SIZE);
		}
	}
}
