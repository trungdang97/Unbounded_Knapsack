using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbounded_Knapsack
{
    class Item
    {
        private int weight;
        private int value;

        public Item(int weight, int value)
        {
            this.weight = weight;
            this.value = value;
        }

        public int Weight
        {
            get
            {
                return weight;
            }

            set
            {
                weight = value;
            }
        }

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }
}
