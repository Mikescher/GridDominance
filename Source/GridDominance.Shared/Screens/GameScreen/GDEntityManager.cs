using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using GridDominance.Shared.Framework;
using GridDominance.Shared.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace GridDominance.Shared.Screens.GameScreen
{
	class GDEntityManager
	{
		private List<GDEntity> entities = new List<GDEntity>();

		public readonly World PhysicsWorld;
		public readonly GameScreen Owner;

#if DEBUG
		private DebugViewXNA debugView;
#endif

		public GDEntityManager(GameScreen screen)
		{
			Owner = screen;
			PhysicsWorld = new World(Vector2.Zero);

#if DEBUG
			debugView = new DebugViewXNA(PhysicsWorld);
			debugView.LoadContent(screen.Graphics.GraphicsDevice, screen.Owner.Content, Textures.DebugFont);
			debugView.AppendFlags(DebugViewFlags.Shape);
			debugView.AppendFlags(DebugViewFlags.DebugPanel);
			debugView.AppendFlags(DebugViewFlags.PerformanceGraph);
			debugView.AppendFlags(DebugViewFlags.ContactPoints);
			debugView.AppendFlags(DebugViewFlags.ContactNormals);
			debugView.AppendFlags(DebugViewFlags.Controllers);
			debugView.TextColor = Color.Black;
#endif
		}

		public void Update(GameTime gameTime, InputState state)
		{
#if DEBUG
			debugView.DebugPanelPosition = new Vector2(55, Owner.Viewport.ViewportHeight - 180);
			debugView.PerformancePanelBounds = new Rectangle(450, Owner.Viewport.ViewportHeight - 180, 200, 100);

			if (state.IsKeyJustDown(Keys.F1))
			{
				debugView.Enabled = !debugView.Enabled;
			}
#endif

			PhysicsWorld.Step(gameTime.GetElapsedSeconds());

			foreach (var gdEntity in entities.ToList())
			{
				gdEntity.Update(gameTime, state);
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
			e.Manager = this;
			entities.Add(e);
			e.OnInitialize();
		}

		public int Count()
		{
			return entities.Count;
		}

		public void DrawRest()
		{
#if DEBUG
			var pMatrix = Owner.Viewport.GetFarseerDebugProjectionMatrix();
			var vMatrix = Matrix.CreateScale(GDSettings.PHYSICS_CONVERSION_FACTOR);
			debugView.RenderDebugData(ref pMatrix, ref vMatrix);
#endif
		}
	}
}
