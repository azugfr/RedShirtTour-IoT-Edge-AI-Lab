namespace FilterTemperatureModule
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Azure.Devices.Shared;
    using Newtonsoft.Json;

    class Program
    {
        static int voltageLimit { get; set; } = 20;
        static int batteryCellsQty { get; set; } = 4;

        static void Main(string[] args)
        {
            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");

            // Cert verification is not yet fully functional when using Windows OS for the container
            bool bypassCertVerification = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!bypassCertVerification) InstallCert();
            Init(connectionString, bypassCertVerification).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Add certificate in local cert store for use by client for secure connection to IoT Edge runtime
        /// </summary>
        static void InstallCert()
        {
            string certPath = Environment.GetEnvironmentVariable("EdgeModuleCACertificateFile");
            if (string.IsNullOrWhiteSpace(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing path to certificate file.");
            }
            else if (!File.Exists(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing certificate file.");
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certPath)));
            Console.WriteLine("Added Cert: " + certPath);
            store.Close();
        }


        /// <summary>
        /// Initializes the DeviceClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(string connectionString, bool bypassCertVerification = false)
        {
            Console.WriteLine("Connection String {0}", connectionString);

            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            // During dev you might want to bypass the cert verification. It is highly recommended to verify certs systematically in production
            if (bypassCertVerification)
            {
                mqttSetting.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            DeviceClient ioTHubModuleClient = DeviceClient.CreateFromConnectionString(connectionString, settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Read input params from Module Twin Desired Properties
            var moduleTwin = await ioTHubModuleClient.GetTwinAsync();
            var moduleTwinCollection = moduleTwin.Properties.Desired;
            if (moduleTwinCollection["voltageLimit"] != null)
                voltageLimit = moduleTwinCollection["voltageLimit"];

            if (moduleTwinCollection["batteryCellsQty"] != null)
                batteryCellsQty = moduleTwinCollection["batteryCellsQty"];


            // Attach callback for Twin desired properties updates
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdate, null);

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", ProcessInputDroneMessages, ioTHubModuleClient);
        }

        static Task onDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                Console.WriteLine("Desired property change:");
                Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

                if (desiredProperties["voltageLimit"] != null)
                    voltageLimit = desiredProperties["voltageLimit"];

                if (desiredProperties["batteryCellsQty"] != null)
                    batteryCellsQty = desiredProperties["batteryCellsQty"];

            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error when receiving desired property: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> ProcessInputDroneMessages(Message message, object userContext)
        {
            try {
                DeviceClient deviceClient = (DeviceClient)userContext;

                byte[] messageBytes = message.GetBytes();
                string messageString = Encoding.UTF8.GetString(messageBytes);
                messageString = messageString.Replace("[","").Replace("]","");
                Console.WriteLine(messageString);

                // Get message body
                var receivedMessage = JsonConvert.DeserializeObject<ReceivedMessage>(messageString);

                MessageBody messageBody = new MessageBody();
                Machine drone = new Machine();
                Ambient ambient = new Ambient();

                if (receivedMessage != null)
                {
                    string outputRoute = "";
                    bool isIncorrectVoltage = (bool)(receivedMessage.m_avg_battery_v < voltageLimit);

                    if (isIncorrectVoltage == true)
                    {
                        messageBody.isVoltageError = true;
                        drone.batteryVoltage = receivedMessage.m_avg_battery_v * batteryCellsQty;
                        outputRoute = "output1";
                    }
                    else
                    {
                        messageBody.isVoltageError = false;
                        drone.batteryVoltage = receivedMessage.m_avg_battery_v;
                        outputRoute = "output2";
                    }

                    drone.responseTime = receivedMessage.m_avg_resp_time;
                    ambient.temperature = receivedMessage.a_avg_temp;
                    ambient.humidity = receivedMessage.a_avg_hum;

                    messageBody.ambient = ambient;
                    messageBody.drone = drone;
                    

                    var processedMessageString = JsonConvert.SerializeObject(messageBody);
                    var processedMessage = new Message(Encoding.ASCII.GetBytes(processedMessageString));

                    await deviceClient.SendEventAsync(outputRoute, processedMessage);

                }

                // Indicate that the message treatment is completed
                return MessageResponse.Completed;
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", exception);
                }
                // Indicate that the message treatment is not completed
                DeviceClient deviceClient = (DeviceClient)userContext;
                return MessageResponse.Abandoned;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
                // Indicate that the message treatment is not completed
                DeviceClient deviceClient = (DeviceClient)userContext;
                return MessageResponse.Abandoned;
            }
        }
    }

    class ReceivedMessage 
    {
        public double m_avg_battery_v { get; set; }
        public double m_avg_resp_time { get; set; }
        public double a_avg_temp { get; set; }
        public double a_avg_hum { get; set; }
        public DateTime timeCreated { get; set; }
    }

    class MessageBody
    {
        public Machine drone { get; set; }
        public Ambient ambient { get; set; }
        public bool isVoltageError { get; set; }
    }
    class Machine
    {
        public double batteryVoltage { get; set; }
        public double responseTime { get; set; }         
    }
    class Ambient
    {
        public double temperature { get; set; }
        public double humidity { get; set; }         
    }
}
