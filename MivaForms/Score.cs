using LibMIVA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MivaForms
{
    public static class Score
    {
        public enum AlgorithmType
        {
            Sharkfin,
            Parabola,
            Sinusoidal,
            //Custom,
            Old,
        }

        public enum VariantType
        {
            Normal,
            Cut,
            Bit,
            Symmetric
        }

        public delegate double ScoreFunction(double x, double m);

        public static Dictionary<AlgorithmType, ScoreFunction> FunctionMap = new Dictionary<AlgorithmType, ScoreFunction>()
        {
            { AlgorithmType.Old, (double x, double m) => { return -1; }  },
            { AlgorithmType.Sharkfin, CalculateSharkfin },
            { AlgorithmType.Parabola, CalculateParabola },
            //{ AlgorithmType.ParabolaBit, CalculateParabola },
            //{ AlgorithmType.ParabolaCut, CalculateParabola },
            { AlgorithmType.Sinusoidal, CalculateSinusoidal },
            //{ AlgorithmType.Custom, CalculateSharkfin },
        };

        public static ScoreFunction GetScoreFunction(AlgorithmType algorithmType, VariantType variantType)
        {
            ScoreFunction baseFunction = FunctionMap[algorithmType];

            ScoreFunction func;
            switch (variantType)
            {
                default:
                case VariantType.Normal:
                    func = baseFunction;
                    break;
                case VariantType.Cut:
                    func = (double x, double m) => { return x > m ? 0 : baseFunction(x, m); };
                    break;
                case VariantType.Bit:
                    func = (double x, double m) => { return x > m ? 1 / (x - m + 1) : baseFunction(x, m); };
                    break;
                case VariantType.Symmetric:
                    func = (double x, double m) => { return x > m ? baseFunction(m - (x - m) , m) : baseFunction(x, m); };
                    break;
            }

            return func;
        }
        public static double CalculateTotal(
            Knapsack knapsack, 
            ValuesTuple<double> baseMarketCapacity,
            ValuesTuple<double> demand,
            ValuesTuple<double> prices,
            AlgorithmType algorithmType,
            VariantType variantType)
        {
            ScoreFunction func = GetScoreFunction(algorithmType, variantType);

            double salesTotal = prices.Pech * knapsack.Pech + prices.Prom * knapsack.Prom + prices.Stan * knapsack.Stan;
            ValuesTuple<double> sales = new ValuesTuple<double>(prices.Pech * knapsack.Pech, prices.Prom * knapsack.Prom, prices.Stan * knapsack.Stan); 

            double pech = func(knapsack.Pech, baseMarketCapacity.Pech * demand.Pech);
            double prom = func(knapsack.Prom, baseMarketCapacity.Prom * demand.Prom);
            double stan = func(knapsack.Stan, baseMarketCapacity.Stan * demand.Stan);

            return (pech + prom + stan) / (demand.Sum());
            //return pech + prom + stan;
        }

        //x - ship count
        //m - market cap
        //Returns a value between 0 and 1
        public static double CalculateSharkfin(double x, double m)
        {
            //if (x > m) return 0;
            return x / m;
        }

        //x - ship count
        //m - market cap
        //Returns a value between 0 and 1
        public static double CalculateParabola(double x, double m)
        {
            return ( - 4 / Math.Pow(2 * m, 2) * Math.Pow(x, 2) ) + (4 / (2 * m) * x);
        }

        //x - ship count
        //m - market cap
        //Returns a value between 0 and 1
        public static double CalculateSinusoidal(double x, double m)
        {
            if (x > 2 * m) return 0;
            return 1 - ((Math.Cos(x * Math.PI / m) + 1) / 2);
        }
    }
}
