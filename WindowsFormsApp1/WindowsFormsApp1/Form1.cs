using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Threading;
using System.IO;
using System.Net.Sockets;

namespace WindowsFormsApp1
    // pobieranie danych z bazy,sortowanie wielowatkowe, wysyłanie danych do innej aplikacji
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public class elements
        {
            public elements(decimal[] a, decimal b, decimal c, decimal d, decimal e)
            {
                tabela = a;
                zmienna_0 = b;
                zmienna_1 = c;
                kmin = d;
                kdiff = e;
            }

            public decimal[] tabela;
            public decimal zmienna_0;
            public decimal zmienna_1;
            public decimal kmin;
            public decimal kdiff;

        }

        static void _sort(List<decimal> a) // sortowanie
        {
            for (int j = 1; j < a.Count; j++)
            {
                for (int i = 1; i < a.Count; i++)
                {
                    if (a[i - 1] > a[i])
                    {
                        decimal c = a[i - 1];
                        decimal d = a[i];
                        a[i] = c;
                        a[i - 1] = d;
                    }
                }
            }
        }

        static void write_temp(List<decimal> temp, int a)
        {
            for (int i = 0; i < temp.Count; i++)     
            {
                temp_1[a].Add(temp[i]);     
            }

        }

        static void TreadProc(object a) 
        {                               
            elements b = (elements)a;   
                                        
            decimal[] x = b.tabela;
            decimal y = b.zmienna_0;
            decimal z = b.zmienna_1;
            decimal min_temp = b.kmin;
            decimal diff_temp = b.kdiff;
            int id = 0;
            if (y == min_temp) id = 0;
            else if (y == min_temp + diff_temp / 4) id = 1;
            else if (y == min_temp + (diff_temp / 4) * 2) id = 2;
            else if (y == min_temp + (diff_temp / 4) * 3) id = 3;


            List<decimal> temp = new List<decimal>();   
            for (int i = 0; i < x.Length; i++)  
            {
                if (x[i] >= y && x[i] < z)     
                {
                    temp.Add(x[i]);         
                }
                else if (id == 3 && x[i] == z)
                {
                    temp.Add(x[i]);
                }
            }



            _sort(temp);        


            write_temp(temp, id); 

            Thread.Sleep(5000); // 5s przerwy

        }
        private void button1_Click(object sender, EventArgs e)
        {                                                                 //IPaddress:port
            OracleConnection MyConnection = new OracleConnection("Data source=;user id=ICTstudent;password=student;");
            OracleCommand mySqlCommand = MyConnection.CreateCommand();
            mySqlCommand.CommandText = "select ID,  Cast(col_A as Decimal(38,5)),Cast(col_B as Decimal(38,5)), Cast(col_C as Decimal(38,5)), Cast(col_D as Decimal(38,5)) from ICT.student_data";
            MyConnection.Open();

            
            if (MyConnection.State == ConnectionState.Open)
                MessageBox.Show("Stan Połączenie", "Połączenie zestawione", MessageBoxButtons.OK);
            

            OracleDataAdapter mySqlDataAdapter = new OracleDataAdapter(mySqlCommand);
         
            OracleCommandBuilder comBuild = new OracleCommandBuilder(mySqlDataAdapter);

            DataTable myDataTable = new DataTable();


            button1.Text = "ok";
            mySqlDataAdapter.Fill(myDataTable);
            MyConnection.Close();

            dataGridView1.DataSource = myDataTable;


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        decimal[] _array; 
        private void button2_Click(object sender, EventArgs e)
        {
            _array = new decimal[dataGridView1.RowCount - 1];
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("RESULTS", typeof(decimal));

            for (int i = 1; i < dataGridView1.Rows.Count; i++)
            {
                decimal liczba = (decimal)dataGridView1.Rows[i - 1].Cells[3].Value;
                decimal id = (decimal)dataGridView1.Rows[i - 1].Cells[0].Value;
                decimal x = 2 * liczba;
                table.Rows.Add(id, 2 * liczba);
                _array[i - 1] = x;
            }
            dataGridView2.DataSource = table;



        }

        static List<List<decimal>> temp_1;

        private void button3_Click(object sender, EventArgs e)
        {
            decimal max = _array.Max();
            decimal min = _array.Min();
            decimal diff = max - min;

            decimal r_0 = min;                        
            decimal r_1 = min + diff / 4;             



            List<elements> all_elem = new List<elements>();     

            temp_1 = new List<List<decimal>>();                     

            for (int i = 0; i < 4; i++)
            {
                temp_1.Add(new List<decimal>());    
            }                                   

            for (int i = 0; i < 4; i++)         
            {                                   
                decimal x = i * diff / 4;
                decimal a_1 = (r_0 + x);
                decimal a_2 = (r_1 + x);
                elements all = new elements(_array, a_1, a_2, min, diff);
                all_elem.Add(all);
            }

            Thread raz = new Thread(new ParameterizedThreadStart(TreadProc));   
            raz.Start(((object)all_elem[0]));

            Thread dwa = new Thread(new ParameterizedThreadStart(TreadProc));
            dwa.Start(((object)all_elem[1]));

            Thread trzy = new Thread(new ParameterizedThreadStart(TreadProc));
            trzy.Start(((object)all_elem[2]));

            Thread czt = new Thread(new ParameterizedThreadStart(TreadProc));
            czt.Start(((object)all_elem[3]));

            raz.Join();     
            dwa.Join();
            trzy.Join();
            czt.Join();

            int z = 0;  
            for (int i = 0; i < temp_1.Count; i++)  
            {
                if (i > 0)
                    z += temp_1[i - 1].Count;   
                for (int j = 0; j < temp_1[i].Count; j++)    
                {
                    _array[z + j] = temp_1[i][j];   
                }
            }

            for (int i = 0; i < 100000; i++)
            {
                dataGridView2.Rows[i].Cells[1].Value = _array[i];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TcpClient tcp = new TcpClient();
            try
            {
                tcp.Connect("127.0.0.1", 8001);
                ASCIIEncoding asciiEnc = new ASCIIEncoding();
                Stream stream = tcp.GetStream();
                
                    for(int i = 0; i <dataGridView2.Rows.Count-1 ; i++)
                    {
                        string str = "UPDATE ICT.Student_Results set FFNVXJFC=" + dataGridView2.Rows[i].Cells[1].Value.ToString() + " WHERE ID="+dataGridView2.Rows[i].Cells[0].Value.ToString() + ";";
                        string str2 =  str.Replace(",", ".");
                        if (str2.ToLower() == "quit") break;
                        byte[] binDataOut = asciiEnc.GetBytes(str2);
                        stream.Write(binDataOut, 0, binDataOut.Length);
                    
                    }
                tcp.Close();
            }

            catch
            {
                Console.WriteLine("error");
                Console.ReadLine();
            }

        }
    }
}

        

    
