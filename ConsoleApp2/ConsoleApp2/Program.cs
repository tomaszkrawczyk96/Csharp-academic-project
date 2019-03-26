using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Oracle.ManagedDataAccess.Client;
public class server
{
    public static void Main()
    {
        bool connected = false;
        try
        {
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            TcpListener myListener = new TcpListener(ipAddr, 8001);
            myListener.Start();
            Console.WriteLine("The server is running at port: " + myListener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection.....");
            Socket socket = myListener.AcceptSocket();
            Console.WriteLine("Connection accepted from" + socket.RemoteEndPoint);
            connected = true;


            byte[] binDataIn = new byte[255];
            string pmsg = "";
            while (true)
            {
                int k = socket.Receive(binDataIn);
                if (k == 0) break;
                ASCIIEncoding asciiEnc = new ASCIIEncoding();
                string msg = asciiEnc.GetString(binDataIn, 0, k);       // msg to wiadomosc ktora odbieram
                Console.WriteLine(msg);
                pmsg += msg;
            }

            string[] msg_list = pmsg.Split(';');
            OracleConnection connection = new OracleConnection();                                        //IPaddress:port
            connection.ConnectionString = "User Id = ICTstudent;" + "Password = student;" + "Data Source = ;";
            connection.Open();
            Console.WriteLine("Connected to Oracle " + connection.ServerVersion);
            OracleCommand cmd = connection.CreateCommand();
            for (int i = 0; i < msg_list.Length - 1; i++)
            {
                cmd.CommandText = msg_list[i];
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("job's done!");
            socket.Close();
            myListener.Stop();
        }
        catch (Exception e) { if (!connected) Console.WriteLine("Error..... " + e.ToString()); }
        Console.WriteLine("\nClient closed connection");
        Console.ReadLine();
    }
}
