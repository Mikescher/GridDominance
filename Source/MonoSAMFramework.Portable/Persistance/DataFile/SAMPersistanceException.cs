using System;

namespace MonoSAMFramework.Portable.Persistance.DataFile
{
	public class SAMPersistanceException : Exception
	{
		public SAMPersistanceException(Exception inner) : base("Deserialization Exception", inner) { }
		public SAMPersistanceException(string msg) : base("Deserialization Exception: " + msg) { }
	}
}
