
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.GameMath
{
	public class AdaptionFloat : ISAMUpdateable
	{
		public float Value = 0;

		public float TargetValue = 0;
		public float Speed = 0;

		public float ValueMin = float.MinValue;
		public float ValueMax = float.MaxValue;

		private readonly float _forcePerUnit;
		private readonly float _dragFactor;
		private readonly float _minSpeed;

		public AdaptionFloat(float initial, float forceFactor, float drag, float minSpeed)
		{
			Value = TargetValue = initial;
			
			_forcePerUnit = forceFactor;
			_dragFactor = drag;
			_minSpeed = minSpeed;
		}
		
		public void Update(SAMTime gameTime, InputState istate)//TODO FPS dependent (OverworldScrollAgent acts strange with ~400UPS)
		{
			var force = (TargetValue - Value) * _forcePerUnit;

			Speed += force * gameTime.ElapsedSeconds;
			Speed *= _dragFactor;

			if (FloatMath.Abs(Speed) < _minSpeed && Speed > 0) Speed = _minSpeed * FloatMath.Sign(Speed);
			
			var nv = Value + Speed * gameTime.ElapsedSeconds;

			if (nv < ValueMin)
			{
				var nnv = FloatMath.Max(Value, ValueMin);
				Value = nnv;
				if (Speed < 0) Speed = 0;
			}
			else if (nv > ValueMax)
			{
				var nnv = FloatMath.Min(Value, ValueMax);
				Value = nnv;
				if (Speed > 0) Speed = 0;
			}
			else if (FloatMath.Sign(nv-TargetValue) != FloatMath.Sign(Value - TargetValue) && FloatMath.Abs(FloatMath.Abs(Speed) - _minSpeed) < FloatMath.EPSILON)
			{
				Value = TargetValue;
				Speed = 0;
			} 
			else
			{
				Value = nv;
			}
		}

		public void Set(float v)
		{
			TargetValue = v;
		}

		public void Set(float v, bool direct)
		{
			if (direct) Value = v;
			TargetValue = v;
		}

		public void SetDirect(float v)
		{
			Value = v;
			TargetValue = v;
		}

		public void SetFull(AdaptionFloat v)
		{
			Value = v.Value;
			TargetValue = v.TargetValue;
			Speed = v.Speed;
			ValueMin = v.ValueMin;
			ValueMax = v.ValueMax;
		}
	}
}
