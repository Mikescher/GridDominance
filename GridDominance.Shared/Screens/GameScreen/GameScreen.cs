using GridDominance.Shared.Framework;
using GridDominance.Shared.Screens.GameScreen.Background;
using GridDominance.Shared.Screens.GameScreen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.InputListeners;

namespace GridDominance.Shared.Screens.GameScreen
{
    class GameScreen : GDScreen
    {
        public const int TILE_WIDTH = 128;
        
        public const int VIEW_WIDTH  = 8 * TILE_WIDTH; // 1024
        public const int VIEW_HEIGHT = 5 * TILE_WIDTH; // 640

        //-----------------------------------------------------------------

        private TolerantBoxingViewportAdapter viewport;
	    private GDEntityManager entities;

		private InputStateManager inputStateMan;
		private InputListenerManager inputs;
	    private MouseListener mouseListener;
	    private TouchListener touchListener;

		private SpriteBatch entityBatch;

		private GameGridBackground background;

        public GameScreen(MainGame game, GraphicsDeviceManager gdm) : base(game, gdm)
        {
            Initialize();
        }

        private void Initialize()
        {
			entityBatch = new SpriteBatch(Graphics.GraphicsDevice);
            viewport = new TolerantBoxingViewportAdapter(Owner.Window, Graphics, VIEW_WIDTH, VIEW_HEIGHT);
			inputs = new InputListenerManager(viewport);
			inputStateMan = new InputStateManager(viewport);
			background = new GameGridBackground(Graphics.GraphicsDevice, viewport);
			entities = new GDEntityManager();

			mouseListener = inputs.AddListener(new MouseListenerSettings());
			touchListener = inputs.AddListener(new TouchListenerSettings());

			//--------------------

			entities.AddEntity(new Cannon(2, 3));
            entities.AddEntity(new Cannon(2, 0));
            entities.AddEntity(new Cannon(5, 1));
            entities.AddEntity(new Cannon(6, 2));
        }

        public override void Update(GameTime gameTime)
        {
	        var state = inputStateMan.GetNewState();
			
			if (state.IsExit()) Owner.Exit();

			inputs.Update(gameTime);

			entities.Update(gameTime, state);
        }

        public override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.AliceBlue);

            entityBatch.Begin(transformMatrix: viewport.GetScaleMatrix());
            {
                background.Draw(entityBatch);

                entities.Draw(entityBatch);
            }
            entityBatch.End();
        }
    }
}
