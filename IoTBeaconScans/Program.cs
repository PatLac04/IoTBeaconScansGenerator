using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IoTBeaconScans
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("IoT Beacon Scans Generator: Simulated device\n");

            await Console.Out.WriteLineAsync("Generator starting...");
            try
            {
                TelemetryGenerator generator = new TelemetryGenerator();
                await generator.StartGeneration(5000);
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync($"Failed with exception:\t{e}");
            }
            finally
            {
                await Console.Out.WriteLineAsync("Beacon Scans Generator completed successfully.");
                await Console.Out.WriteLineAsync("Press ENTER key to exit...");
                await Console.In.ReadLineAsync();
            }
        }
    }
}
