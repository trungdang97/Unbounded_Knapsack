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
        public double selection_rate;

        //constructor
        public Chromosome(int length)
        {
            gene = new int[length];
        }

        //generate random chromosome value
        public void Generate(int bag_capacity, Item[] Items)
        {
            int max_volume = bag_capacity / Items.Min(i => i.Weight);
            int temp = bag_capacity;
            gene = new int[Items.Length];
            for (int i = 0; i < gene.Length; i++)
            {
                gene[i] = RandomNumber(0, bag_capacity / Items[i].Weight);
                bag_capacity -= gene[i] * Items[i].Weight;
            }
            Evaluate(Items, temp);
            BinaryEncode(max_volume);
            Decimal();
        }

        //operators
        public Chromosome Crossover(Chromosome pair, double mutation_rate, Item[] Items, int bag_capacity)
        {
            Chromosome child = new Chromosome(gene.Length);
            //uniform crossover
            for (int i = 0; i < binary.Length; i++)
            {
                if (RandomDouble() < 0.5)
                {
                    child.binary += binary[i];
                }
                else
                {
                    child.binary += pair.binary[i];
                }
            }
            child.Decimal();
            child.Mutate(mutation_rate, bag_capacity, Items);

            return child;
        }
        public void Mutate(double mutation_rate, int bag_capacity, Item[] Items)
        {
            int max_volume = bag_capacity / Items.Min(i => i.Weight);
            int temp_cap = 0;
            int temp_alelle = 0;
            for (int i = 0; i < gene.Length; i++)
            {
                if (RandomDouble() < mutation_rate)
                {
                    temp_alelle = gene[i];
                    temp_cap = total_weight - gene[i] * Items[i].Weight;
                    while (true)
                    {
                        gene[i] = RandomNumber(0, (bag_capacity - total_weight) / Items[i].Weight);
                        if (gene[i] != temp_alelle)
                        {
                            break;
                        }
                    }
                }
            }
            Evaluate(Items, bag_capacity);
            BinaryEncode(max_volume);
        }
        public void Evaluate(Item[] Items, int bag_capacity)
        {
            total_weight = 0;
            total_value = 0;
            for (int i = 0; i < gene.Length; i++)
            {
                total_weight += gene[i] * Items[i].Weight;
                total_value += gene[i] * Items[i].Value;
                if (total_weight > bag_capacity)
                {
                    total_value = -1;
                    break;
                }
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
        public static double RandomDouble()
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble();
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
