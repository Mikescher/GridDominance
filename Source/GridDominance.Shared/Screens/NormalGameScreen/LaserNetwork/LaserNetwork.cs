using MonoSAMFramework.Portable.Interfaces;
using System.Collections.Generic;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;
using MonoSAMFramework.Portable.Screens.Entities;
using MonoSAMFramework.Portable.Extensions;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using System;

namespace GridDominance.Shared.Screens.NormalGameScreen.LaserNetwork
{
	public sealed class LaserNetwork : ISAMUpdateable
	{
		public const int MAX_LASER_RAYCOUNT   = 16;
		public const int MAX_LASER_PER_SOURCE = 128; // 8 * 16

		private readonly GDGameScreen _screen;
		private readonly World _world;
		private readonly float _worldWidth;
		private readonly float _worldHeight;

		public List<LaserSource> Sources = new List<LaserSource>();

		public LaserNetwork(World b2dworld, GDGameScreen scrn)
		{
			_screen = scrn;
			_world = b2dworld;

			_worldWidth  = scrn.Blueprint.LevelWidth;
			_worldHeight = scrn.Blueprint.LevelHeight;
		}

		public void Update(SAMTime gameTime, InputState istate)
		{
			for (int i = 0; i < Sources.Count; i++)
			{
				if (!Sources[i].IsDirty) continue;
				Sources[i].LaserCount = 0;
			}

			for (int i = 0; i < Sources.Count; i++)
			{
				if (!Sources[i].IsDirty) continue;

				RecalcSource(Sources[i]);

				Sources[i].IsDirty = false;
			}
		}

		public LaserSource AddSource(GameEntity e)
		{
			var src = new LaserSource(e.Position, _screen.GetNeutralFraction());
			Sources.Add(src);
			return src;
		}
		
		private void RecalcSource(LaserSource src)
		{
			if (!src.LaserActive) { src.LaserCount = 0; return; }

			Vector2 start = src.Position;
			Vector2 end   = src.Position + new Vector2(_worldWidth+_worldHeight,0).Rotate(src.LaserRotation);
			
			for (int i = 0; i < MAX_LASER_RAYCOUNT; i++)
			{
				var result = RayCast(_world, start, end);

				//TODO
			}
		}

		private static Tuple<Fixture, Vector2, Vector2> RayCast(World w, Vector2 start, Vector2 end)
		{
			Tuple<Fixture, Vector2, Vector2> result = null;

			//     return -1:       ignore this fixture and continue 
			//     return  0:       terminate the ray cast
			//     return fraction: clip the ray to this point
			//     return 1:        don't clip the ray and continue
			Func<Fixture, Vector2, Vector2, float, float> callback = (f, pos, normal, frac) =>
			{
				result = Tuple.Create(f, pos, normal);

				return frac; // limit
			};

			w.RayCast(callback, start, end);

			return result;
		}
	}
}
