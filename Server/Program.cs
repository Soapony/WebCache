using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class SynchronousSocketListener
{

    public static void StartListening()
    {
        // Data buffer for incoming data.  

        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and
        // listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.  
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();

                Console.WriteLine("Connection build");
                //initialize bytes array to receive package
                byte[] bytes = new byte[1024];

                //read the offset of package
                string data = null;
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                data = data.Substring(0, data.Length - 5);
                // Show the data on the console.  
                Console.WriteLine("Data received : {0}",data);

                if(string.Compare(data,"LIST") == 0)
                {
                    //return all the image in server
                    var files = Directory.GetFiles(Environment.CurrentDirectory+Path.DirectorySeparatorChar+"txt");
                    string filenames = null;
                    foreach(var file in files)
                    {
                        Console.WriteLine("File: {0}",file);
                        filenames += file + "<EOL>";
                    }
                    filenames += "<EOF>";
                    byte[] msg = Encoding.ASCII.GetBytes(filenames);
                    handler.Send(msg);
                }
                else
                {
                    //return the image.
                    using (FileStream fs = new FileStream(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "txt" + Path.DirectorySeparatorChar + data, FileMode.OpenOrCreate, FileAccess.Read))
                    { 
                        int fsLen=(int)fs.Length;
                        byte[] buffer = new byte[fsLen];
                        int r=fs.Read(buffer, 0, fsLen);
                        byte[] length = BitConverter.GetBytes(buffer.Length);
                        byte[] msg = new byte[buffer.Length + length.Length];
                        length.CopyTo(msg, 0);
                        buffer.CopyTo(msg,length.Length);
                        handler.Send(msg);
                    }
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}