using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
namespace CacheServer
{
    public partial class Form1 : Form
    {
        private string log = null;
        private static string exePath = System.IO.Directory.GetCurrentDirectory();
        private List<string> cachelog=new List<string>();
        private bool flag = false;
        Thread socketThread;
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            DirectoryInfo dirDL = new DirectoryInfo(exePath + "\\cache");
            if (!dirDL.Exists)
            {
                dirDL.Create();
            }
        }

        public void appendLog(string s)
        {
            log += s;
            Log.Text = log;
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(exePath +  "\\cache");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            cacheLog.Clear();
            cachelog.Clear();
        }

        public void cacheSocket()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11001);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                while (true)
                {

                    Socket handler = listener.Accept();

                    //initialize bytes array to receive package
                    List<byte> recData = new List<byte>();
                    byte[] bytes = new byte[1024];
                    byte[] reclength = new byte[4];
                    byte[] recBytes;

                    //read the offset of package
                    string byteString = null;
                    //string recString = null;

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        byteString += Encoding.ASCII.GetString(bytes,0,bytesRec);
                        if (byteString.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    byte[] forwardMsg = Encoding.ASCII.GetBytes(byteString);
                    byteString = byteString.Substring(0, byteString.Length - 5);
                    if (string.Compare(byteString,"LIST") == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        string date = DateTime.Now.ToString("yyyy-MM-dd");
                        string time = DateTime.Now.ToLongTimeString().ToString();
                        string tmp = "user request: file " + byteString + " at " + time + " " + date+"\r\n";
                        appendLog(tmp);
                    }

                    if (!flag && cachelog.Contains(byteString))
                    {
                        string tmp = "response: cached file " + byteString+"\r\n";
                        appendLog(tmp);
                        using (FileStream fs = new FileStream(exePath+"\\cache\\" + byteString, FileMode.OpenOrCreate, FileAccess.Read))
                        {
                            int fsLen = (int)fs.Length;
                            byte[] buffer = new byte[fsLen];
                            int r = fs.Read(buffer, 0, fsLen);
                            byte[] length = BitConverter.GetBytes(buffer.Length);
                            byte[] msg = new byte[buffer.Length + length.Length];
                            length.CopyTo(msg, 0);
                            buffer.CopyTo(msg, length.Length);
                            handler.Send(msg);
                        }
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        continue;
                    }

                    try
                    {
                        IPHostEntry ipHostInfo1 = Dns.GetHostEntry("localhost");
                        IPAddress ipAddress1 = ipHostInfo1.AddressList[0];
                        IPEndPoint remoteEP = new IPEndPoint(ipAddress1, 11000);

                        // Create a TCP/IP  socket.  
                        Socket Sender1 = new Socket(ipAddress1.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        // Connect the socket to the remote endpoint. Catch any errors.  
                        try
                        {
                            Sender1.Connect(remoteEP);

                            // Send the data through the socket.  
                            int bytesSent = Sender1.Send(forwardMsg);


                            // Receive the response from the remote device.
                            if (flag)
                            {
                                while (true)
                                {
                                    int bytesRec = Sender1.Receive(bytes);
                                    recData.AddRange(bytes);
                                    string recString = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                    if (recString.IndexOf("<EOF>") > -1)
                                    {
                                        break;
                                    }
                                }
                                byte[] msg = recData.ToArray();
                                handler.Send(msg);
                                flag = false;
                            }
                            else
                            {
                                Sender1.Receive(reclength);
                                int length = BitConverter.ToInt32(reclength, 0);
                                recBytes = new byte[length];
                                Sender1.Receive(recBytes);
                                string tmp = "response: file " + byteString + " downloaded from the server\r\n";
                                appendLog(tmp);
                                using (FileStream fs = new FileStream(exePath + "\\cache\\" + byteString, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    fs.Write(recBytes, 0, recBytes.Length);
                                }
                                cachelog.Add(byteString);
                                tmp = string.Join("\r\n", cachelog.ToArray());
                                cacheLog.Text = tmp;

                                byte[] msg = new byte[reclength.Length + recBytes.Length];
                                reclength.CopyTo(msg, 0);
                                recBytes.CopyTo(msg, reclength.Length);
                                handler.Send(msg);

                            }
                            // Release the socket.  
                            Sender1.Shutdown(SocketShutdown.Both);
                            Sender1.Close();

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
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            socketThread = new Thread(new ThreadStart(cacheSocket));
            socketThread.IsBackground = true;
            socketThread.Start();
        }
    }
}