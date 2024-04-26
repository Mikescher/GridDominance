using System;

namespace MonoSAMFramework.Portable.Extensions
{
	public static class VersionExtension
	{
		public static ulong ToNum(this Version v)
		{
			ulong num = 0;
			num |= (uint)v.Major;
			num <<= 12;
			num |= (uint)v.Minor;
			num <<= 12;
			num |= (uint)v.Build;
			num <<= 12;
			num |= (uint)v.Revision;

			return num;
		}
	}
}
