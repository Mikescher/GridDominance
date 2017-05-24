using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoSAMFramework.Portable.Screens.Entities.Particles
{
	public interface IParticleEmitter
	{
		bool Enabled { get; set; }
		bool Alive { get; set; }
	}
}
