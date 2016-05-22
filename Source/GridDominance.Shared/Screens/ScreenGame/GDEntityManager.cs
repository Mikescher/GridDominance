using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens.Entities;
using GridDominance.Shared.Resources;
using MonoSAMFramework.Portable.DebugTools;

namespace GridDominance.Shared.Screens.ScreenGame
{
	class GDEntityManager : EntityManager
	{
		public readonly World PhysicsWorld;
		public GDGameScreen GDOwner => (GDGameScreen) Owner;

#if DEBUG
		private DebugViewXNA debugView;
#endif

		public GDEntityManager(GDGameScreen scrn) : base(scrn)
		{
			PhysicsWorld = new World(Vector2.Zero);

#if DEBUG
			debugView = new DebugViewXNA(PhysicsWorld);
			debugView.LoadContent(GDOwner.Graphics.GraphicsDevice, GDOwner.Owner.Content, Textures.DebugFont);
			debugView.AppendFlags(DebugViewFlags.Shape);
			debugView.AppendFlags(DebugViewFlags.DebugPanel);
			debugView.AppendFlags(DebugViewFlags.PerformanceGraph);
			debugView.AppendFlags(DebugViewFlags.ContactPoints);
			debugView.AppendFlags(DebugViewFlags.ContactNormals);
			debugView.AppendFlags(DebugViewFlags.Controllers);
			debugView.TextColor = Color.Black;
			debugView.Enabled = false;
#endif
		}

		protected override RectangleF RecalculateBoundingBox()
		{
			var ox = GDOwner.GDViewport.OffsetX;
			var oy = GDOwner.GDViewport.OffsetY;

			var tolerance = GDGameScreen.TILE_WIDTH;

			return new RectangleF(-(ox + tolerance), -(oy + tolerance), GDGameScreen.VIEW_WIDTH + 2* (oy + tolerance), GDGameScreen.VIEW_HEIGHT + 2 * (ox + tolerance));
		}

		protected override void OnBeforeUpdate(GameTime gameTime, InputState state)
		{
#if DEBUG
			debugView.DebugPanelPosition = new Vector2(55, Owner.Viewport.ViewportHeight - 180);
			debugView.PerformancePanelBounds = new Rectangle(450, Owner.Viewport.ViewportHeight - 180, 200, 100);

			debugView.Enabled = DebugSettings.Get("PhysicsDebugView");
#endif

			PhysicsWorld.Step(gameTime.GetElapsedSeconds());
		}

		protected override void OnAfterUpdate(GameTime gameTime, InputState state)
		{
			// NOP
		}

		public override void DrawOuterDebug()
		{
#if DEBUG
			var pMatrix = GDOwner.GDViewport.GetFarseerDebugProjectionMatrix();
			var vMatrix = Matrix.CreateScale(GDSettings.PHYSICS_CONVERSION_FACTOR);
			debugView.RenderDebugData(ref pMatrix, ref vMatrix);
#endif
		}
	}
}
