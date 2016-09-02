using System;
using System.Linq;

namespace MonoSAMFramework.Portable.GameMath
{
	public class ConstantRandom
	{
		private readonly int backupInext;
		private readonly int backupInextp;
		private readonly int[] backupSeedArray;

		private int inext;
		private int inextp;
		private int[] seedArray;

		public ConstantRandom(object o)
		{
			seedArray = new int[0x38];
			int num2 = 0x9a4ec86 - System.Math.Abs(Environment.TickCount ^ o.GetHashCode());
			seedArray[0x37] = num2;
			int num3 = 0x1;
			for (int i = 0x1; i < 0x37; i++)
			{
				int index = 0x15 * i % 0x37;
				seedArray[index] = num3;
				num3 = num2 - num3;
				if (num3 < 0x0)
				{
					num3 += 0x7fffffff;
				}
				num2 = seedArray[index];
			}
			for (int j = 0x1; j < 0x5; j++)
			{
				for (int k = 0x1; k < 0x38; k++)
				{
					seedArray[k] -= seedArray[0x1 + (k + 0x1e) % 0x37];
					if (seedArray[k] < 0x0)
					{
						seedArray[k] += 0x7fffffff;
					}
				}
			}
			inext = 0x0;
			inextp = 0x15;

			//--------------------------------------------------------------------------

			backupInext = inext;
			backupInextp = inextp;

			backupSeedArray = seedArray.ToArray();
		}

		public void Reseed()
		{
			inext = backupInext;
			inextp = backupInextp;

			for (var i = 0; i < backupSeedArray.Length; i++)
				seedArray[i] = backupSeedArray[i];
		}

		public virtual int Next()
		{
			return (int)(Sample() * 2147483647.0);
		}

		public virtual int Next(int maxValue)
		{
			if (maxValue < 0x0) throw new ArgumentOutOfRangeException(nameof(maxValue));

			return (int)(Sample() * maxValue);
		}

		public virtual int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));

			int num = maxValue - minValue;
			if (num < 0x0)
			{
				long num2 = maxValue - minValue;
				return (int)(long)(Sample() * num2) + minValue;
			}
			return (int)(Sample() * num) + minValue;
		}

		public virtual double NextDouble()
		{
			return Sample();
		}

		protected virtual double Sample()
		{
			int inextLocal = inext;
			int inextpLocal = inextp;

			if (++inextLocal >= 0x38)
				inextLocal = 0x1;
			if (++inextpLocal >= 0x38)
				inextpLocal = 0x1;

			int num = seedArray[inextLocal] - seedArray[inextpLocal];
			if (num < 0x0)
				num += 0x7fffffff;

			seedArray[inextLocal] = num;

			inext = inextLocal;
			inextp = inextpLocal;

			return (num * 4.6566128752457969E-10);
		}
	}
}
