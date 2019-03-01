using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTBeaconScans
{
    public class TelemetryGenerator
    {
        private int pendingTaskCount;
        private long messageSent;

        private static DeviceClient s_deviceClient;

        // Connection string for your Device
        // Each Device has a different connection string
        private readonly static string s_connectionString = "";

        private static int s_telemetryInterval = 20; // Seconds

        // private static List<Guid> potentialBeaconIds = Enumerable.Range(0, 20).Select(d => Guid.NewGuid()).ToList<Guid>();
        private static List<string> potentialClientIds = new List<string>(new string[] { "ARIjNEVWZ3iJmqu8zd7v8A==", "ZZIjBESWZ3iJmqu8zd5d4C==", "OPQvMDEUY3iJmqu8zd6h0K==" });
        // private static List<string> potentialClientIds = new List<string>(new string[] { "ARIjNEVWZ3iJmqu8zd7v8A==" });
        private Bogus.Faker<BeaconReading> beaconReadingGenerator = new Bogus.Faker<BeaconReading>().Rules(
            (faker, recording) =>
            {
                recording.Version = 1.0d;
                //recording.BeaconId = faker.PickRandom(potentialBeaconIds);
                recording.BeaconId = faker.Random.Number(1, 200);
                recording.GatewayId = faker.Random.Number(1, 10);
                recording.ClientId = faker.PickRandom(potentialClientIds);
                recording.CompoundKey = $"{recording.ClientId}_{recording.BeaconId}";
                recording.State = new BeaconState
                {
                    RSSI = faker.Random.Int(-100, 100),
                    BatteryLevel = faker.Random.Number(0, 100),
                    Humidity = faker.Random.Double(0.1d, 0.25d),
                    Temperature = faker.Random.Number(20, 30)
                };
                recording.EventDatetime = DateTime.UtcNow;
                DateTime submitTime = new DateTime(2019, 02, 26, 15, faker.Random.Number(20, 25), faker.Random.Number(0, 59), DateTimeKind.Utc);
                recording.SubmitDatetime = ((DateTimeOffset)submitTime).ToUnixTimeSeconds();
                recording.SubmitDay = submitTime.ToString("yyyy-MM-dd");
                recording.SubmitHour = submitTime.ToString("yyyy-MM-dd-HH");
                recording.SubmitMinute = submitTime.ToString("yyyy-MM-dd-HH-mm");
                recording.SubmitSecond = submitTime.ToString("yyyy-MM-dd-HH-mm-ss");
            }
        );

        public async Task StartGeneration(int numberOfMessagesToSend)
        {
            int taskCount = 10;
            int minThreadPoolSize = 100;
            ThreadPool.SetMinThreads(minThreadPoolSize, minThreadPoolSize);

            await Console.Out.WriteLineAsync($"Starting with {taskCount} tasks");

            pendingTaskCount = taskCount;
            List<Task> tasks = new List<Task>();
            tasks.Add(
                this.LogOutputStatsAsync()
            );

            for (int i = 0; i < taskCount; i++)
            {
                tasks.Add(SendTelemetryMessages(numberOfMessagesToSend / taskCount));
            }

            await Task.WhenAll(tasks);
        }


        public async Task SendTelemetryMessages(int numberOfMessagesToSend)
        {
            s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString);

            for (int i = 0; i < numberOfMessagesToSend; i++)
            {
                BeaconReading telemetry = beaconReadingGenerator.Generate();
                var telemetryDataString = JsonConvert.SerializeObject(telemetry);
                var message = new Message(Encoding.ASCII.GetBytes(telemetryDataString));

                // Send the tlemetry message
                await s_deviceClient.SendEventAsync(message);
                Interlocked.Increment(ref this.messageSent);

                //Console.WriteLine($"{DateTime.Now} > Sent message");
                //await Task.Delay(500);
            }

            Interlocked.Decrement(ref this.pendingTaskCount);
        }

        public async Task LogOutputStatsAsync()
        {
            long lastCount = 0;
            double lastSeconds = 0;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (this.pendingTaskCount > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                double seconds = watch.Elapsed.TotalSeconds;

                long currentCount = this.messageSent;

                await Console.Out.WriteLineAsync($"Sent {currentCount} telemetry messages @ {Math.Round(this.messageSent / seconds)} msg/s");

                lastCount = messageSent;
                lastSeconds = seconds;
            }

            double totalSeconds = watch.Elapsed.TotalSeconds;

            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("Summary:");
            await Console.Out.WriteLineAsync("--------------------------------------------------------------------- ");
            await Console.Out.WriteLineAsync($"Total Time Elapsed:\t{watch.Elapsed}");
            await Console.Out.WriteLineAsync($"Sent {lastCount} telemetry messages @ {Math.Round(this.messageSent / watch.Elapsed.TotalSeconds)} msg/s");
            await Console.Out.WriteLineAsync("--------------------------------------------------------------------- ");
            await Console.Out.WriteLineAsync();
        }
    }
}
