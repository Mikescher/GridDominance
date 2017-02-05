using MonoSAMFramework.Portable.DeviceBridge;
using RSAxPlus.ArpanTECH;
using System;
using System.Text;

namespace GridDominance.Android
{
	class RSAxWrapper : IRSAProvider
	{
		private RSAx rsa;

		public IRSAProvider SetPublicKey(string key)
		{
			rsa = RSAx.CreateFromPEM(key);
			return this;
		}

		public IRSAProvider SetPrivateKey(string key)
		{
			rsa = RSAx.CreateFromPEM(key);
			return this;
		}

		public string Encrypt(string plain)
		{
			byte[] ct = rsa.Encrypt(Encoding.UTF8.GetBytes(plain), false, false);
			return Convert.ToBase64String(ct);
		}

		public string Decrypt(string crypted)
		{
			byte[] pt = rsa.Decrypt(Convert.FromBase64String(crypted.Replace("\r", "").Replace("\n", "")), false, false);
			return Encoding.UTF8.GetString(pt);
		}
	}
}