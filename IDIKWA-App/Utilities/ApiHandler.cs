using Avalonia.Threading;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class ApiHandler
    {
        private MainWindowViewModel viewModel;

        public ApiHandler(MainWindowViewModel viewmodel)
        {
            viewModel = viewmodel;
        }

        public async Task StartAsync()
        => await Task.Run(() =>
        {
            var pipe = new NamedPipeServerStream("Idikwa-Api", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message);
            pipe.WaitForConnection();
            _ = StartAsync();
            var reader = new BinaryReader(pipe);
            var writer = new BinaryWriter(pipe);

            var request = reader.ReadString();
            if (!viewModel.Settings.EulaAccepted)
            {
                writer.Write("EULA not accepted");
                return;
            }
            var args = JsonConvert.DeserializeObject<string[]>(request);

            static T? ValueAt<T>(T[] array, int index) where T : class
            {
                if (array.Length <= index)
                    return null;
                else
                    return array[index];
            }
            const string helpResponse =
@"Use :
idikwa-api <command> [options]

commands :
----------------------------------------------------------------------------------------------
<none>, --help, help, -h, h     displays the help panel
----------------------------------------------------------------------------------------------
--get-devices                   displays the list of devices, JSON formatted

Usage :
--get-devices

Example :
idikwa-api --get-devices
----------------------------------------------------------------------------------------------
--add-devices                   activates the given devices, and display the successful
                                devices activation

Usage :
--add-devices [devices]

Devices :
A list of all the devices to activate
Can be the id :         {0.0.0.00000000}.{5010a0b1-d565-4648-baa1-0090c83909cb}
Can be the name :       Microphone ((Realtek High Definition Audio(SST)))
Example :
idikwa-api --add-devices {0.0.0.00000000}.{5010a0b1-d565-4648-baa1-0090c83909cb}
----------------------------------------------------------------------------------------------
--remove-devices                deactivates the given devices, and display the successful
                                devices activation

Usage :
--remove-devices [devices]

Devices :
A list of all the devices to deactivate
Can be the id :         {0.0.0.00000000}.{5010a0b1-d565-4648-baa1-0090c83909cb}
Can be the name :       Microphone ((Realtek High Definition Audio(SST)))
Example :
idikwa-api --remove-devices {0.0.0.00000000}.{5010a0b1-d565-4648-baa1-0090c83909cb}
----------------------------------------------------------------------------------------------
--start-record                  starts recording if the software was not already

Usage :
--start-record

Example :
idikwa-api --start-record
----------------------------------------------------------------------------------------------
--cancel-record                 stops recording if the software was not already, and does
                                not open the samples edition window

Usage :
--cancel-record

Example :
idikwa-api --cancel-record
----------------------------------------------------------------------------------------------
--capture-record                captures the current record and opens the samples
                                edition window

Usage :
--capture-record

Example :
idikwa-api --capture-record
----------------------------------------------------------------------------------------------
--recording                     returns true if the software is currently recording,
                                false otherwise

Usage :
--recording

Example :
idikwa-api --recording
----------------------------------------------------------------------------------------------
--set-output                    changes the output directory

Usage :
--set-output <new directory>

Example :
idikwa-api --set-output D:\Desktop
----------------------------------------------------------------------------------------------
--get-output                    returns the output directory

Usage :
--get-output

Example :
idikwa-api --get-output
----------------------------------------------------------------------------------------------
--set-bitrate                   changes the bitrate of mp3 encoded files

Usage :
--set-bitrate <new bitrate>

Example :
idikwa-api --set-bitrate 128000
----------------------------------------------------------------------------------------------
--get-bitrate                   returns the bitrate of mp3 encoded files

Usage :
--get-bitrate

Example :
idikwa-api --get-bitrate
----------------------------------------------------------------------------------------------
--set-mono                      changes the mono value. ""true"" for mono, ""false""
                                for stereo

Usage :
--set-mono <new value>

Example :
idikwa-api --set-mono true
----------------------------------------------------------------------------------------------
--get-mono                      returns the mono value. ""true"" for mono, ""false""
                                for stereo

Usage :
--get-mono

Example :
idikwa-api --get-mono
----------------------------------------------------------------------------------------------
--set-sample-rate               changes the sample rate of the recorded audio

Usage :
--set-sample-rate <new sample rate>

Example :
idikwa-api --set-sample-rate 44100
----------------------------------------------------------------------------------------------
--get-sample-rate               returns the sample rate of the recorded audio

Usage :
--get-sample-rate

Example :
idikwa-api --get-sample-rate
----------------------------------------------------------------------------------------------
--set-duration               changes the duration of the recorded audio

Usage :
--set-duration <new duration>

Example :
idikwa-api --set-duration 0:02:30
----------------------------------------------------------------------------------------------
--get-duration               returns the duration of the recorded audio

Usage :
--get-duration

Example :
idikwa-api --get-duration
----------------------------------------------------------------------------------------------
";
            if (args is not null)
            {
                switch (ValueAt(args, 0))
                {
                    case "--get-devices":
                        {
                            var devices = viewModel.Settings.AllDevices
                                .Select(device => new
                                {
                                    name = device.Name,
                                    id = device.ID,
                                    mode = device.DataFlow switch
                                    {
                                        DataFlow.Render => "render",
                                        DataFlow.Capture => "capture",
                                        _ => "unknown",
                                    },
                                    active = device.Recording
                                });
                            writer.Write(JsonConvert.SerializeObject(devices, Formatting.Indented));
                        }
                        break;

                    case "--add-devices":
                        {
                            for (int index = 1; args.Length > index; ++index)
                            {
                                var deviceInfo = args[index];
                                var device = viewModel.Settings.AllDevices.FirstOrDefault(device =>
                                {
                                    return device.ID == deviceInfo ||
                                        device.Name.ToLowerInvariant() == deviceInfo.ToLowerInvariant();
                                });
                                if (device is not null)
                                    device.Recording = true;
                            }
                            SettingsManager.Save(viewModel.Settings.Model);
                            writer.Write("");
                        }
                        break;

                    case "--remove-devices":
                        {
                            for (int index = 1; args.Length > index; ++index)
                            {
                                var deviceInfo = args[index];
                                var device = viewModel.Settings.AllDevices.FirstOrDefault(device =>
                                {
                                    return device.ID == deviceInfo ||
                                        device.Name.ToLowerInvariant() == deviceInfo.ToLowerInvariant();
                                });
                                if (device is not null)
                                    device.Recording = false;
                            }
                            SettingsManager.Save(viewModel.Settings.Model);
                            writer.Write("");
                        }
                        break;

                    case "--start-record":
                        {
                            if (!viewModel.Recording)
                                _ = Dispatcher.UIThread.InvokeAsync(viewModel.RunRecord);
                            writer.Write("");
                        }
                        break;

                    case "--stop-record":
                        {
                            if (viewModel.Recording)
                                _ = Dispatcher.UIThread.InvokeAsync(viewModel.RunCancelRecording);
                            writer.Write("");
                        }
                        break;

                    case "--capture-record":
                        {
                            if (viewModel.Recording)
                                _ = Dispatcher.UIThread.InvokeAsync(viewModel.StopRecord);
                            writer.Write("");
                        }
                        break;

                    case "--recording":
                        writer.Write(viewModel.Recording ? "true" : "false");
                        break;

                    case "--set-output":
                        {
                            if (args.Length > 1)
                                viewModel.Settings.OutputPath = args[1];
                            writer.Write("");
                        }
                        break;

                    case "--get-output":
                        {
                            writer.Write(viewModel.Settings.OutputPath);
                        }
                        break;

                    case "--set-bitrate":
                        {
                            int value;
                            if (args.Length > 1 && int.TryParse(args[1], out value) && value > 0)
                                viewModel.Settings.BitRate = value;
                            writer.Write("");
                        }
                        break;

                    case "--get-bitrate":
                        {
                            writer.Write(viewModel.Settings.BitRate.ToString());
                        }
                        break;

                    case "--set-mono":
                        {
                            bool value;
                            if (args.Length > 1 && bool.TryParse(args[1], out value))
                                viewModel.Settings.Mono = value;
                            writer.Write("");
                        }
                        break;

                    case "--get-mono":
                        {
                            writer.Write(viewModel.Settings.Mono ? "true" : "false");
                        }
                        break;

                    case "--set-sample-rate":
                        {
                            int value;
                            if (args.Length > 1 && int.TryParse(args[1], out value) && value > 0)
                                viewModel.Settings.SampleRate = value;
                            writer.Write("");
                        }
                        break;

                    case "--get-sample-rate":
                        {
                            writer.Write(viewModel.Settings.SampleRate.ToString());
                        }
                        break;

                    case "--set-duration":
                        {
                            TimeSpan value;
                            if (args.Length > 1 && TimeSpan.TryParse(args[1], out value) && value > TimeSpan.FromSeconds(1))
                            {
                                if (value > TimeSpan.FromMinutes(5))
                                    value = TimeSpan.FromMinutes(5);
                                viewModel.Settings.Duration = value;
                            }
                            writer.Write("");
                        }
                        break;

                    case "--get-duration":
                        {
                            writer.Write(viewModel.Settings.Duration.ToString());
                        }
                        break;

                    case "":
                    case "--help":
                    case "help":
                    case "-h":
                    case "h":
                        writer.Write(helpResponse);
                        break;

                    default:
                        writer.Write($"Unknown argument {args[0]}");
                        break;
                }
            }

            pipe.Disconnect();
            pipe.Dispose();
        });
    }
}