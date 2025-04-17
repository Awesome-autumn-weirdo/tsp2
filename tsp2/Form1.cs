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
        private double averageArrivalTime = 1;
        private double virtualTimeMinutes = 0.0;
        private double mean = 10;
        private double stdDev = 4;

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
            // Проверка средней частоты прибытия
            if (averageArrivalTime <= 0)
            {
                MessageBox.Show("Среднее время прибытия (averageArrivalTime) должно быть положительным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Проверки параметров нормального распределения
            if (mean < 0)
            {
                MessageBox.Show("Среднее количество вагонов (mean) не может быть отрицательным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            if (stdDev <= 0)
            {
                MessageBox.Show("Стандартное отклонение (stdDev) должно быть положительным!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            trainCount = 0;
            virtualTimeMinutes = 0;
            wagonCounts.Clear();
            //listBox1.Items.Clear();

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
            double r1 = 1.0 - random.NextDouble();
            double r2 = 1.0 - random.NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(r1)) * Math.Sin(2.0 * Math.PI * r2);
            int wagonCount = (int)Math.Round(mean + stdDev * z);
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
            GenerateTrains(5);
            GenerateTrains(10);
            GenerateTrains(15);
        }
    }
}
