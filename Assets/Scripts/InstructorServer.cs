using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

// Adopted from https://gist.github.com/danielbierwirth/0636650b005834204cb19ef5ae6ccedb
public class InstructorServer : MonoBehaviour
{
    /// <summary> 	
    /// TCPListener to listen for incoming TCP connection 	
    /// requests. 	
    /// </summary> 	
    private TcpListener _tcpListener;

    /// <summary> 
    /// Background thread for TcpServer workload. 	
    /// </summary> 	
    private Thread _tcpListenerThread;

    /// <summary> 	
    /// Create handle to connected tcp client. 	
    /// </summary> 	
    private TcpClient _connectedTcpClient;

    public MasterInstructor MasterInstructor;
    public Queue<DecodedMessage> MessageQueue = new Queue<DecodedMessage>();

    private void Start()
    {
        // Start TcpServer background thread 		
        _tcpListenerThread = new Thread(ListenForIncomingRequests) {IsBackground = true};
        _tcpListenerThread.Start();
    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incoming TcpClient requests 	
    /// </summary> 	
    private void ListenForIncomingRequests()
    {
        try
        {
            // Create listener on localhost port 8052. 			
            _tcpListener = new TcpListener(IPAddress.Any, 38823);
            _tcpListener.Start();
            Debug.Log("Server is listening");
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (_connectedTcpClient = _tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading 					
                    using (NetworkStream stream = _connectedTcpClient.GetStream())
                    {
                        int length;
                        // Read incoming stream into byte array. 						
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // Convert byte array to string message. 							
                            var clientTextMessage = Encoding.UTF8.GetString(incomingData);
                            Debug.Log("client message received as: " + clientTextMessage);
                            try
                            {
                                var clientMessage = DecodedMessage.FromJsonString(clientTextMessage);
                                MessageQueue.Enqueue(clientMessage);
                            }
                            catch (ArgumentException e)
                            {
                                Debug.Log(e);
                                SendMessageToClient(DecodedMessage.ErrorMessage(MessageError.ParseError));
                            }
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException);
        }
    }

    public void Update()
    {
        while (MessageQueue.Count != 0)
        {
            var lastMessage = MessageQueue.Dequeue();
            MasterInstructor.HandleCommand(lastMessage);
        }
    }

    /// <summary> 	
    /// Send message to client using socket connection. 	
    /// </summary> 	
    public void SendMessageToClient(DecodedMessage serverDecodedMessage)
    {
        if (_connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            var stream = _connectedTcpClient.GetStream();
            if (!stream.CanWrite) return;
            // Convert string message to byte array.                 
            var serverMessageAsByteArray = Encoding.UTF8.GetBytes(serverDecodedMessage.ToJsonString());
            // Write byte array to socketConnection stream.               
            stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
            Debug.Log("Server sent his message - should be received by client");
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    
    
}