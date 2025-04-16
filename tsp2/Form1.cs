using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace tsp2
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private List<int> wagonCounts = new List<int>();
        private int trainCount = 0;
        private double averageArrivalTime = 10.0;
        private double virtualTimeMinutes = 0.0;

        public Form1()
        {
            InitializeComponent();
            InitializeChart();

            buttonStartStop.Text = "Пуск";
        }

        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            ChartArea chartArea = new ChartArea();
            chart1.ChartAreas.Add(chartArea);

            Series histogramSeries = new Series
            {
                Name = "Распределение",
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.Color.Plum
            };

            chart1.Series.Add(histogramSeries);
        }

        private void GenerateTrains(int numberOfTrains)
        {
            trainCount = 0;
            virtualTimeMinutes = 0;
            wagonCounts.Clear();
            listBox1.Items.Clear();

            for (int i = 0; i < numberOfTrains; i++)
            {
                double interval = -averageArrivalTime * Math.Log(1.0 - random.NextDouble());
                virtualTimeMinutes += interval;

                int numWagons = GenerateWagonCount();
                trainCount++;
                wagonCounts.Add(numWagons);

                TimeSpan virtualTime = TimeSpan.FromMinutes(virtualTimeMinutes);
                string formattedTime = $"{virtualTime:hh\\:mm\\:ss}";
                string trainInfo = $"[{formattedTime}] Поезд {trainCount} прибыл. Количество вагонов: {numWagons}";
                listBox1.Items.Add(trainInfo);
            }

            UpdateHistogram();
        }

        private int GenerateWagonCount()
        {
            double mean = 10;
            double stdDev = 4;
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            int wagonCount = (int)Math.Round(mean + stdDev * randStdNormal);
            return Math.Max(1, wagonCount);
        }

        private void UpdateHistogram()
        {
            chart1.Series["Распределение"].Points.Clear();

            Dictionary<int, int> wagonFrequency = new Dictionary<int, int>();

            foreach (int count in wagonCounts)
            {
                if (wagonFrequency.ContainsKey(count))
                    wagonFrequency[count]++;
                else
                    wagonFrequency[count] = 1;
            }

            foreach (var pair in wagonFrequency)
            {
                chart1.Series["Распределение"].Points.AddXY(pair.Key, pair.Value);
            }
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            GenerateTrains(100);
        }
    }
}
