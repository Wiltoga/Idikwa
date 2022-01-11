﻿using Avalonia.Controls;
using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NAudio.Wave;
using System.Collections.Specialized;
using DynamicData;
using System.IO;
using System.Diagnostics;

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Factory = new SampleFactory();
            Window = null;
            Settings = App.InitialSettings is not null
                ? new SettingsViewModel(App.InitialSettings)
                : SettingsViewModel.Default;
            Record = CommandHandler.Create(async () =>
            {
                if (Recording)
                    await StopRecord();
                else
                    RunRecord();
            });
            EditSettings = CommandHandler.Create(async () => await RunEditSettingsAsync());
            Recording = false;
        }

        [Reactive]
        public ICommand EditSettings { get; private set; }

        [Reactive]
        public ICommand Record { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        public SettingsViewModel Settings { get; }

        [Reactive]
        public Window? Window { get; set; }

        public bool WithLogin => false;

        private SampleFactory Factory { get; }

        public async Task Exit()
        {
            await Factory.StopRecord();
        }

        public async Task RunEditSettingsAsync()
        {
            var dialog = new SettingsWindow()
            {
                DataContext = Settings
            };

            await dialog.ShowDialog(Window);

            SettingsManager.Save(Settings.Model);
        }

        public void RunRecord()
        {
            if (Settings.RecordingDevices.Any() && Settings.Duration > TimeSpan.Zero)
                try
                {
                    Factory.StartRecord(Settings.RecordingDevices.Select(device => device.Device), WaveFormat.CreateIeeeFloatWaveFormat(Settings.SampleRate, 1), Settings.Duration);
                    Recording = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        public async Task StopRecord()
        {
            try
            {
                var streams = await Factory.StopRecord();
                Recording = false;
                //var filename = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.mp3";
                //Directory.CreateDirectory(Settings.OutputPath);
                //var filepath = Path.Combine(Settings.OutputPath, filename);
                //using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                //{
                //    Factory.Save(streams, stream, Settings.BitRate);
                //}
                //new Process
                //{
                //    StartInfo = new ProcessStartInfo("explorer.exe", $"/select,\"{filepath}\"")
                //    {
                //        UseShellExecute = true
                //    }
                //}.Start();
                var computationStream = streams.First().Item2;
                if (computationStream.Length / computationStream.WaveFormat.AverageBytesPerSecond < 1)
                    return;
                var dialog = new SamplesEditionWindow()
                {
                    DataContext = new SamplesEditionViewModel(
                        streams
                            .Select(stream => new RecordViewModel(Settings.AllDevices.First(device => device.Device.ID == stream.Item1.ID), stream.Item2)))
                };
                await dialog.ShowDialog(Window);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}