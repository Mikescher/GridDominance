using System.Linq;
using FarseerPhysics;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.Common;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.NormalGameScreen.Entities
{
	public class BlackHole : GameEntity
	{
		public  const float ANIMATION_DURATION = 1.8f;

		public override FPoint Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Gold;

		private readonly float _radius;
		private readonly float _diameter;
		private readonly float _power;

		private float _animationTimer = 0f;

		public BlackHole(GDGameScreen scrn, BlackHoleBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_BLACKHOLE)
		{
			var pos   = new FPoint(blueprint.X, blueprint.Y);

			_diameter = blueprint.Diameter;
			_radius = _diameter / 2f;
			_power = blueprint.Power;

			Position = pos;

			DrawingBoundingBox = new FSize(_diameter, _diameter);

			this.GDOwner().GDBackground.RegisterBlockedCircle(new FCircle(pos, _diameter / 2f));
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
			_animationTimer += gameTime.ElapsedSeconds;

			foreach (var bullet in Manager.Enumerate().OfType<Bullet>())
			{
				var pp = bullet.Position - Position;

				var ppls = pp.LengthSquared();

				if (ppls < FloatMath.EPSILON6) continue;

				var force = _power / pp.LengthSquared();
				var vecForce = pp.WithLength(force);
				
				if (ppls <= (_radius / 3f) * (_radius / 3f))
				{
					bullet.PhysicsBody.LinearDamping = 1000;
					bullet.DisintegrateIntoVortex();
				}

				bullet.PhysicsBody.ApplyForce(ConvertUnits.ToSimUnits(vecForce));
			}
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			CommonObstacleRenderer.DrawBlackHole(sbatch, Position, _animationTimer, _diameter, _power);
		}
	}
}
