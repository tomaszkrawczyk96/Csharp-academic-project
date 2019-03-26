using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            OracleConnection connection = new OracleConnection();                                    //IP address:port    
            connection.ConnectionString = "User Id = ICTstudent;" + "Password = student;" + "Data Source = ;";
            connection.Open();
            Console.WriteLine("Connected to Oracle " + connection.ServerVersion);

            // wstawanie to tabeli
            // OracleCommand cmd = connection.CreateCommand();
            //cmd.CommandText = "INSERT INTO ICT.persons (STUDENT_ID,FIRST_NAME,LAST_NAME) values(131013,'Tomasz','Krawczyk')";
            //cmd.ExecuteNonQuery();

            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select * from ICT.Student_tasks where STUDENT_ID = 131013";
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["STUDENT_ID"].ToString() + " " + reader["TASK"].ToString()+ " "  +reader["TABLE_NAME"].ToString());
            }


            Console.ReadLine();


        }
    }
}
