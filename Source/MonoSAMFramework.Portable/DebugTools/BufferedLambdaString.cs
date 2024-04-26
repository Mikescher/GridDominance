using System;

namespace MonoSAMFramework.Portable.DebugTools
{
	public class BufferedLambdaString
	{
		private float _lastUpdate = float.MinValue;
		private readonly float? _lifetime;

		private readonly Func<string> _lambda;

		private string _value = "";
		public string Value
		{
			get
			{
				if (_lifetime==null || MonoSAMGame.CurrentTime.TotalElapsedSeconds - _lastUpdate > _lifetime)
				{
					_lastUpdate = MonoSAMGame.CurrentTime.TotalElapsedSeconds;
					_value = _lambda();
				}
				return _value;
			}
		}

		public BufferedLambdaString(Func<string> lambda, float? bufferDuration)
		{
			_lambda = lambda;
			_lifetime = bufferDuration;
		}

		public override string ToString() => Value;
	}
}
