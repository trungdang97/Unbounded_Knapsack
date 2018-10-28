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
        int pop = 20;
        int gene_length = 0;
        int bag_capacity = 0;
        double mutation_prob = 0.3;
        double crossover_prob = 0.8;
        
        List<Chromosome> population;
        List<Chromosome> new_population = new List<Chromosome>();

        //List<Chromosome> next_gen = new List<Chromosome>();

        int k = 5; // tournament size
        int r = 5;

        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select data";
            DialogResult result = openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
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
                    if (int.Parse(intValues[i])!=0)
                    {
                        parsed[i - 1] = int.Parse(intValues[i]);
                    }                                         
                }
                bag_capacity = int.Parse(intValues[intValues.Length - 1]);

                Items = new Item[gene_length];
                int j = 0;
                for (int i = 0; i < parsed.Length / 2; i++)
                {
                    Items[i] = new Item(parsed[j], parsed[j + 1]);
                    j++;
                }
            }
            InitPop();            
            textBox1.Text = "1000";
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;        
            int iterations = int.Parse(textBox1.Text);
            int i = 0;
            Chromosome best = new Chromosome(gene_length);           
            
            while(i<iterations)
            {
                if (best.total_value < population[0].total_value)
                {
                    best.Clone(population[0]);
                    //i = 0;
                }
                Search();
                i++;
            }
            textBox1.Text = i.ToString();
            ShowPop();
            button1.Enabled = true;
            button2.Enabled = true;
        }

        void Search()
        {
            r++;
            //selection: tournament selection            
            Chromosome dad = new Chromosome(gene_length);
            Chromosome mom = new Chromosome(gene_length);
            
            SortAsc();
            SelectionRate();

            //elitism: keep 1-2 best individual(s)
            new_population.Add(new Chromosome(gene_length));
            new_population[0].Clone(population[population.Count - 1]);

            //crossover + mutation
            while(new_population.Count < pop)
            {
                if (RandomDouble() < crossover_prob)
                {
                    dad = RouletteSelection();
                    mom = RouletteSelection();
                    new_population.Add(dad.Crossover(mom, mutation_prob, Items, bag_capacity));
                    if(new_population[new_population.Count-1].total_weight > bag_capacity)
                    {
                        new_population.RemoveAt(new_population.Count - 1);
                    }
                }
                else
                {
                    new_population.Add(RouletteSelection());
                }
            }
            //replace the old population
            population.Clear();
            population.AddRange(new_population);
            new_population.Clear();

            //sort descending
            SortDesc();
            while (population.Count > pop)
            {
                population.RemoveRange(pop, population.Count - pop);
            }

            //adaptive
            //if (r % 200 == 0)
            //{
            //    int new_pop = pop * 2;
            //    for (int i = pop; i < new_pop; i++)
            //    {
            //        population.Add(new Chromosome(gene_length));
            //        population[i].Generate(bag_capacity, Items);
            //    }
            //    pop = new_pop;
            //}
        }

        Chromosome RouletteSelection()
        {
            Chromosome parent = new Chromosome(gene_length);
            double cumulative = 0;
            for(int i = 0; i < population.Count; i++)
            {
                cumulative += population[i].selection_rate;
                if(cumulative >= RandomDouble())
                {
                    parent.Clone(population[i]);
                    break;
                }
            }
            return parent;
        }
        void SelectionRate()
        {            
            int ValueSum = population.Sum(o=>o.total_value);
            population[0].selection_rate = (double)population[0].total_value / ValueSum;
            for (int i = 1; i < population.Count; i++)
            {
                population[i].selection_rate = (double)population[i].total_value / ValueSum + population[i-1].selection_rate;
            }
        }

        void SortDesc()
        {
            population = population.OrderByDescending(o => o.total_value).ToList();
        }
        void SortAsc()
        {
            population = population.OrderBy(o => o.total_value).ToList();
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

        private void button3_Click(object sender, EventArgs e)
        {
            population.Clear();
            //dataGridView1.DataSource = dt;
            InitPop();
            dataGridView2.DataSource = null;
        }
    }
}
