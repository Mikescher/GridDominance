using System;

namespace MonoSAMFramework.Portable.FileHelper.DataFile
{
	public class DeserializationException : Exception
	{
		public DeserializationException(Exception inner) : base("Deserialization Exception", inner) { }
		public DeserializationException(string msg) : base("Deserialization Exception: " + msg) { }
	}
}
