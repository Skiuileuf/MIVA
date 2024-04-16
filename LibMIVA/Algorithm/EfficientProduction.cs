using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMIVA.Algorithm
{
    public class EfficientProduction : Algorithm
    {
        //Limits
        //TONNES
        public double StockPlastic;
        public double StockWood;
        // WORK HOURS (multiply robot count by workhoursperrobot
        public double StockWorkshopPlastic;
        public double StockWorkshopWood;

        //Score calculation
        public double PechDemandMultiplier;
        public double PromDemandMultiplier;
        public double StanDemandMultiplier;

        public double PechSalePrice; 
        public double PromSalePrice;
        public double StanSalePrice;

        public void SetInitialStock(double StockPlastic, double StockWood, double RobotsPlastic, double RobotsWood)
        {
            this.StockPlastic = StockPlastic;
            this.StockWood = StockWood;
            this.StockWorkshopPlastic = RobotsPlastic * workHoursPerRobot;
            this.StockWorkshopWood = RobotsWood * workHoursPerRobot;
        }

        public void SetDemandMultipliers(double Pech, double Prom, double Stan)
        {
            this.PechDemandMultiplier = Pech;
            this.PromDemandMultiplier = Prom;
            this.StanDemandMultiplier = Stan;
        }

        public void SetSalePrices(double Pech, double Prom, double Stan)
        {
            this.PechSalePrice = Pech;
            this.PromSalePrice = Prom;
            this.StanSalePrice = Stan;
        }

        public override List<Knapsack> Run()
        {
            Product STOCK = new Product(StockPlastic, StockWorkshopPlastic, StockWood, StockWorkshopWood, "STOCK");

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

                        if ( !(deltaPlastic < 0 || deltaPlWH < 0 || deltaWood < 0 || deltaWoWH < 0)  )
                        {
                            //Variant can't be made using available resources
                            variants[i, j, k] = new Knapsack()
                            {
                                //Score = score,
                                Pech = i,
                                Prom = j,
                                Stan = k,
                                Delta = new Product(deltaPlastic, deltaPlWH, deltaWood, deltaWoWH, "DELTA"),
                                Cost = new Product(plasticUsage, plasticWHUsage, woodUsage, woodWHUsage, "COST")
                            };
                        }
                        //else
                        //{
                        //    //Variant can be made, calculate score:
                        //    //score = deltaPlastic + deltaWood + deltaPlWH/1000 + deltaWoWH/1000;

                        //    //Calculate score by profit TODO FIXME CALC ACTUAL PROFIT
                        //    //score = (PechSalePrice * PechDemandMultiplier * i) * 1 +
                        //    //        (PromSalePrice * PromDemandMultiplier * j) * 1 +
                        //    //        (StanSalePrice * StanDemandMultiplier * k) * 0.5;
                            
                        //}
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
                        if (var != null)
                        {
                            variantsFlat.Add(var);
                        }
                    }
                }
            }

            return variantsFlat;
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
