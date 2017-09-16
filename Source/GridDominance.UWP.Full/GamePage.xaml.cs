using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using GridDominance.Shared;

namespace GridDominance.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private readonly WinPhoneBridge _impl;
        private readonly MainGame _game;

        public GamePage()
        {
            this.InitializeComponent();

            // Create the game.
            var launchArguments = string.Empty;
            _impl = new WinPhoneBridge();
            _game = XamlGame<MainGame>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
            _game.Construct(_impl);
        }
    }
}
