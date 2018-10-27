using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unbounded_Knapsack
{
    class Chromosome
    {
        public int[] gene;
        public string binary;
        public int total_weight;
        public int total_value;

        //constructor
        public Chromosome(int length)
        {
            gene = new int[length];
        }

        //generate random chromosome value
        public void Generate(int bag_capacity, Item[] Items)
        {
            int max_volume = bag_capacity / Items.Min(i => i.Weight);
            gene = new int[Items.Length];
            for (int i = 0; i < gene.Length; i++)
            {
                gene[i] = RandomNumber(0, bag_capacity / Items[i].Weight);
                bag_capacity -= gene[i] * Items[i].Weight;
            }
            Evaluate(Items);
            BinaryEncode(max_volume);
            Decimal();

            //Thread.Sleep(70);
        }

        //operators
        public void Crossover(Chromosome pair)
        {
            Random rand = new Random();
            Chromosome child
            //uniform crossover
            for(int i = 0; i < binary.Length; i++)
            {
                if (rand.NextDouble() < 0.5)
                {

                }
            }
        }
        public void Mutate(double mutation_rate, int bag_capacity, Item[] Items)
        {
            int max_volume = bag_capacity / Items.Min(i => i.Weight);
            Random rand = new Random();
            for (int i = 0; i < gene.Length; i++)
            {
                if (rand.NextDouble() < mutation_rate)
                {
                    gene[i] = RandomNumber(0, bag_capacity / Items[i].Weight);
                }
            }
            Evaluate(Items);
            BinaryEncode(max_volume);
        }
        public void Evaluate(Item[] Items)
        {
            for (int i = 0; i < gene.Length; i++)
            {
                total_weight += gene[i] * Items[i].Weight;
                total_value += gene[i] * Items[i].Value;
            }
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

        //print integer
        public override string ToString()
        {
            string chromosome = "";
            for (int i = 0; i < gene.Length; i++)
            {
                chromosome += " " + gene[i].ToString();
            }
            return chromosome;
        }

        //convert to binary
        public void BinaryEncode(int max_volume)
        {
            int max_bit = Convert.ToString(max_volume, 2).Length;
            string item_binary = "";
            binary = "";
            for (int i = 0; i < gene.Length; i++)
            {
                item_binary = Convert.ToString(gene[i], 2);
                while (item_binary.Length < max_bit)
                {
                    item_binary = "0" + item_binary;
                }
                binary += item_binary + " ";
                item_binary = "";
            }
        }

        //convert back to decimal
        public void Decimal()
        {
            string[] bin = binary.Split(' ');
            for (int i = 0; i < gene.Length; i++)
            {
                gene[i] = Convert.ToInt32(bin[i], 2);
            }
        }
    }
}
