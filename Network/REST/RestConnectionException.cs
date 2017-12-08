using System;

namespace MonoSAMFramework.Portable.Network.REST
{
	public class RestConnectionException : Exception
	{
		public RestConnectionException(Exception e) : base(e.Message, e) { }
	}
}
