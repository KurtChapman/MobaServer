using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MobaServer
{
    class Server
    {
        private static ManualResetEvent operationComplete = new ManualResetEvent(false);
		  private List<Client> clients;

        public Server()
        {
				clients = new List<Client>();

            BeginListening();
        }

		  private void BeginListening()
        {
				TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
				server.Start(128);
				Console.WriteLine("Server has started on 127.0.0.1:8080.\nWaiting for a connection...");

				while(true)
				{
					 // Set the event to nonsignaled state.  
					 operationComplete.Reset();

					 server.BeginAcceptTcpClient(OnAccept, server);

					 // Wait until a connection is made before continuing.  
					 operationComplete.WaitOne();
				}
		  }

        private void OnAccept(IAsyncResult result)
        {

				// Signal the main thread to continue.  
				operationComplete.Set();

				// Get the socket that handles the client request.  
				TcpListener listener = (TcpListener)result.AsyncState;
				TcpClient socket = listener.EndAcceptTcpClient(result);

            // Create the client socket and track it.
            Console.WriteLine("Connection started, created a new client");
				Client client = new Client(socket);

				clients.Add(client);
        }
    }
}
