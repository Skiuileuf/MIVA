using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MIVA.TUI;

namespace MIVA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TUI ui = new TUI("MIVA Sharks", 160, 50);

            //ui.Init("MIVA Sharks");

            MenuNode menu = new MenuNode("MIVA Sharks", "Calculator diversi factori", null, 
                new MenuNode("Productie eficienta", "Calculeaza productia cea mai eficienta (pe baza resurselor disponibile", () =>
                {
                    EfficientProduction.Run();
                }),
                new MenuNode("Aprovizionare eficienta", "Aprovizionarea maxima pe baza orelor de lucru disponibile", () =>
                {
                    MaximizeRobotUsage.Run();
                })
            );

            ui.Run(menu);

            Console.ReadKey();

        }

        
    }

    

    
}
