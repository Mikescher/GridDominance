using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MonoGame.Framework;
using GridDominance.Shared;
using GridDominance.WinPhone;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace GridDominance.WinPhone8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        private readonly WinPhoneBridge _impl;
        private readonly MainGame _game;

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            _impl = new WinPhoneBridge();
            _game = XamlGame<MainGame>.Create(launchArguments, Window.Current.CoreWindow, this);
            _game.Construct(_impl);
        }
    }
}
