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
--get-devices                   displays the list of devices

Usage :
--get-devices [options]

Options :
<none>                          displays every device by default
render                          displays the rendering devices
capture                         displays the capturing devices
active                          displays the currently selected devices
inactive                        displays the currently inactive devices

Example :
idikwa-api --get-devices active capture
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
";
            if (args is not null)
            {
                switch (ValueAt(args, 0))
                {
                    case "--display-devices":
                        {
                            const int RenderDevices = 1 << 0;
                            const int CaptureDevices = 1 << 1;
                            const int ActiveDevices = 1 << 2;
                            const int InactiveDevices = 1 << 3;
                            int selection = 0;

                            for (int index = 1; args.Length > index; ++index)
                                switch (ValueAt(args, index))
                                {
                                    case "capture":
                                        selection |= CaptureDevices;
                                        break;

                                    case "render":
                                        selection |= RenderDevices;
                                        break;

                                    case "active":
                                        selection |= ActiveDevices;
                                        break;

                                    case "inactive":
                                        selection |= InactiveDevices;
                                        break;
                                }

                            if ((selection & (RenderDevices | CaptureDevices)) == 0)
                                selection |= RenderDevices | CaptureDevices;
                            if ((selection & (ActiveDevices | InactiveDevices)) == 0)
                                selection |= ActiveDevices | InactiveDevices;

                            var devices = viewModel.Settings.AllDevices
                                .Where(device =>
                                {
                                    if (((selection & RenderDevices) == 0) && device.DataFlow == DataFlow.Render)
                                        return false;
                                    if (((selection & CaptureDevices) == 0) && device.DataFlow == DataFlow.Capture)
                                        return false;
                                    if (((selection & ActiveDevices) == 0) && device.Recording)
                                        return false;
                                    if (((selection & InactiveDevices) == 0) && !device.Recording)
                                        return false;
                                    return true;
                                }).Select(device => new
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