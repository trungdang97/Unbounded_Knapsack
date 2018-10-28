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
        Item[] Items;
        int pop = 30;
        int gene_length = 0;
        int bag_capacity = 0;
        double mutation_prob = 0.3;
        List<Chromosome> population;

        int k = 3; // tournament size
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            button3.Enabled = false;

            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;

            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select data";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filepath = openFileDialog1.FileName;
                string file;

                file = File.ReadAllText(filepath);
                string[] intValues = file.Split(new char[] { '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                //leave out number or items and capacity 
                int[] parsed = new int[intValues.Length - 2];
                //
                gene_length = int.Parse(intValues[0]);
                for (int i = 1; i < intValues.Length - 1; i++)
                {
                    if (int.Parse(intValues[i]) != 0)
                    {
                        parsed[i - 1] = int.Parse(intValues[i]);
                    }
                }
                bag_capacity = int.Parse(intValues[intValues.Length - 1]);

                Items = new Item[gene_length];
                
                for (int i = 0,j=0; i < parsed.Length / 2; i++, j=j+2)
                {
                    Items[i] = new Item(parsed[j], parsed[j + 1]);                  
                }
            }
            InitPop();
            //textBox1.Text = "1000";
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            //int iterations = int.Parse(textBox1.Text);
            int i = 0;
            Chromosome best = new Chromosome(gene_length);

            while (population[0].total_value != population.Sum(o => o.total_value) / pop)
            //while (i < 1000)
            {
                if (best.total_value < population[0].total_value)
                {
                    best.Clone(population[0]);
                }
                Search();
                i++;
            }
            textBox1.Text = i.ToString();
            ShowPop();

            ShowBest(best);

            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
        }

        void Search()
        {
            //r++;
            //selection: tournament selection            
            Chromosome dad = new Chromosome(gene_length);
            Chromosome mom = new Chromosome(gene_length);
            Chromosome fresh = new Chromosome(gene_length);
            //crossover + mutation
            while (population.Count < pop + pop / 2)
            {
                dad = TournamentSelection();
                mom = TournamentSelection();
                fresh = dad.Crossover(mom, mutation_prob, Items, bag_capacity);
                if (!population.Contains(fresh) && fresh.total_weight <= bag_capacity)
                {
                    population.Add(new Chromosome(gene_length));
                    population[population.Count - 1].Clone(fresh);
                }
            }
            while (population.Count < pop * 2)
            {                
                fresh.Generate(bag_capacity, Items);
                if (!population.Contains(fresh) && fresh.total_weight <= bag_capacity)
                {
                    population.Add(new Chromosome(gene_length));
                    population[population.Count - 1].Clone(fresh);
                }
            }
            SortDesc();
            population.RemoveRange(pop, pop);

            //adaptive
            //if (R % 5 == 0)
            //{
            //    int new_pop = pop * 2;
            //    for (int i = pop; i < new_pop; i++)
            //    {
            //        population.Add(new Chromosome(gene_length));
            //        population[i].Generate(bag_capacity, Items);
            //    }
            //    pop = new_pop;
            //}
            ////sort descending
            //SortDesc();
        }

        Chromosome TournamentSelection()
        {
            List<Chromosome> tournament = new List<Chromosome>();
            Chromosome parent = new Chromosome(gene_length);
            int index = 0;
            int max = population.Count - 1;
            for (int i = 0; i < k; i++)
            {
                while (true)
                {
                    index = RandomNumber(0, max);
                    if (!tournament.Contains(population[index]))
                    {
                        tournament.Add(population[index]);
                        break;
                    }
                }
            }
            tournament = tournament.OrderByDescending(o => o.total_value).ToList();
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
            for (int i = 0; i < pop; i++)
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
                dt.Rows.Add(i + 1, population[i].total_weight, population[i].total_value);
            }

            dataGridView2.RowHeadersVisible = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.DataSource = dt;
        }

        void InitPop()
        {
            population = new List<Chromosome>(pop);
            Chromosome temp = new Chromosome(gene_length);
            bool check;
            for (int i = 0; i < pop; i++)
            {
                if (i == 0)
                {
                    population.Add(new Chromosome(10));
                    population[i].Generate(bag_capacity, Items);
                }
                else
                {
                    population.Add(new Chromosome(10));
                    while (true)
                    {
                        temp.Generate(bag_capacity, Items);
                        check = population.Contains(temp);
                        if (!check)
                        {
                            population[i].Clone(temp);
                            break;
                        }
                    }
                }
            }
            SortDesc();
            ShowInitPop();
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

        //reset
        private void button3_Click(object sender, EventArgs e)
        {
            population.Clear();
            //dataGridView1.DataSource = dt;
            InitPop();
            dataGridView2.DataSource = null;
            button2.Enabled = true;
            button3.Enabled = false;

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            dataGridView3.DataSource = null;
        }
        //show best current converged solution
        void ShowBest(Chromosome best)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Items", typeof(string));
            dt.Columns.Add("Weight", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            for (int i = 0; i < gene_length; i++)
            {
                dt.Rows.Add(i + 1, Items[i].Weight, Items[i].Value, best.gene[i]);
            }

            dataGridView3.RowHeadersVisible = false;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.DataSource = dt;

            textBox2.Text = best.total_weight.ToString();
            textBox3.Text = best.total_value.ToString();
        }
    }
}
