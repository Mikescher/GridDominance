using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.GameMath
{
	public class DeltaLimitedModuloFloat
	{
		private readonly float deltaSpeed; // 1/sec
		private readonly float modulo; // 1/sec

		public float ActualValue { get; private set; }
		public float TargetValue { get; private set; }

		public DeltaLimitedModuloFloat(float initialValue, float maxDelta, float moduloValue)
		{
			deltaSpeed = maxDelta;
			modulo = moduloValue;

			ActualValue = initialValue;
			TargetValue = initialValue;
		}

		public void Update(SAMTime gameTime)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (ActualValue != TargetValue)
			{
				var radSpeed = deltaSpeed * gameTime.ElapsedSeconds;
				var diff = FloatMath.DiffModulo(ActualValue, TargetValue, modulo);

				ActualValue = FloatMath.Abs(diff) <= radSpeed ? TargetValue : FloatMath.AddRads(ActualValue, -FloatMath.Sign(diff) * radSpeed);
			}
		}

		public bool CUpdate(SAMTime gameTime)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (ActualValue != TargetValue)
			{
				var radSpeed = deltaSpeed * gameTime.ElapsedSeconds;
				var diff = FloatMath.DiffModulo(ActualValue, TargetValue, modulo);

				ActualValue = FloatMath.Abs(diff) <= radSpeed ? TargetValue : FloatMath.AddRads(ActualValue, -FloatMath.Sign(diff) * radSpeed);

				return true;
			}

			return false;
		}

		public void Set(float v)
		{
			TargetValue = v;
		}

		public void ForceSet(float v)
		{
			TargetValue = v;
			ActualValue = v;
		}

		public void SetFull(float va, float vt)
		{
			TargetValue = vt;
			ActualValue = va;
		}
	}
}
