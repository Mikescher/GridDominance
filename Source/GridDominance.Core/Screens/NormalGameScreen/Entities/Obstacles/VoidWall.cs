using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.NormalGameScreen.Physics;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.DebugTools;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities.Obstacles
{
	public class VoidWall : GameEntity
	{
		public const float WIDTH      = VoidWallBlueprint.DEFAULT_WIDTH;
		public const float MARGIN_TEX = 8f;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Transparent;

		private readonly float _rotation;
		private readonly float _length;
		private readonly FRectangle[] _rectsUnrotated;

		public Body PhysicsBody;
		public Fixture PhysicsFixture;
		
		public VoidWall(GDGameScreen scrn, VoidWallBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_WALL)
		{
			var pos   = new FPoint(blueprint.X, blueprint.Y);

			_rotation = FloatMath.DegreesToRadians * blueprint.Rotation;
			_length = blueprint.Length;
			_rectsUnrotated = CommonWallRenderer.CreateVoidWallRenderRects(pos, _length, _rotation);

			Position = pos;

			DrawingBoundingBox = new Vector2(_length, 0).Rotate(_rotation).ToAbsFSize().AtLeast(WIDTH, WIDTH);

			this.GDOwner().GDBackground.RegisterBlockedLine(pos - Vector2.UnitX.RotateWithLength(_rotation, _length/2f), pos + Vector2.UnitX.RotateWithLength(_rotation, _length / 2f));
		}

		public override void OnInitialize(EntityManager manager)
		{
			PhysicsBody = BodyFactory.CreateBody(this.GDManager().PhysicsWorld, ConvertUnits2.ToSimUnits(Position), 0, BodyType.Static);
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
			CommonWallRenderer.DrawVoidWall_BG(sbatch, _length, _rotation, _rectsUnrotated);

#if DEBUG
			if (DebugSettings.Get("DebugEntityBoundaries"))
			{
				foreach (var r in _rectsUnrotated)
				{
					sbatch.DrawRectangleRot(r, Color.Cyan, _rotation);
				}
			}
#endif
		}

		protected override void OnDrawOrderedForegroundLayer(IBatchRenderer sbatch)
		{
			CommonWallRenderer.DrawVoidWall_FG(sbatch, _length, _rotation, _rectsUnrotated);
		}
	}
}
