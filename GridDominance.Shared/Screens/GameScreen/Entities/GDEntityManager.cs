using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GridDominance.Shared.Screens.GameScreen.Entities
{
    class GDEntityManager
	{
		private List<GDEntity> entities = new List<GDEntity>();

	    public GDEntityManager()
	    {
		    
	    }

	    public void Update(GameTime gameTime)
		{
			foreach (var gdEntity in entities)
			{
				gdEntity.Update(gameTime);
			}
		}

	    public void Draw(SpriteBatch sbatch)
	    {
		    foreach (var gdEntity in entities)
		    {
			    gdEntity.Draw(sbatch);
		    }
	    }

	    public void AddEntity(GDEntity e)
	    {
			entities.Add(e);
	    }
	}
}
