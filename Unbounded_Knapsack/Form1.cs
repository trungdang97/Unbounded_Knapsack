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
        Item[] Items = new Item[10];
        int pop = 50;
        int gene_length = 10;
        int bag_capacity = 2000;
        double mutation_prob = 0.3;
        double crossover_prob = 0.8;
        List<Chromosome> population;
        //List<Chromosome> next_gen = new List<Chromosome>();

        int k = 5;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            population = new List<Chromosome>(pop);
            for (int i = 0; i < 50; i++)
            {
                population.Add(new Chromosome(10));
                population[i].Generate(bag_capacity, Items);
                while (!population.Contains(population[i]))
                {
                    population[i].Generate(100, Items);
                }
            }
            SortDesc();
            ShowInitPop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < 1000; i++)
            {
                Search();
            }           
        }

        void Search()
        {
            //selection: tournament selection            
            Chromosome dad = new Chromosome(gene_length);
            Chromosome mom = new Chromosome(gene_length);

            for (int i = 0; i < pop / 2; i++)
            {
                dad = TournamentSelection();
                mom = TournamentSelection();
                population.Add(dad.Crossover(mom, mutation_prob, Items, bag_capacity));
            }
            SortDesc();
            while(population.Count > 50)
            {
                population.RemoveAt(population.Count - 1);
            }
            ShowPop();
        }

        Chromosome TournamentSelection()
        {
            List<Chromosome> tournament = new List<Chromosome>();
            Chromosome parent = new Chromosome(gene_length);
            int index = 0;
            for (int i = 0; i < k; i++)
            {
                while (true)
                {
                    index = RandomNumber(0, population.Count - 1);
                    if (!tournament.Contains(population[index]))
                    {
                        tournament.Add(population[index]);
                        break;
                    }
                }
            }
            tournament.OrderByDescending(o => o.total_value);
            return tournament[0];
        }

        void SortDesc()
        {
            population = population.OrderByDescending(o => o.total_value).ToList();
        }

        void ShowInitPop()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("i", typeof(string));
            dt.Columns.Add("Weight", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            for (int i = 0; i < 50; i++)
            {
                dt.Rows.Add(i + 1, population[i].total_weight, population[i].total_value);
            }

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = dt;
        }

        void ShowPop()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("i", typeof(string));
            dt.Columns.Add("Weight", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            for (int i = 0; i < population.Count; i++)
            {
                dt.Rows.Add(i+1,population[i].total_weight, population[i].total_value);
            }

            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.DataSource = dt;
        }

        //Utilities
        //random
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        public static double RandomDouble()
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble();
            }
        }
    }
}
