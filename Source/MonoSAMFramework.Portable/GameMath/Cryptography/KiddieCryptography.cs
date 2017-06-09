namespace MonoSAMFramework.Portable.GameMath.Cryptography
{
	public static class KiddieCryptography
	{
		public static int Limit(int v, int revbase, int len)
		{
			int[] digits = new int[len];
			int p = 0;
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

			int d = 0;
			for (int i = len - 1; i >= 0; i--)
			{
				d *= revbase;
				d += digits[i];
			}
			return d;
		}

		public static int Reverse(int v, int revbase, int len)
		{
			int d = 0;
			for (int i = 0; i < len; i++)
			{
				d *= revbase;
				d += v % revbase;
				v /= revbase;
			}
			return d;
		}

		public static int ReverseXOR(int v, int revbase, int len)
		{
			return v ^ Reverse(v, revbase, len);
		}

		public static int SelfDefininedShuffle(int v, int revbase, int len)
		{
			int?[] digits = new int?[len];
			for (int i = 0; i < len; i++)
			{
				digits[i] = v % revbase;
				v /= revbase;
			}

			int d = 0;
			int pos = 0;
			for (int i = 0; ;)
			{
				var dpv = digits[pos].Value;
				d *= revbase;
				d += dpv;
				digits[pos] = null;
				dpv++;

				i++;
				if (i >= len) break;

				while (dpv > 0)
				{
					pos = (pos + 1) % digits.Length;
					if (digits[pos] != null) dpv--;
				}
			}

			return d;
		}

		public static int PrefixShift(int v, int revbase, int len)
		{
			int[] digits = new int[len];
			for (int i = 0; i < len; i++)
			{
				digits[len - i - 1] = v % revbase;
				v /= revbase;
			}

			int d = 0;
			for (int i = 0; i < len; i++)
			{
				var shift = digits[(len + (i - 1)) % len];
				var value = (digits[i] + shift) % revbase;

				d *= revbase;
				d += value;
			}
			return d;
		}

		public static int ZeroFix(int v, int revbase, int len)
		{
			int[] digits = new int[len];
			for (int i = 0; i < len; i++)
			{
				digits[len - i - 1] = v % revbase;
				v /= revbase;
			}

			for (int i = 1; i < len; i++)
			{
				if (digits[i] != 0 && digits[i - 1] != 0)
				{
					int d = 0;
					for (int j = 0; j < len; j++)
					{
						d *= revbase;
						d += digits[(i + j) % len];
					}
					return d;
				}
			}
			{
				digits[0] = 1;
				int d = 0;
				for (int j = 0; j < len; j++)
				{
					d *= revbase;
					d += digits[j];
				}
				return d;
			}
		}
	}
}
