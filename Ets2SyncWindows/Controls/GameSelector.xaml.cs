using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ets2SyncWindows.Data;

namespace Ets2SyncWindows.Controls
{
    public partial class GameSelector : UserControl
    {
        public static readonly DependencyProperty SelectedGameProperty = DependencyProperty.Register(
            nameof(SelectedGame),
            typeof(Game),
            typeof(GameSelector),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnGameChanged)
        );

        private static void OnGameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var gameSelector = dependencyObject as GameSelector;
            gameSelector?.OnItemSelected(gameSelector, (Game) args.NewValue);
        }

        public delegate void GameSelectionChangedDelegate(object sender, Game newGame);
        public event GameSelectionChangedDelegate GameSelectionChanged;

        public Game SelectedGame
        {
            get => (Game) GetValue(SelectedGameProperty);
            set => SetValue(SelectedGameProperty, value);
        }
        
        public GameSelector()
        {
            InitializeComponent();
            Resources.Add("Games", GameData.Games);
        }

        private void OnItemSelected(object sender, Game newGame)
        {
            GameSelectionChanged?.Invoke(this, newGame);
        }
    }
}