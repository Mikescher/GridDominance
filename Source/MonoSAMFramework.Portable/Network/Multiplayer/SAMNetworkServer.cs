using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Interfaces;
using MonoSAMFramework.Portable.Screens;

namespace MonoSAMFramework.Portable.Network.Multiplayer
{
	public abstract class SAMNetworkServer : ISAMUpdateable
	{
		private readonly INetworkMedium _medium;
		
		protected SAMNetworkServer(INetworkMedium medium)
		{
			_medium = medium;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			
		}
	}
}
