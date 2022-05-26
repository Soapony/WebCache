using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    public partial class Form1 : Form
    {
        private static string exePath= System.IO.Directory.GetCurrentDirectory();
        public Form1()
        {
            DirectoryInfo dirDL = new DirectoryInfo(exePath+"\\Download");
            if (!dirDL.Exists)
            {
                dirDL.Create();
            }
            InitializeComponent();
        }
        private void Show_Click(object sender, EventArgs e)
        {
            string imageName = imageList.Text;
            // Data buffer for incoming data.  
            byte[] reclength = new byte[4];
            byte[] recBytes;
            string recTxt = null;
            //string recString = null;

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11001);

                // Create a TCP/IP  socket.  
                Socket Sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    Sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}", Sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(imageName+"<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = Sender.Send(msg);

                    // Receive the response from the remote device.
                    Sender.Receive(reclength);
                    int length = BitConverter.ToInt32(reclength, 0);
                    recBytes = new byte[length];
                    Sender.Receive(recBytes);
                    recTxt = Encoding.ASCII.GetString(recBytes);
                    //recString = recString.Substring(0, recString.Length - 5);

                    // Release the socket.  
                    Sender.Shutdown(SocketShutdown.Both);
                    Sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                }
                catch (Exception ne)
                {
                    Console.WriteLine("Unexpected exception : {0}", ne.ToString());
                }

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
            using (FileStream fs = new FileStream(exePath + "\\Download\\" + imageName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(recTxt);
                fs.Write(buffer,0,buffer.Length);
                txtData.Text = recTxt;
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            string recData = null;

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11001);

                // Create a TCP/IP  socket.  
                Socket Sender = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    Sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",Sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("LIST<EOF>");

                    // Send the data through the socket.  
                    int bytesSent = Sender.Send(msg);

                    // Receive the response from the remote device.
                    while (true)
                    {
                        int bytesRec = Sender.Receive(bytes);
                        recData += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (recData.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    Console.WriteLine("data receive = {0}",recData);

                    // Release the socket.  
                    Sender.Shutdown(SocketShutdown.Both);
                    Sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception ne)
                {
                    Console.WriteLine("Unexpected exception : {0}", ne.ToString());
                }

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
            imageList.Items.Clear();
            string[] separateS = { "<EOL>", "<EOF>" };
            string[] splitData = recData.Split(separateS, System.StringSplitOptions.RemoveEmptyEntries);
            for(int i=0;i<splitData.Length-1;i++)
            {
                int j;
                for(j=splitData[i].Length-1; j>0; j--)
                {
                    if (splitData[i][j] == '\\')
                        break;
                }
                imageList.Items.Add(splitData[i].Substring(j + 1, splitData[i].Length - j - 1));
            }
        }
    }
}