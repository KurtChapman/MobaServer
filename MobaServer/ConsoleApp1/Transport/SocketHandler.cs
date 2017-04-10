using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MobaServer.Transport
{
    public class SocketState
    {
        public Socket ClientSocket = null;
        public const int READ_BUFFER_SIZE = 1024;
        public byte[] ReadBuffer = new byte[READ_BUFFER_SIZE];
    }

    class SocketHandler
    {
        private static ManualResetEvent operationComplete = new ManualResetEvent(false);
        private PacketParser packetParser;

        public SocketHandler(PacketParser parser)
        {
            this.packetParser = parser;

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

        private void OnRead(IAsyncResult result)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            SocketState state = (SocketState)result.AsyncState;
            Socket handler = state.ClientSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(result);
            if (bytesRead > 0)
            {
                //handle the stuff
                packetParser.OnData(state.ReadBuffer);
                Array.Clear(state.ReadBuffer, 0, state.ReadBuffer.Length);
            }

            //start listening again.
            handler.BeginReceive(state.ReadBuffer, 0, SocketState.READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), state);
        }

        private void OnAccept(IAsyncResult result)
        {
            // Signal the main thread to continue.  
            operationComplete.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)result.AsyncState;
            Socket handler = listener.EndAccept(result);

            // Create the state object.  
            Console.WriteLine("Opened New Socket");
            SocketState state = new SocketState();
            state.ClientSocket = handler;
            handler.BeginReceive(state.ReadBuffer, 0, SocketState.READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), state);
        }
    }
}
