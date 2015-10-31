using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MillingMachineSimulator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
		readonly MillingMachine _game;

		public GamePage()
        {
            this.InitializeComponent();

			// Create the game.
			var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<MillingMachine>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileOpener = new FileOpenPicker();
            fileOpener.FileTypeFilter.Add(".k16");
            fileOpener.FileTypeFilter.Add(".f16");
            fileOpener.FileTypeFilter.Add(".k14");
            fileOpener.FileTypeFilter.Add(".f14");

            StorageFile file = await fileOpener.PickSingleFileAsync();
            if (file != null)
                _game.FileHelper.FileLoad(file);
        }

        private void StartMillingButton_Click(object sender, RoutedEventArgs e)
        {
            _game.StartMilling();
        }
    }
}
