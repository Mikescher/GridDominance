using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace GridDominance.Shared.Framework
{
	class DeltaLimitedModuloFloat
	{
		private readonly float deltaSpeed; // 1/sec
		private readonly float modulo; // 1/sec

		public float ActualValue { get; private set; }
		public float TargetValue { get; private set; }

		public DeltaLimitedModuloFloat(float initialValue, float maxDelta, float moduloValue)
		{
			deltaSpeed = maxDelta;
			modulo = moduloValue;
		}
		
		public void Update(GameTime gameTime)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (ActualValue != TargetValue)
			{
				var radSpeed = deltaSpeed * gameTime.GetElapsedSeconds();
				var diff = FloatMath.DiffModulo(ActualValue, TargetValue, modulo);

				ActualValue = FloatMath.Abs(diff) <= radSpeed ? TargetValue : FloatMath.AddRads(ActualValue, -FloatMath.Sign(diff) * radSpeed);
			}
		}

		public void Set(float v)
		{
			TargetValue = v;
		}
	}
}
