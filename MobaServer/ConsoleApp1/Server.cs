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
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            socket.Listen(128);
            

            while (true)
            {
                // Set the event to nonsignaled state.  
                operationComplete.Reset();

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine("Waiting for a connection...");
                socket.BeginAccept(null, 0, new AsyncCallback(OnAccept), socket);

                // Wait until a connection is made before continuing.  
                operationComplete.WaitOne();
            }
        }

        private void OnAccept(IAsyncResult result)
        {
            // Signal the main thread to continue.  
            operationComplete.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            // Create the client socket and track it.
            Console.WriteLine("Connection started, created a new client");
				Client client = new Client(handler);
				clients.Add(client);
        }
    }
}
