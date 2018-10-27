using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unbounded_Knapsack
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Item[] Items = new Item[10];
            Chromosome[] list = new Chromosome[50];

            string weight_path = @"C:\Users\Trung\Desktop\Unbounded_Knapsack\Unbounded_Knapsack\bin\Debug\weight.txt";
            string value_path = @"C:\Users\Trung\Desktop\Unbounded_Knapsack\Unbounded_Knapsack\bin\Debug\value.txt";

            string[] weight_line = File.ReadAllLines(weight_path);
            string[] value_line = File.ReadAllLines(value_path);

            string[] weights = weight_line[0].Split(' ');
            string[] values = value_line[0].Split(' ');

            for (int i = 0; i < 10; i++)
            {
                Items[i] = new Item(int.Parse(weights[i]), int.Parse(values[i]));
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Individual", typeof(string));
            dt.Columns.Add("Chromosome", typeof(string));
            dt.Columns.Add("Weight", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            for (int i = 0; i < 50; i++)
            {
                list[i] = new Chromosome(10);
                list[i].Generate(100, Items);
                while (!list.Contains(list[i]))
                {
                    list[i].Generate(100, Items);
                }
                dt.Rows.Add(i, list[i].ToString(), list[i].total_weight, list[i].total_value);
            }

            dataGridView1.DataSource = dt;
        }
    }
}
