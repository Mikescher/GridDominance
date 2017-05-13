using System.Linq;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Shared.Resources;
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
		private const float ANIMATION_DURATION = 1.8f;
		private const float CLAMPING_FACTOR    = 10f;

		public override Vector2 Position { get; }
		public override FSize DrawingBoundingBox { get; }
		public override Color DebugIdentColor { get; } = Color.Gold;

		private readonly float _radius;
		private readonly float _diameter;
		private readonly float _power;

		private float _animationTimer = 0f;

		public BlackHole(GDGameScreen scrn, BlackHoleBlueprint blueprint) : base(scrn, GDConstants.ORDER_GAME_BLACKHOLE)
		{
			var pos   = new Vector2(blueprint.X, blueprint.Y);

			_diameter = blueprint.Diameter;
			_radius = _diameter / 2f;
			_power = blueprint.Power;

			Position = pos;

			DrawingBoundingBox = new FSize(_diameter, _diameter);
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
			var fillColor = (_power < 0) ? Color.Black : Color.White;

			sbatch.DrawCentered(Textures.TexCircle, Position, _diameter, _diameter, fillColor);
			sbatch.DrawCentered(Textures.TexVortex2, Position, _diameter, _diameter, Color.Gray);
			sbatch.DrawCentered(Textures.TexVortex1, Position, _diameter * 0.666f, _diameter * 0.666f, Color.Gray);
			sbatch.DrawCentered(Textures.TexVortex0, Position, _diameter * 0.333f, _diameter * 0.333f, Color.Gray);

			float animProgress = (_animationTimer % ANIMATION_DURATION) / ANIMATION_DURATION;

			if (_power < 0)
			{
				if (animProgress < 2 / 4f)
					DrawAnimation0_In(sbatch, animProgress * 2 - 0);
				else if (animProgress < 3 / 4f)
					DrawAnimation1_In(sbatch, animProgress * 4 - 2);
				else if (animProgress < 4 / 4f)
					DrawAnimation2_In(sbatch, animProgress * 4 - 3);
			}
			else
			{
				if (animProgress < 1 / 3f)
					DrawAnimation0_Out(sbatch, animProgress * 3 - 0);
				else if (animProgress < 2 / 3f)
					DrawAnimation1_Out(sbatch, animProgress * 3 - 1);
				else if (animProgress < 3 / 3f)
					DrawAnimation2_Out(sbatch, animProgress * 3 - 2);
			}
		}

		private void DrawAnimation0_Out(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 1 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex0, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.DarkGray * (1-progress));
		}

		private void DrawAnimation1_Out(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 2 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex1, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.DarkGray * (1 - progress));
		}

		private void DrawAnimation2_Out(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 3 + FloatMath.FunctionEaseOutQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex2, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.DarkGray * (1 - progress));
		}

		private void DrawAnimation0_In(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 5 - 2*FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex2, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.LightGray * FloatMath.FunctionEaseInQuart(progress));
		}

		private void DrawAnimation1_In(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 3 - FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex1, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.LightGray * progress);
		}

		private void DrawAnimation2_In(IBatchRenderer sbatch, float progress)
		{
			var tfProgress = 2 - FloatMath.FunctionEaseInQuad(progress);

			sbatch.DrawCentered(Textures.TexVortex0, Position, _diameter * 0.333f * tfProgress, _diameter * 0.333f * tfProgress, Color.LightGray * progress);
		}
	}
}
