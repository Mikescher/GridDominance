namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IRSAProvider
	{
		IRSAProvider SetPublicKey(string key);
		IRSAProvider SetPrivateKey(string key);

		string Encrypt(string plain);
		string Decrypt(string crypted);
	}
}
