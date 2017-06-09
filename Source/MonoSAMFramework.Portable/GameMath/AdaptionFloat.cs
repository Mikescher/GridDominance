
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

		public AdaptionFloat(float initial, float forceFactor, float drag)
		{
			Value = TargetValue = initial;
			
			_forcePerUnit = forceFactor;
			_dragFactor = drag;
		}
		
		public void Update(SAMTime gameTime, InputState istate)
		{
			var force = (TargetValue - Value) * _forcePerUnit;

			Speed += force * gameTime.ElapsedSeconds;
			Speed *= _dragFactor;

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
			else
			{
				Value = nv;
			}
		}

		public void Set(float v)
		{
			TargetValue = v;
		}

		public void SetDirect(float v)
		{
			Value = v;
			TargetValue = v;
		}
	}
}
