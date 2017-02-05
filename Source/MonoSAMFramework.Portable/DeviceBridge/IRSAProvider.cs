namespace MonoSAMFramework.Portable.DeviceBridge
{
	public interface IRSAProvider
	{
		IRSAProvider SetPublicKey(string key, int keysize);
		IRSAProvider SetPrivateKey(string key, int keysize);

		string Encrypt(string plain);
		string Decrypt(string crypted);
	}
}
