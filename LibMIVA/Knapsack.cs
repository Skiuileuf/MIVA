using LibMIVA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMIVA
{
    public class Knapsack
    {
        public double Pech { get; set; }
        public double Prom { get; set; }
        public double Stan { get; set; }

        public Product Delta;

        public Product Cost;

        public override string ToString()
        {
            return $"{Pech,2} Pech, {Prom,2} Prom, {Stan,2} Stan w/rem \t{Delta}";
        }
    }
}
