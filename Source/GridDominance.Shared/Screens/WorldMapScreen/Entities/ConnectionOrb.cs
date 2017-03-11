using System;
using System.Collections.Generic;
using System.Text;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.BatchRenderer;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;

namespace GridDominance.Shared.Screens.WorldMapScreen.Entities
{
	class ConnectionOrb : GameEntity
	{
		public  const float DIAMETER = LevelNode.DIAMETER * 0.15f;
		private const float INITIAL_SPEED = 150f;
		private const float GRAVITY_TARGET = 150f;
		private const float MAX_LIFETIME = 15f;
		private const float INITIAL_BOOST_TIME = 1f;
		private const float GROW_SPEED = 1f;

		public override FSize DrawingBoundingBox { get; } = new FSize(DIAMETER, DIAMETER);
		public override Color DebugIdentColor { get; } = Color.MediumOrchid;

		private readonly Vector2 _target;
		private readonly FractionDifficulty _diff;
		private readonly Vector2 _boostVelocity;

		private Vector2 _pos;
		public override Vector2 Position => _pos;

		private float _spawnPercentage = 0f;
		private float _remainingBoost = 1f;

		public ConnectionOrb(GameScreen scrn, Vector2 start, Vector2 velocity, Vector2 target, FractionDifficulty diff) : base(scrn)
		{
			_pos = start;
			_target = target;
			_boostVelocity = velocity.Normalized() * INITIAL_SPEED;
			_diff = diff;
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
			if (_spawnPercentage < 1)
			{
				_spawnPercentage = FloatMath.LimitedInc(_spawnPercentage, gameTime.ElapsedSeconds * GROW_SPEED, 1f);
			}
			else
			{
				if (_remainingBoost > 0) _remainingBoost = FloatMath.LimitedDec(_remainingBoost, gameTime.ElapsedSeconds / INITIAL_BOOST_TIME, 0f);

				var dist = (_target - Position);
				var gravity = dist.Normalized() * GRAVITY_TARGET;

				_pos += (_remainingBoost * _boostVelocity + (1 - _remainingBoost) * gravity) * gameTime.ElapsedSeconds;

				if (dist.LengthSquared() < LevelNode.DIAMETER * LevelNode.DIAMETER / 4f) Remove();
			}

			if (Lifetime > MAX_LIFETIME) Remove();
		}

		protected override void OnDraw(IBatchRenderer sbatch)
		{
			sbatch.DrawCentered(Textures.TexParticle[14], Position, DIAMETER * _spawnPercentage, DIAMETER * _spawnPercentage, FractionDifficultyHelper.GetColor(_diff));
		}
	}
}
