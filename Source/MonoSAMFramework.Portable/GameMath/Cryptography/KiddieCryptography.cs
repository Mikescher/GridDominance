namespace MonoSAMFramework.Portable.GameMath.Cryptography
{
	public static class KiddieCryptography
	{
		public static uint Limit(uint v, uint revbase, uint len)
		{
			uint[] digits = new uint[len];
			uint p = 0;
			while (v != 0)
			{
				if (p < len)
				{
					digits[p] = v % revbase;
					v /= revbase;
					p++;
				}
				else
				{
					var dv = v % revbase;
					digits[dv % len] = (digits[dv % len] + dv) % revbase;
					v /= revbase;
				}
			}

			uint d = 0;
			for (int i = (int)len - 1; i >= 0; i--)
			{
				d *= revbase;
				d += digits[i];
			}
			return d;
		}

		public static uint Reverse(uint v, uint revbase, uint len)
		{
			uint d = 0;
			for (uint i = 0; i < len; i++)
			{
				d *= revbase;
				d += v % revbase;
				v /= revbase;
			}
			return d;
		}

		public static uint ReverseXOR(uint v, uint revbase, uint len)
		{
			return v ^ Reverse(v, revbase, len);
		}

		public static uint SelfDefininedShuffle(uint v, uint revbase, uint len)
		{
			uint?[] digits = new uint?[len];
			for (uint i = 0; i < len; i++)
			{
				digits[i] = v % revbase;
				v /= revbase;
			}

			uint d = 0;
			uint pos = 0;
			for (uint i = 0; ;)
			{
				var dpv = digits[pos] ?? 0;
				d *= revbase;
				d += dpv;
				digits[pos] = null;
				dpv++;

				i++;
				if (i >= len) break;

				while (dpv > 0)
				{
					pos = (pos + 1) % len;
					if (digits[pos] != null) dpv--;
				}
			}

			return d;
		}

		public static uint PrefixShift(uint v, uint revbase, uint len)
		{
			uint[] digits = new uint[len];
			for (uint i = 0; i < len; i++)
			{
				digits[len - i - 1] = v % revbase;
				v /= revbase;
			}

			uint d = 0;
			for (uint i = 0; i < len; i++)
			{
				var shift = digits[(len + (i - 1)) % len];
				var value = (digits[i] + shift) % revbase;

				d *= revbase;
				d += value;
			}
			return d;
		}

		public static uint ZeroFix(uint v, uint revbase, uint len)
		{
			uint[] digits = new uint[len];
			for (uint i = 0; i < len; i++)
			{
				digits[len - i - 1] = v % revbase;
				v /= revbase;
			}

			for (uint i = 1; i < len; i++)
			{
				if (digits[i] != 0 && digits[i - 1] != 0)
				{
					uint d = 0;
					for (uint j = 0; j < len; j++)
					{
						d *= revbase;
						d += digits[(i + j) % len];
					}
					return d;
				}
			}
			{
				digits[0] = 1;
				uint d = 0;
				for (uint j = 0; j < len; j++)
				{
					d *= revbase;
					d += digits[j];
				}
				return d;
			}
		}
	}
}
