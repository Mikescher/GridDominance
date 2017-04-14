using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Parser;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.ScreenGame.Entities
{
	public class VoidWall : GameEntity
	{
		public const float WIDTH = GDConstants.TILE_WIDTH / 16f;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.LightSteelBlue;

		private readonly float _rotation;
		private readonly float _length;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;

		public VoidWall(GDGameScreen scrn, LPVoidWall blueprint) : base(scrn)
		{
			var pos   = new Vector2(blueprint.X, blueprint.Y);
			_rotation = FloatMath.DegreesToRadians * blueprint.Rotation;
			_length = blueprint.Length;
			Position = pos;
			DrawingBoundingBox = pos.Rotate(_rotation).ToSize().AtLeast(WIDTH, WIDTH);
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits.ToSimUnits(Position), 0, BodyType.Static);

			PhysicsFixture = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(_length), ConvertUnits.ToSimUnits(WIDTH), 1, Vector2.Zero, PhysicsBody, this);

			PhysicsBody.Rotation = _rotation;
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
			sbatch.FillRectangle(FRectangle.CreateByCenter(Position, _length, WIDTH), Color.WhiteSmoke, _rotation);
		}
	}
}
