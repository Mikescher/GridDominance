using System;

namespace MonoSAMFramework.Portable.GameMath
{
	public static class ByteMath
	{
		public static ushort ToUInt16(byte[] value)
		{
			return (ushort)(
				value[0] << 0 |
				value[1] << 8);
		}
		
		public static ushort ToUInt16(byte value0, byte value1)
		{
			return (ushort)(
				value0 << 0 |
				value1 << 8);
		}

		public static byte[] GetBytesFromUInt16(ushort value)
		{
			return new[] 
			{
				(byte)((value >>  0) & 0xFF),
				(byte)((value >>  8) & 0xFF),
			};
		}
		
		public static uint ToUInt32(byte[] value)
		{
			return
				(uint)value[0] << 0  |
				(uint)value[1] << 8  |
				(uint)value[2] << 16 |
				(uint)value[3] << 24;
		}
		
		public static uint ToUInt32(byte value0, byte value1, byte value2, byte value3)
		{
			return 
				(uint)value0 << 0  |
				(uint)value1 << 8  |
				(uint)value2 << 16 |
				(uint)value3 << 24;
		}

		public static byte[] GetBytesFromUInt32(uint value)
		{
			return new[] 
			{
				(byte)((value >>  0) & 0xFF),
				(byte)((value >>  8) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 24) & 0xFF),
			};
		}
		
		public static ulong ToUInt48(byte[] value)
		{
			return 
				(ulong)value[0] << 0  |
				(ulong)value[1] << 8  |
				(ulong)value[2] << 16 |
				(ulong)value[3] << 24 |
				(ulong)value[4] << 32 |
				(ulong)value[5] << 40;
		}
		
		public static ulong ToUInt48(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5)
		{
			return 
				(ulong)value0 << 0  |
				(ulong)value1 << 8  |
				(ulong)value2 << 16 |
				(ulong)value3 << 24 |
				(ulong)value4 << 32 |
				(ulong)value5 << 40;
		}

		public static byte[] GetBytesFromUInt48(ulong value)
		{
			return new[] 
			{
				(byte)((value >>  0) & 0xFF),
				(byte)((value >>  8) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 24) & 0xFF),
				(byte)((value >> 32) & 0xFF),
				(byte)((value >> 40) & 0xFF),
			};
		}
		
		public static ulong ToUInt64(byte[] value)
		{
			return 
				(ulong)value[0] << 0  |
				(ulong)value[1] << 8  |
				(ulong)value[2] << 16 |
				(ulong)value[3] << 24 |
				(ulong)value[4] << 32 |
				(ulong)value[5] << 40 |
				(ulong)value[6] << 48 |
				(ulong)value[7] << 56;
		}
		
		public static ulong ToUInt64(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5, byte value6, byte value7)
		{
			return 
				(ulong)value0 << 0  |
				(ulong)value1 << 8  |
				(ulong)value2 << 16 |
				(ulong)value3 << 24 |
				(ulong)value4 << 32 |
				(ulong)value5 << 40 |
				(ulong)value6 << 48 |
				(ulong)value7 << 56;
		}

		public static byte[] GetBytesFromUInt64(ulong value)
		{
			return new[] 
			{
				(byte)((value >>  0) & 0xFF),
				(byte)((value >>  8) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 24) & 0xFF),
				(byte)((value >> 32) & 0xFF),
				(byte)((value >> 40) & 0xFF),
				(byte)((value >> 48) & 0xFF),
				(byte)((value >> 56) & 0xFF),
			};
		}

		public static Tuple<uint, ushort, ushort, ushort, ulong> SplitGuid(Guid g)
		{
			// strange ToByteArray behaviour, see https://stackoverflow.com/q/9195551/1761622

			var ba = g.ToByteArray();

			var a = ToUInt32(ba[0], ba[1], ba[2], ba[3]);
			var b = ToUInt16(ba[4], ba[5]);
			var c = ToUInt16(ba[6], ba[7]);
			var d = ToUInt16(ba[9], ba[8]);
			var e = ToUInt48(ba[15], ba[14], ba[13], ba[12], ba[11], ba[10]);

			return Tuple.Create(a, b, c, d, e);
		}

		public static Guid JoinGuid(uint a, ushort b, ushort c, ushort d, ulong e)
		{
			var aa = GetBytesFromUInt32(a);
			var bb = GetBytesFromUInt16(b);
			var cc = GetBytesFromUInt16(c);
			var dd = GetBytesFromUInt16(d);
			var ee = GetBytesFromUInt48(e);

			var ba = new byte[16];

			ba[0x0] = aa[0];
			ba[0x1] = aa[1];
			ba[0x2] = aa[2];
			ba[0x3] = aa[3];

			ba[0x4] = bb[0];
			ba[0x5] = bb[1];

			ba[0x6] = cc[0];
			ba[0x7] = cc[1];

			ba[0x8] = dd[1];
			ba[0x9] = dd[0];

			ba[0xA] = ee[5];
			ba[0xB] = ee[4];
			ba[0xC] = ee[3];
			ba[0xD] = ee[2];
			ba[0xE] = ee[1];
			ba[0xF] = ee[0];

			return new Guid(ba);
		}
	}
}
