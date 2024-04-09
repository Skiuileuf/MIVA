using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVA
{
    class Knapsack
    {
        public double Score;
        public double Pech;
        public double Prom;
        public double Stan;

        public Product Delta;

        public override string ToString()
        {
            return $"Score: {Score}\t{Pech,2} Pech, {Prom,2} Prom, {Stan,2} Stan w/rem \t{Delta}";
        }
    }
}
