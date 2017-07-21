using System;
using System.Runtime.InteropServices;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public static class NetworkDataTools
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct UIntFloat
		{
			[FieldOffset(0)]
			public float FloatValue;

			[FieldOffset(0)]
			public uint IntValue;
		}

		public static ushort GetUInt16(byte b1, byte b2)
		{
			return (ushort) ((b1 << 8) | b2);
		}

		public static byte GetLowBits(byte b, int c)
		{
			return (byte) (b & ((1 << c) - 1));
		}

		public static byte GetHighBits(byte b, int c)
		{
			return (byte)((b>>(8-c)) & ((1 << c) - 1));
		}

		public static ushort GetSplitBits(byte b1, byte b2, int c1, int c2)
		{
			return (ushort)((GetLowBits(b1, c1) << c2) | (GetHighBits(b2, c2)));
		}

		public static byte GetByte(byte b)
		{
			return b;
		}

		public static bool GetBit(byte b, int bitIndex) // lowest bit is bit_0
		{
			return (b & (1 << bitIndex)) != 0;
		}

		public static float ConvertToRadians(int data, int bitsize)
		{
			return (data / ((1 << bitsize) * 1f)) * FloatMath.TAU;
		}

		public static byte ConvertFromRadians(float data, int bitsize)
		{
			return (byte) FloatMath.IClamp((int) (((FloatMath.NormalizeAngle(data) / FloatMath.TAU) * (1 << bitsize))), 0, 1 << bitsize);
		}

		public static void SetByte(out byte target, byte v)
		{
			target = v;
		}

		public static void SetBit(out byte target, int bitIndex, bool v)
		{
			target = (byte)( (v?1:0)<<bitIndex ); // lowest bit is bit_0
		}

		public static void SetByteClamped(out byte target, int v)
		{
			target = (byte)FloatMath.IClamp(v, 0, 255);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target">targwt</param>
		/// <param name="b1">value1</param>
		/// <param name="b2">value2</param>
		/// <param name="bs1">total bitsize value 1</param>
		/// <param name="bs2">total bitsize value 2</param>
		/// <param name="c1">take x (lowest)  bits from value1</param>
		/// <param name="c2">take x (highest) bits from value2</param>
		public static void SetSplitByte(out byte target, int b1, int b2, int bs1, int bs2, int c1, int c2)
		{
			// [   bs1    ][  bs2 ]
			// 11111111111122222222
			//        [  target ]
			//        [c1 ][ c2 ]

			target = (byte)
			(
				((b1 & ((1 << c1)-1)) << (8-c1))
				|
				(((b2 >> (bs2 - c2))) & ((1 << c2) - 1))
			);
		}

		public static void SetByteFloor(out byte target, float v)
		{
			target = (byte)v;
		}

		public static void SetByteFloorRange(out byte target, float min, float max, float v)
		{
			SetByteFloor(out target, FloatMath.Clamp((v - min) / (max - min), 0f, 1f) * 255);
		}

		public static float GetByteFloorRange(byte b, float min, float max)
		{
			return min + (b / 255f) * (max - min);
		}

		public static void SetByteWithHighBits(out byte b, int value, int bitsize)
		{
			b = (byte)(value >> (bitsize - 8));
		}

		public static void SetByteWithLowBits(out byte b, int value, int bitsize)
		{
			b = (byte)(value & 0xFF);
		}

		public static void SetUInt16(out byte b1, out byte b2, ushort v)
		{
			b1 = (byte) ((v >> 8) & 0xFF);
			b2 = (byte) ((v     ) & 0xFF);
		}

		public static unsafe byte[] GetBytes(float value)
		{
			uint val = *((uint*)&value);
			return GetBytes(val);
		}

		public static unsafe float ToSingle(byte[] value, int index)
		{
			uint i = ToUInt32(value, index);
			return *(((float*)&i));
		}

		public static uint ToUInt32(byte[] value, int index)
		{
			return (uint)(
				value[0 + index] << 0 |
				value[1 + index] << 8 |
				value[2 + index] << 16 |
				value[3 + index] << 24);
		}

		public static byte[] GetBytes(uint value)
		{
			return new byte[4] {
				(byte)(value & 0xFF),
				(byte)((value >> 8) & 0xFF),
				(byte)((value >> 16) & 0xFF),
				(byte)((value >> 24) & 0xFF) };
		}

		public static void SetSingle(out byte b1, out byte b2, out byte b3, out byte b4, float data)
		{
			var cv = new UIntFloat{FloatValue = data};

			b1 = (byte)((cv.IntValue >>  0) & 0xFF);
			b2 = (byte)((cv.IntValue >>  8) & 0xFF);
			b3 = (byte)((cv.IntValue >> 16) & 0xFF);
			b4 = (byte)((cv.IntValue >> 24) & 0xFF);
		}

		public static float GetSingle(byte b1, byte b2, byte b3, byte b4)
		{
			var cv = new UIntFloat { IntValue = (uint)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1) };
			return cv.FloatValue;
		}
	}
}
