﻿using System;
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
        private AppBarToggleButton LastMillingDiameterButton;
        private bool IsInitiated = false;

        public GamePage()
        {

            this.InitializeComponent();

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<MillingMachine>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
            this.DataContext = _game;
            LastMillingDiameterButton = Mill16Button;
            IsInitiated = true;
        }

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileOpener = new FileOpenPicker();
            fileOpener.FileTypeFilter.Add(".k16");
            fileOpener.FileTypeFilter.Add(".k08");
            fileOpener.FileTypeFilter.Add(".k01");
            fileOpener.FileTypeFilter.Add(".f12");
            fileOpener.FileTypeFilter.Add(".f10");

            StorageFile file = await fileOpener.PickSingleFileAsync();
            if (file != null)
            {
                _game.IsWorking = false;
                _game.FileHelper.FileLoad(file);
            }
            if (_game.FileHelper.Frez == FileHelper.FrezType.K)
                MillingTypeButton.IsChecked = false;
            else if (_game.FileHelper.Frez == FileHelper.FrezType.F)
                MillingTypeButton.IsChecked = true;

            LastMillingDiameterButton.IsChecked = false;
            if (_game.FileHelper.Diameter == 16)
            {
                Mill16Button.IsChecked = true;
                LastMillingDiameterButton = Mill16Button;
            }
            if (_game.FileHelper.Diameter == 8)
            {
                Mill8Button.IsChecked = true;
                LastMillingDiameterButton = Mill8Button;
            }
            if (_game.FileHelper.Diameter == 1)
            {
                Mill1Button.IsChecked = true;
                LastMillingDiameterButton = Mill1Button;
            }
            if (_game.FileHelper.Diameter == 12)
            {
                Mill12Button.IsChecked = true;
                LastMillingDiameterButton = Mill12Button;
            }
            if (_game.FileHelper.Diameter == 10)
            {
                Mill10Button.IsChecked = true;
                LastMillingDiameterButton = Mill10Button;
            }
        }

        private void StartMillingButton_Click(object sender, RoutedEventArgs e)
        {
            _game.StartMilling();
        }

        private void ToggleWireframeButton_Click(object sender, RoutedEventArgs e)
        {
            _game.Brick.IsWireframeOn = !_game.Brick.IsWireframeOn;
        }

        private void QuickFinishButton_Click(object sender, RoutedEventArgs e)
        {
            _game.DoFastSimulation();
        }

        private void MillingTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MillingTypeButton.IsChecked == true)
                _game.FileHelper.Frez = FileHelper.FrezType.F;
            else if (MillingTypeButton.IsChecked == false)
                _game.FileHelper.Frez = FileHelper.FrezType.K;
        }

        private void MillingDiameterButton_Click(object sender, RoutedEventArgs e)
        {
            var b = (AppBarToggleButton)sender;
            LastMillingDiameterButton.IsChecked = false;
            b.IsChecked = true;
            switch (b.Name)
            {
                case "Mill16Button":
                    _game.FileHelper.Diameter = 16;
                    LastMillingDiameterButton = Mill16Button;
                    break;
                case "Mill8Button":
                    _game.FileHelper.Diameter = 8;
                    LastMillingDiameterButton = Mill8Button;
                    break;
                case "Mill1Button":
                    _game.FileHelper.Diameter = 1;
                    LastMillingDiameterButton = Mill1Button;
                    break;
                case "Mill12Button":
                    _game.FileHelper.Diameter = 12;
                    LastMillingDiameterButton = Mill12Button;
                    break;
                case "Mill10Button":
                    _game.FileHelper.Diameter = 10;
                    LastMillingDiameterButton = Mill10Button;
                    break;
                default:
                    break;
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var s = (Slider)sender;
            if (IsInitiated)//TODO: handle objects better when not yet initiated
                _game.Speed = (float)s.Value;
        }

        private void ShowHidePath_Click(object sender, RoutedEventArgs e)
        {
            _game.showPath = !(_game.showPath);
        }

    }
}
