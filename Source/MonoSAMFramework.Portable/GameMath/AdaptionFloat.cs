using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.GameMath
{
	public class AdaptionFloat : ISAMUpdateable
	{
		public const int MAX_UPDATE_PER_CALL = 4;

		public float CalculationUPS = 30f;

		public float LastValue;
		public float NextValue;
		public float Value = 0;

		public float TargetValue = 0;
		public float Speed = 0;

		public float ValueMin = float.MinValue;
		public float ValueMax = float.MaxValue;

		private readonly float _forcePerUnit;
		private readonly float _dragFactor;
		private readonly float _minSpeed;

		private float timeSinceUpdate = 0f;

		public AdaptionFloat(float initial, float forceFactor, float drag, float minSpeed)
		{
			Value = TargetValue = LastValue = NextValue = initial;
			
			_forcePerUnit = forceFactor;
			_dragFactor = drag;
			_minSpeed = minSpeed;
		}
		
		public void Update(SAMTime gameTime, InputState istate)
		{
			timeSinceUpdate += gameTime.ElapsedSeconds;

			for (int i = 0; timeSinceUpdate > (1 / CalculationUPS); i++)
			{
				if (i >= MAX_UPDATE_PER_CALL) { timeSinceUpdate = 0; break; }

				RealUpdate();
				timeSinceUpdate -= (1 / CalculationUPS);
			}

			Value = LastValue + (NextValue - LastValue) * timeSinceUpdate * CalculationUPS;
		}

		private void RealUpdate()
		{
			LastValue = NextValue;

			var delta = 1 / CalculationUPS;

			var force = (TargetValue - NextValue) * _forcePerUnit;

			Speed += force * delta;
			Speed *= _dragFactor;

			if (FloatMath.Abs(Speed) < _minSpeed && Speed > 0) Speed = _minSpeed * FloatMath.Sign(Speed);

			var nv = NextValue + Speed * delta;

			if (nv < ValueMin)
			{
				var nnv = FloatMath.Max(NextValue, ValueMin);
				NextValue = nnv;
				if (Speed < 0) Speed = 0;
			}
			else if (nv > ValueMax)
			{
				var nnv = FloatMath.Min(NextValue, ValueMax);
				NextValue = nnv;
				if (Speed > 0) Speed = 0;
			}
			else if (FloatMath.Sign(nv - TargetValue) != FloatMath.Sign(NextValue - TargetValue) && FloatMath.Abs(FloatMath.Abs(Speed) - _minSpeed) < FloatMath.EPSILON)
			{
				NextValue = TargetValue;
				Speed = 0;
			}
			else
			{
				NextValue = nv;
			}
		}

		public void Set(float v)
		{
			TargetValue = v;
		}

		public void Set(float v, bool direct)
		{
			if (direct) Value = NextValue = LastValue = v;
			TargetValue = v;
		}

		public void SetDirect(float v)
		{
			Value = TargetValue = NextValue = LastValue = v;
		}

		public void SetFull(AdaptionFloat v)
		{
			Value = v.Value;
			TargetValue = v.TargetValue;
			LastValue = v.LastValue;
			NextValue = v.NextValue;
			Speed = v.Speed;
			ValueMin = v.ValueMin;
			ValueMax = v.ValueMax;
			CalculationUPS = v.CalculationUPS;
			timeSinceUpdate = v.timeSinceUpdate;
		}
	}
}
