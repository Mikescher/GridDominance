using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.BatchRenderer.TextureAtlases;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class MirrorCircle : GameEntity
	{
		public const float MARGIN_TEX = 8f;
		
		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		private readonly float _diameter;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;
		
		public MirrorCircle(GDGameScreen scrn, MirrorCircleBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			var pos   = new FPoint(blueprint.X, blueprint.Y);

			_diameter = blueprint.Diameter;

			Position = pos;

			DrawingBoundingBox = new FSize(_diameter + 2 * MARGIN_TEX, _diameter + 2 * MARGIN_TEX);

			this.GDOwner().GDBackground.RegisterBlockedCircle(new FCircle(pos, _diameter / 2f));
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), 0, BodyType.Static);

			PhysicsFixture = FixtureFactory.AttachCircle(ConvertUnits.ToSimUnits(_diameter/2f), 1, PhysicsBody, this);
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
			CommonObstacleRenderer.DrawMirrorCircle(sbatch, Position, _diameter);
		}
	}
}
