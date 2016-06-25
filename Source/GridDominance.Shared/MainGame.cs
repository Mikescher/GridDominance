using System;
using GridDominance.Levelformat.Parser;
using GridDominance.Shared.Resources;
using GridDominance.Shared.Screens.ScreenGame;
using GridDominance.Shared.Screens.ScreenGame.Fractions;
using GridDominance.Shared.Screens.WorldMapScreen;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable;
using MonoSAMFramework.Portable.Persistance;

namespace GridDominance.Shared
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame : MonoSAMGame
	{
		public const string PROFILE_FILENAME = "USERPROFILE";

		public PlayerProfile.PlayerProfile Profile;

		public MainGame()
		{
			Profile = new PlayerProfile.PlayerProfile();

			var sdata = FileHelper.Inst.ReadDataOrNull(PROFILE_FILENAME);
			if (sdata != null)
			{
				try
				{
					Profile.DeserializeFromString(sdata);
				}
				catch (Exception)
				{
					//TODO Log Error

					Profile = new PlayerProfile.PlayerProfile();
				}
			}
			else
			{
				SaveProfile();
			}
		}

		protected override void OnInitialize()
		{
#if __DESKTOP__
			IsMouseVisible = true;
			Graphics.IsFullScreen = false;

			Graphics.PreferredBackBufferWidth = 1200;
			Graphics.PreferredBackBufferHeight = 900;
			Window.AllowUserResizing = true;

#if DEBUG
			Graphics.SynchronizeWithVerticalRetrace = false;
			IsFixedTimeStep = false;
			TargetElapsedTime = TimeSpan.FromMilliseconds(1);
#endif

			Graphics.ApplyChanges();
			Window.Position = new Point((1920 - Graphics.PreferredBackBufferWidth) / 2, (1080 - Graphics.PreferredBackBufferHeight) / 2);

#else
			Graphics.IsFullScreen = true;
			Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
			Graphics.ApplyChanges();
#endif
		}

		protected override void OnAfterInitialize()
		{
			//SetLevelScreen(Levels.LEVEL_003);
			SetWorldMapScreen();
		}

		public void SetLevelScreen(LevelFile blueprint)
		{
			SetCurrentScreen(new GDGameScreen(this, Graphics, blueprint, FractionDifficulty.KI_EASY));
		}

		public void SetWorldMapScreen()
		{
			SetCurrentScreen(new GDWorldMapScreen(this, Graphics));
		}

		protected override void LoadContent()
		{
			Textures.Initialize(Content, GraphicsDevice);
			Levels.LoadContent(Content);
		}

		protected override void UnloadContent()
		{
			// NOP
		}


		public void SaveProfile()
		{
			FileHelper.Inst.WriteData(PROFILE_FILENAME, Profile.SerializeToString());
		}
	}
}

