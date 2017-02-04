using ArpanTECH;
using MonoSAMFramework.Portable.DeviceBridge;
using MonoSAMFramework.Portable.Language;
using System;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace GridDominance.Windows
{
	class WindowsImpl : IOperatingSystemBridge
	{
		public FileHelper FileHelper { get; } = new WindowsFileHelper();

		public string FullDeviceInfoString { get; } = "";
		public string DeviceName { get; } = "PC";
		public string DeviceVersion { get; } = Environment.OSVersion.VersionString;

		private readonly SHA256 sha256 = SHA256.Create();

		public string DoSHA256(string input)
		{
			return ByteUtils.ByteToHexBitFiddle(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
		}

		public string DoRSAEncrypt(string input, string pubkey, int size)
		{
			RSAx rsax = new RSAx(pubkey, size);
			byte[] ctx = rsax.Encrypt(Encoding.UTF8.GetBytes(input), false);
			return Convert.ToBase64String(ctx);
		}
	}
}