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

        private void Main_Load(object sender, EventArgs e)
        {
            cmbScoreAlgo.Items.Clear();
            foreach(var v in Enum.GetNames(typeof(Score.AlgorithmType)))
            {
                cmbScoreAlgo.Items.Add(v);
            }
            cmbScoreAlgo.SelectedIndex = 0;

            cmbScoreVariant.Items.Clear();
            foreach (var v in Enum.GetNames(typeof(Score.VariantType)))
            {
                cmbScoreVariant.Items.Add(v);
            }
            cmbScoreVariant.SelectedIndex = 0;
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            try
            {
                var defaultGridView = (dataGridView.DataSource as DataTable).DefaultView;
                defaultGridView.RowFilter = txtFilter.Text;
                lblResultsCount.Text = $"{defaultGridView.Count} Production Variants";
            } catch (Exception ex)
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

        private double CalculateScore(Knapsack knapsack)
        {
            if (cmbScoreAlgo.SelectedIndex == (int)Score.AlgorithmType.Old)
            {
                return  (double)numPricePech.Value * (double)numDemandPech.Value * knapsack.Pech * 1 +
                        (double)numPriceProm.Value * (double)numDemandProm.Value * knapsack.Prom * 1 +
                        (double)numPriceStan.Value * (double)numDemandStan.Value * knapsack.Stan * 0.5;

            }
            else
            {
                return Score.CalculateTotal(
                    knapsack,
                    (double)marketCapPech.Value,
                    (double)marketCapProm.Value,
                    (double)marketCapStan.Value,
                    (double)numDemandPech.Value,
                    (double)numDemandProm.Value,
                    (double)numDemandStan.Value,
                    (Score.AlgorithmType)cmbScoreAlgo.SelectedIndex,
                    (Score.VariantType)cmbScoreVariant.SelectedIndex);
            }
        }

        private void runEfficientProduction_Click(object sender, EventArgs e)
        {
            EfficientProduction ep = new EfficientProduction();

            //ep.SetInitialStock(40, 20, 6, 7);
            //ep.SetDemandMultipliers(0.8, 0.7, 0.8);
            //ep.SetSalePrices(6000, 10000, 30000);

            ep.SetInitialStock(
                (double)numStockPlastic.Value,
                (double)numStockWood.Value,
                (double)numStockPlasticRobots.Value,
                (double)numStockWoodRobots.Value );

            ep.SetDemandMultipliers(
                (double)numDemandPech.Value,
                (double)numDemandProm.Value,
                (double)numDemandStan.Value);

            ep.SetSalePrices(
                (double)numPricePech.Value,
                (double)numPriceProm.Value,
                (double)numPriceStan.Value);

            var results = ep.Run();

            DataTable table = new DataTable();
            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Score", typeof(double) ),
                new DataColumn("Pech", typeof(double)),
                new DataColumn("Prom", typeof(double)),
                new DataColumn("Stan", typeof(double)),
                new DataColumn("Delta.Plastic", typeof(double)),
                new DataColumn("Delta.Wood", typeof(double)),
                new DataColumn("Delta.PlasticWorkshopHours", typeof(double)),
                new DataColumn("Delta.WoodWorkshopHours", typeof(double)),

            });

            foreach (var result in results)
            {
                object[] values = new object[8];

                //values[0] = result.Score;
                values[0] = CalculateScore(result);
                values[1] = result.Pech;
                values[2] = result.Prom;
                values[3] = result.Stan;

                values[4] = Math.Round(result.Delta.Plastic, 2);
                values[5] = Math.Round(result.Delta.Wood, 2);
                values[6] = Math.Round(result.Delta.PlasticWorkshopHours, 2);
                values[7] = Math.Round(result.Delta.WoodWorkshopHours, 2);

                table.Rows.Add(values);
            }
            
            dataGridView.DataSource = table;

            var defaultGridView = (dataGridView.DataSource as DataTable).DefaultView;
            lblResultsCount.Text = $"{defaultGridView.Count} Production Variants";
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
    }
}
