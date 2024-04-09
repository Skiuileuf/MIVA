using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibMIVA
{
    public class Product
    {
        public double Plastic;
        public double PlasticWorkshopHours;
        public double Wood;
        public double WoodWorkshopHours;
        public string name;

        public Product(double p, double pwh, double w, double wwh, string name = "Unknown")
        {
            this.Plastic = p;
            this.PlasticWorkshopHours = pwh;
            this.Wood = w;
            this.WoodWorkshopHours = wwh;
            this.name = name;
        }

        //Pech4:
        //Plastic 0.2
        //WorkshopPlastic 8
        //Wood 0.1
        //WorkshopWood 10
        public static Product Pech = new Product(0.2, 8, 0.1, 10, "Pech4");

        //Prom5
        //Plastic 0.4
        //WorkshopPlastic 12
        //Wood 0.2
        //WorkshopWood 14
        public static Product Prom = new Product(0.4, 12, 0.2, 14, "Prom5.5");

        //Stan8
        //Plastic 1 
        //WorkshopPlastic 28
        //Wood 0.5
        //WorkshopWood 28
        public static Product Stan = new Product(1, 28, 0.5, 28, "Stan8");

        public override string ToString()
        {
            return $"P: {Plastic:0.00} t; PWH: {PlasticWorkshopHours:0.00} ; W: {Wood:0.00} ; WWH: {WoodWorkshopHours:0.00}  ";
        }
    }
}
