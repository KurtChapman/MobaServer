using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MobaServer.Transport
{
    public class SocketState
    {
        public Socket clientSocket = null;
        public const int READ_BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[READ_BUFFER_SIZE];
    }

    class SocketHandler
    {
        private static ManualResetEvent operationComplete = new ManualResetEvent(false);


        public SocketHandler()
        {
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

        private static void OnRead(IAsyncResult result)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            SocketState state = (SocketState)result.AsyncState;
            Socket handler = state.clientSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(result);
            string sent = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
            Console.WriteLine(sent);


            if (bytesRead > 0)
            {
                //handle the stuff
            }

            handler.BeginReceive(state.buffer, 0, SocketState.READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), state);
        }

        private static void OnAccept(IAsyncResult result)
        {
            // Signal the main thread to continue.  
            operationComplete.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            // Create the state object.  
            Console.WriteLine("Opened New Socket");
            SocketState state = new SocketState();
            state.clientSocket = handler;
            handler.BeginReceive(state.buffer, 0, SocketState.READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), state);
        }
    }
}
