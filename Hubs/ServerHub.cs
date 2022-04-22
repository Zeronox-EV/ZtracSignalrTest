//#define TRACE


using Microsoft.AspNetCore.SignalR;






using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;










namespace ZtracSignalrTest.Hubs
{







    public class ServerHub : Hub
    {

        /*
        public async Task SendMessage(string user, string message)
       => await Clients.All.SendAsync("ReceiveMessage", user, message);
        */





        public async Task Main(string[] args)
        {
            const bool isUsingMessagepack = false;
            const int numberOfPackets = 2000;
            const int packetSizeInBytes = 256 * 1024;

            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            //Trace.AutoFlush = true;

            
            /*
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Entering Main");
            Console.WriteLine("Hello World.");
            Trace.WriteLine("Exiting Main");
            Trace.Unindent();
            */


            Trace.WriteLine($"isUsingMessagepack={isUsingMessagepack} | numberOfPackets={numberOfPackets} | packetSize={packetSizeInBytes} Bytes");

            

            Debug.WriteLine("hello");
            Console.WriteLine("hello");

            var hubConnectionBuilder = new HubConnectionBuilder()

              
                
            
                .WithUrl("http://localhost:5000/ServerHub", (opts) =>
                 {
                     opts.HttpMessageHandlerFactory = (message) =>
                     {
                         if (message is HttpClientHandler clientHandler)
                             // always verify the SSL certificate
                             clientHandler.ServerCertificateCustomValidationCallback +=
                                 (sender, certificate, chain, sslPolicyErrors) => { return true; };
                         return message;
                     };
                 })

            ;

                

            if (isUsingMessagepack)
                hubConnectionBuilder.AddMessagePackProtocol();
            else
                hubConnectionBuilder.AddJsonProtocol();

            var hubConnection = hubConnectionBuilder.Build();


            await hubConnection.StartAsync();
            Trace.WriteLine("Connected to hub");

            var cancellationTokenSource = new CancellationTokenSource();
            var stream = hubConnection.StreamAsync<object>("SendData", numberOfPackets, packetSizeInBytes, cancellationTokenSource.Token);
            var startDate = DateTime.Now;
            var receivedBytes = 0;
            var packetNumber = 0;

            await foreach (var data in stream.WithCancellation(cancellationTokenSource.Token))
            {
                packetNumber++;
                receivedBytes += isUsingMessagepack ? ((byte[])data).Length : Convert.FromBase64String(data.ToString()).Length;
                var diffTimeInSeconds = GetDiffTimeInSeconds(startDate);

                if (packetNumber % 100 == 0)
                    Trace.WriteLine($"Received data ({packetNumber * 100 / numberOfPackets}%). {packetNumber / diffTimeInSeconds} packets/s | Bandwidth={(receivedBytes / 1024) / (diffTimeInSeconds)} Kbytes/s");
            }

            Trace.WriteLine($"Streaming completed. It took {GetDiffTimeInSeconds(startDate)}s");
        }

        private static double GetDiffTimeInSeconds(DateTime startTime)
        {
            return (DateTime.Now - startTime).TotalMilliseconds / 1000;
        }




    }
















}
