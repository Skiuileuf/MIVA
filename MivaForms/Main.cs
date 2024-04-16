using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastMember;
using LibMIVA;
using LibMIVA.Algorithm;


namespace MivaForms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public ValuesTuple<double> Prices;
        public ValuesTuple<double> Demand;
        public ValuesTuple<double> BaseMarket;

        public Score.AlgorithmType AlgorithmType;
        public Score.VariantType VariantType;

        private void Main_Load(object sender, EventArgs e)
        {
            cmbScoreAlgo.Items.Clear();
            foreach (var v in Enum.GetNames(typeof(Score.AlgorithmType)))
            {
                cmbScoreAlgo.Items.Add(v);
            }
            cmbScoreAlgo.SelectedIndex = (int)Score.AlgorithmType.Parabola;

            cmbScoreVariant.Items.Clear();
            foreach (var v in Enum.GetNames(typeof(Score.VariantType)))
            {
                cmbScoreVariant.Items.Add(v);
            }
            cmbScoreVariant.SelectedIndex = (int)Score.VariantType.Normal;

            calculateActualMarketCapacity();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultGridView = (dataGridView.DataSource as DataTable).DefaultView;
                defaultGridView.RowFilter = txtFilter.Text;
                lblResultsCount.Text = $"{defaultGridView.Count} Production Variants";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnApplySort_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultGridView = (dataGridView.DataSource as DataTable).DefaultView;
                defaultGridView.Sort = txtSort.Text;
                lblResultsCount.Text = lblResultsCount.Text = $"{defaultGridView.Count} Production Variants";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lnkFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.csharp-examples.net/dataview-rowfilter/");
        }

        private void GrabValuesFromForm()
        {
            try
            {
                Prices = new ValuesTuple<double>((double)numPricePech.Value, (double)numPriceProm.Value, (double)numPriceStan.Value);
                Demand = new ValuesTuple<double>((double)numDemandPech.Value, (double)numDemandProm.Value, (double)numDemandStan.Value);
                BaseMarket = new ValuesTuple<double>((double)numBasePech.Value, (double)numBaseProm.Value, (double)numBaseStan.Value);

                AlgorithmType = (Score.AlgorithmType)cmbScoreAlgo.SelectedIndex;
                VariantType = (Score.VariantType)cmbScoreVariant.SelectedIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double CalculateScore(Knapsack knapsack, Score.AlgorithmType algorithmType, Score.VariantType variantType)
        {
            var production = new ValuesTuple<double>(knapsack.Pech, knapsack.Prom, knapsack.Stan);
            var oldAlgoWeights = new ValuesTuple<double>(1, 1, 0.5);

            if (algorithmType == Score.AlgorithmType.Old)
            {
                return (production * Prices * oldAlgoWeights).Sum();
            }
            else
            {
                return Score.CalculateTotal(
                    knapsack,
                    BaseMarket,
                    Demand,
                    Prices,
                    algorithmType,
                    variantType
                );
            }
        }

        private void calculateActualMarketCapacity()
        {
            lblPechCap.Text = $"{Math.Floor((double)numBasePech.Value * (double)numDemandPech.Value)}";
            lblPromCap.Text = $"{Math.Floor((double)numBaseProm.Value * (double)numDemandProm.Value)}";
            lblStanCap.Text = $"{Math.Floor((double)numBaseStan.Value * (double)numDemandStan.Value)}";
        }

        private void runEfficientProductionAndDisplayData()
        {
            EfficientProduction ep = new EfficientProduction();

            ep.SetInitialStock(
                (double)numStockPlastic.Value,
                (double)numStockWood.Value,
                (double)numStockPlasticRobots.Value,
                (double)numStockWoodRobots.Value);
            
            ep.SetDemandMultipliers(
                Demand.Pech,
                Demand.Prom,
                Demand.Stan);

            ep.SetSalePrices(
                Prices.Pech,
                Prices.Prom,
                Prices.Stan);

            var results = new List<Knapsack>();

            BackgroundWorker worker = new BackgroundWorker();
            prgVariantGeneration.Value = 100;

            DataTable table = new DataTable();

            worker.DoWork += (s, args) =>
            {
                results = ep.Run();

                table.Columns.AddRange(new DataColumn[] {
                    new DataColumn("Score", typeof(double) ),
                    new DataColumn("Pech", typeof(double)),
                    new DataColumn("Prom", typeof(double)),
                    new DataColumn("Stan", typeof(double)),

                    new DataColumn("D.P", typeof(double)),
                    new DataColumn("D.W", typeof(double)),
                    new DataColumn("D.PWH", typeof(double)),
                    new DataColumn("D.WWH", typeof(double)),

                    new DataColumn("SellPrice", typeof(double)),
                });

                foreach (var result in results)
                {
                    var production = new ValuesTuple<double>(result.Pech, result.Prom, result.Stan);
                    object[] values = new object[9];

                    //values[0] = result.Score;
                    values[0] = CalculateScore(result, AlgorithmType, VariantType);
                    values[1] = result.Pech;
                    values[2] = result.Prom;
                    values[3] = result.Stan;

                    values[4] = Math.Round(result.Delta.Plastic, 2);
                    values[5] = Math.Round(result.Delta.Wood, 2);
                    values[6] = Math.Round(result.Delta.PlasticWorkshopHours, 2);
                    values[7] = Math.Round(result.Delta.WoodWorkshopHours, 2);

                    values[8] = Math.Round((production * Prices).Sum(), 2);

                    table.Rows.Add(values);
                }

               
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                prgVariantGeneration.Value = 0;

                dataGridView.DataSource = table;


                //dataGridView.Columns.Add(new DataGridViewButtonColumn()
                //{
                //    Text = "Details",
                //    UseColumnTextForButtonValue = true,
                //    Name = "Details",
                //    HeaderText = "Details"
                //});

                var defaultGridView = (dataGridView.DataSource as DataTable).DefaultView;
                lblResultsCount.Text = $"{defaultGridView.Count} Production Variants";
            };

            worker.RunWorkerAsync();

            //var results = ep.Run();


        }

        private void runEfficientProduction_Click(object sender, EventArgs e)
        {
            GrabValuesFromForm();
            //BackgroundWorker worker = new BackgroundWorker();

            //prgVariantGeneration.Value = 0;

            //worker.DoWork += (s, args) =>
            //{
            //    runEfficientProductionAndDisplayData();
            //};
            runEfficientProductionAndDisplayData();
            //worker.RunWorkerCompleted += (s, args) =>
            //{
            //    prgVariantGeneration.Value = 100;
            //};

            //worker.RunWorkerAsync();


        }

        private void btnOpenDemandData_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "/data/demand.csv");
        }

        private void numStockPlastic_ValueChanged(object sender, EventArgs e)
        {
            numStockWood.Value = numStockPlastic.Value / 2;
        }

        private void btnTestAllScoreFunctions_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            double m = 50;
            double xMax = m * 2;

            sb.Append("Func,");
            for (int x = 0; x < xMax; x++)
            {
                sb.Append($"{x},");
            }
            sb.AppendLine();


            foreach (Score.AlgorithmType algo in Enum.GetValues(typeof(Score.AlgorithmType)))
            {
                foreach (Score.VariantType type in Enum.GetValues(typeof(Score.VariantType)))
                {
                    var func = Score.GetScoreFunction(algo, type);

                    sb.Append($"{algo}-{type},");
                    for (int x = 0; x < xMax; x++)
                    {
                        double s = func(x, m);
                        sb.Append($"{s},");
                    }

                    sb.AppendLine();
                }
            }

            File.WriteAllText("data/scoreFunctions.txt", sb.ToString());

            MessageBox.Show("Done");
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //FormSerializer.Serialise(this, Application.StartupPath + @"\serialise.xml");
        }

        private void demandOrCapacity_valueChanged(object sender, EventArgs e)
        {
            calculateActualMarketCapacity();
        }

        private void btnProductionCost_Click(object sender, EventArgs e)
        {
            var row = dataGridView.SelectedRows[0];
            double pech = (double)row.Cells["Pech"].Value;
            double prom = (double)row.Cells["Prom"].Value;
            double stan = (double)row.Cells["Stan"].Value;

            double wood = pech * Product.Pech.Wood + prom * Product.Prom.Wood + stan * Product.Stan.Wood;
            double plastic = pech * Product.Pech.Plastic + prom * Product.Prom.Plastic + stan * Product.Stan.Plastic;
            double woodWH = pech * Product.Pech.WoodWorkshopHours + prom * Product.Prom.WoodWorkshopHours + stan * Product.Stan.WoodWorkshopHours;
            double plasticWH = pech * Product.Pech.PlasticWorkshopHours + prom * Product.Prom.PlasticWorkshopHours + stan * Product.Stan.PlasticWorkshopHours;

            MessageBox.Show($"Plastic: {plastic}\nWood: {wood}\nPlasticWH: {plasticWH}\nWoodWH: {woodWH}\n");
        }

        private void addDynamicColumn_Click(object sender, EventArgs e)
        {
            string columnName = "Dynamic Column";
            string columnFormula = "Pech + Prom + Stan";

            Utils.ShowInputDialog(ref columnName);
            Utils.ShowInputDialog(ref columnFormula);

            var table = dataGridView.DataSource as DataTable;
            table.Columns.Add(new DataColumn(columnName, typeof(double), columnFormula));
        }

        private void btnMaximizeStock_Click(object sender, EventArgs e)
        {
            double plasticWorkHours = (double)numStockPlasticRobots.Value * 200;
            double woodWorkHours = (double)numStockWoodRobots.Value * 200;

            double pechPlastic = plasticWorkHours / Product.Pech.PlasticWorkshopHours;
            double promPlastic = plasticWorkHours / Product.Prom.PlasticWorkshopHours;
            double stanPlastic = plasticWorkHours / Product.Stan.PlasticWorkshopHours;

            double pechWood = woodWorkHours / Product.Pech.WoodWorkshopHours;
            double promWood = woodWorkHours / Product.Prom.WoodWorkshopHours;
            double stanWood = woodWorkHours / Product.Stan.WoodWorkshopHours;

            double maxPlastic = Math.Max(stanWood, Math.Max(promWood, pechWood));
            double maxWood = Math.Max(stanPlastic, Math.Max(promPlastic, pechPlastic));

            numStockPlastic.Value = (decimal)maxPlastic;
            numStockWood.Value = (decimal)maxWood;

        }
    }
}
