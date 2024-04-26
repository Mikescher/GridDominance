using System;

namespace MonoSAMFramework.Portable.RenderHelper
{
	class StaticTexturesInitializationException : Exception
	{
		public StaticTexturesInitializationException() : base("The textures in StaticTextures are not completetly initialized") { }
	}
}
