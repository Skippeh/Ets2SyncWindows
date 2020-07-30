using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Ets2SyncWindows.Data;
using Newtonsoft.Json;
using PrismLibrary;

namespace Ets2SyncWindows
{
    public class PersistentAppState
    {
        public Dictionary<int, MapDlc> SelectedMapDlcs { get; set; }
        public Dictionary<int, CargoDlc> SelectedCargoDlcs { get; set; }
        public Dictionary<int, TrailerDlc> SelectedTrailerDlcs { get; set; }
        public int SelectedGameIndex { get; set; }
        public bool BackupSavesBeforeSyncing { get; set; }
        public bool AutomaticallySyncOnSave { get; set; }
        public bool SelectGameCardExpanded { get; set; }
        public bool SelectDlcCardExpanded { get; set; }
        public bool SelectSaveCardExpanded { get; set; }

        private PersistentAppState()
        {
        }

        public bool Save(string filePath)
        {
            try
            {
                using var fileStream = File.CreateText(filePath);
                fileStream.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Failed to save config due to the following error:\n\n{ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return false;
            }
        }

        public static PersistentAppState CreateDefault()
        {
            return new PersistentAppState()
            {
                SelectedGameIndex = GameData.Games.OrderBy(g => g.UiSortOrder).First().Index,
                SelectedMapDlcs = new Dictionary<int, MapDlc>(),
                SelectedCargoDlcs = new Dictionary<int, CargoDlc>(),
                SelectedTrailerDlcs = new Dictionary<int, TrailerDlc>(),
                BackupSavesBeforeSyncing = true,
                SelectGameCardExpanded = true,
                SelectDlcCardExpanded = true,
                SelectSaveCardExpanded = true
            };
        }

        public static PersistentAppState LoadOrCreateDefault(string filePath)
        {
            if (!File.Exists(filePath))
                return CreateDefault();

            var fileContents = File.ReadAllText(filePath);

            try
            {
                return JsonConvert.DeserializeObject<PersistentAppState>(fileContents);
            }
            catch (JsonException)
            {
                MessageBox.Show("Failed to load config due to invalid formatting. The config will be reset.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                File.Delete(filePath);
                return CreateDefault();
            }
        }

        public void ReadFrom(AppState appState)
        {
            SelectedGameIndex = appState.SelectedGame.Index;
            BackupSavesBeforeSyncing = appState.BackupSavesBeforeSyncing;
            AutomaticallySyncOnSave = appState.AutomaticallySyncSaves;
            SelectGameCardExpanded = appState.SelectGameCardExpanded;
            SelectDlcCardExpanded = appState.SelectDlcCardExpanded;
            SelectSaveCardExpanded = appState.SelectSaveCardExpanded;

            ReadDlcs(appState, SelectedMapDlcs, dlcs => dlcs.MapDlcs);
            ReadDlcs(appState, SelectedCargoDlcs, dlcs => dlcs.CargoDlcs);
            ReadDlcs(appState, SelectedTrailerDlcs, dlcs => dlcs.TrailerDlcs);
        }

        private void ReadDlcs<T>(AppState appState, Dictionary<int, T> selectedDlcs, Func<GameDlcs, IEnumerable<Dlc>> dlcSelector) where T : Enum
        {
            foreach (var kv in appState.SelectedDlcs)
            {
                ReadDlc(dlcSelector(appState.SelectedDlcs[kv.Key]), kv.Key, selectedDlcs);
            }
        }

        private void ReadDlc<T>(IEnumerable<Dlc> dlcs, Game game, Dictionary<int, T> selectedDlcs) where T : Enum
        {
            int bitmask = 0;

            foreach (Dlc dlc in dlcs)
            {
                if (dlc.Selected)
                    bitmask |= dlc.Bitmask;
            }

            selectedDlcs[game.Index] = (T) Enum.ToObject(typeof(T), bitmask);
        }

        public void ApplyTo(AppState appState)
        {
            appState.BackupSavesBeforeSyncing = BackupSavesBeforeSyncing;
            appState.SelectedGame = GameData.Games.FirstOrDefault(g => g.Index == SelectedGameIndex);
            appState.AutomaticallySyncSaves = AutomaticallySyncOnSave;
            appState.SelectGameCardExpanded = SelectGameCardExpanded;
            appState.SelectDlcCardExpanded = SelectDlcCardExpanded;
            appState.SelectSaveCardExpanded = SelectSaveCardExpanded;

            ApplyDlcs(appState, SelectedMapDlcs, dlcs => dlcs.MapDlcs);
            ApplyDlcs(appState, SelectedCargoDlcs, dlcs => dlcs.CargoDlcs);
            ApplyDlcs(appState, SelectedTrailerDlcs, dlcs => dlcs.TrailerDlcs);
        }

        private void ApplyDlcs<T>(AppState appState, Dictionary<int, T> selectedDlcs, Func<GameDlcs, IEnumerable<Dlc>> dlcSelector) where T : Enum
        {
            foreach (var kv in selectedDlcs)
            {
                Game game = GameData.Games.First(g => g.Index == kv.Key);
                var gameDlcs = appState.SelectedDlcs[game];
                ApplyDlc(dlcSelector(gameDlcs), Convert.ToInt32(kv.Value));
            }
        }

        private void ApplyDlc(IEnumerable<Dlc> dlcs, int selectedDlcs)
        {
            if (selectedDlcs == 0)
                return;
            
            foreach (Dlc dlc in dlcs)
            {
                if ((selectedDlcs & dlc.Bitmask) != 0)
                {
                    dlc.Selected = true;
                }
            }
        }
    }
}