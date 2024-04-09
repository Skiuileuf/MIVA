using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIVA
{
    internal static class EfficientProduction
    {
        public static void Run()
        {
            const double workHoursPerRobot = 200;
            //Limits
            double StockPlastic = ConsoleUtils.ReadInput("Plastic", 86.0); //Tonnes
            double StockWood = ConsoleUtils.ReadInput("Wood", 43.0); //Tonnes
            double StockWorkshopPlastic = workHoursPerRobot * ConsoleUtils.ReadInput("Moulding Workshop Robots", 6); //Moulding - 200 work hours * 6 robots
            double StockWorkshopWood = workHoursPerRobot * ConsoleUtils.ReadInput("Finish Workshop Robots", 7); //Finish - 200 wkh * 7 robots

            //Score calculation

            //double PechDemandMultiplier = 0.8;
            double PechDemandMultiplier = ConsoleUtils.ReadInput("Pech demand multiplier", 0.8);
            double PromDemandMultiplier = ConsoleUtils.ReadInput("Prom demand multiplier", 0.7);
            double StanDemandMultiplier = ConsoleUtils.ReadInput("Stan demand multiplier", 0.8);

            double PechSalePrice = ConsoleUtils.ReadInput("Pech Sale Price", 6000);
            double PromSalePrice = ConsoleUtils.ReadInput("Prom Sale Price", 10000);
            double StanSalePrice = ConsoleUtils.ReadInput("Stan Sale Price", 30000);

            Product STOCK = new Product(StockPlastic, StockWorkshopPlastic, StockWood, StockWorkshopWood, "STOCK");

            Console.WriteLine($"STOCK INITIAL -> {STOCK}");

            //Calculate all possible combinations of production
            //First, calculate how many at most can be produced of each ship, assuming all resources would be used for it

            int maxPechCount = (int)Math.Floor(CalculateMaxProducts(Product.Pech, STOCK)) + 1;
            int maxPromCount = (int)Math.Floor(CalculateMaxProducts(Product.Prom, STOCK)) + 1;
            int maxStanCount = (int)Math.Floor(CalculateMaxProducts(Product.Stan, STOCK)) + 1;

            //Create the variants table
            Knapsack[,,] variants = new Knapsack[maxPechCount, maxPromCount, maxStanCount];

            //Foreach variant in the table, calculate if that production variant is possible
            for (int i = 0; i < maxPechCount; i++)
            {
                for (int j = 0; j < maxPromCount; j++)
                {
                    for (int k = 0; k < maxStanCount; k++)
                    {
                        double plasticUsage = i * Product.Pech.Plastic + j * Product.Prom.Plastic + k * Product.Stan.Plastic;
                        double plasticWHUsage = i * Product.Pech.PlasticWorkshopHours + j * Product.Prom.PlasticWorkshopHours + k * Product.Stan.PlasticWorkshopHours;
                        double woodUsage = i * Product.Pech.Wood + j * Product.Prom.Wood + k * Product.Stan.Wood;
                        double woodWHUsage = i * Product.Pech.WoodWorkshopHours + j * Product.Prom.WoodWorkshopHours + k * Product.Stan.WoodWorkshopHours;

                        double deltaPlastic = StockPlastic - plasticUsage;
                        double deltaPlWH = StockWorkshopPlastic - plasticWHUsage;
                        double deltaWood = StockWood - woodUsage;
                        double deltaWoWH = StockWorkshopWood - woodWHUsage;

                        double score;
                        if (deltaPlastic < 0 || deltaPlWH < 0 || deltaWood < 0 || deltaWoWH < 0)
                        {
                            //Variant can't be made using available resources
                            score = -1;
                        }
                        else
                        {
                            //Variant can be made, calculate score:
                            //score = deltaPlastic + deltaWood + deltaPlWH/1000 + deltaWoWH/1000;

                            //Calculate score by profit TODO FIXME CALC ACTUAL PROFIT
                            score = (PechSalePrice * PechDemandMultiplier * i) * 1 +
                                    (PromSalePrice * PromDemandMultiplier * j) * 1 +
                                    (StanSalePrice * StanDemandMultiplier * k) * 0.5;
                        }

                        variants[i, j, k] = new Knapsack()
                        {
                            Score = score,
                            Pech = i,
                            Prom = j,
                            Stan = k,
                            Delta = new Product(deltaPlastic, deltaPlWH, deltaWood, deltaWoWH, "DELTA")
                        };
                    }
                }
            }


            List<Knapsack> variantsFlat = new List<Knapsack>();
            for (int i = 0; i < maxPechCount; i++)
            {
                for (int j = 0; j < maxPromCount; j++)
                {
                    for (int k = 0; k < maxStanCount; k++)
                    {
                        Knapsack var = variants[i, j, k];
                        if (!(var.Score < 0))
                        {
                            variantsFlat.Add(var);
                        }
                    }
                }
            }

            variantsFlat.Sort((a, b) => {
                if (a.Score == b.Score) return 0;
                else return a.Score < b.Score ? 1 : -1;
            });

            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine(variantsFlat[i]);
            }

            //foreach (var v in variantsFlat)
            //{
            //    //if (v.Delta.Wood > 0.25 || v.Delta.Plastic > 0.25) break;

            //    Console.WriteLine(v.ToString());
            //}
        }

        static double CalculateMaxProducts(Product p, Product stock)
        {
            double plastic = stock.Plastic / p.Plastic;
            double plasticworkshophours = stock.PlasticWorkshopHours / p.PlasticWorkshopHours;
            double wood = stock.Wood / p.Wood;
            double woodworkshophours = stock.Wood / p.Wood;

            return new double[] { plastic, plasticworkshophours, wood, woodworkshophours }.Min();
        }
    }
}
