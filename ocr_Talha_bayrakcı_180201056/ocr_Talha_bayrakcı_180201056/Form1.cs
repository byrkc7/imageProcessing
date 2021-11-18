using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ocr_Talha_bayrakcı_180201056
{
    public partial class Form1 : Form
    {
        Bitmap IMG_Learn;
        Bitmap IMG_Compare;

        int[] Letter_F_V_Proj;
        int[] Letter_F_H_Proj;

        int[] Letter_V_Proj;//RECOGNATİON
        int[] Letter_H_Proj;

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string F_Name;
            openFileDialog1.ShowDialog();
            F_Name = openFileDialog1.FileName;
            IMG_Learn = new Bitmap(F_Name);
            pictureBox1.Image = IMG_Learn;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Letter_F_H_Proj = new int[IMG_Learn.Height];
            Letter_F_V_Proj = new int[IMG_Learn.Width];

            for (int y = 0; y < IMG_Learn.Height; y++)
                Letter_F_H_Proj[y] = 0;//buraya bak

            for (int x = 0; x < IMG_Learn.Width; x++)
                Letter_F_V_Proj[x] = 0;

            for (int y = 0; y < IMG_Learn.Height; y++)
                for (int x = 0; x < IMG_Learn.Width; x++)
                    if (IMG_Learn.GetPixel(x, y).R < 10)
                    {
                        Letter_F_H_Proj[y]++;
                    }


            for (int x = 0; x < IMG_Learn.Width; x++)
                for (int y = 0; y < IMG_Learn.Height; y++)
                    if (IMG_Learn.GetPixel(x, y).R < 10)
                    {
                        Letter_F_V_Proj[x]++;
                    }
            //Shiftting H
            int Pivot;
            for (Pivot = 0; Pivot < IMG_Learn.Height; Pivot++)
                if (Letter_F_H_Proj[Pivot] > 0) break;


            for (int y = 0; y < IMG_Learn.Height - Pivot; y++)
            {
                Letter_F_H_Proj[y] = Letter_F_H_Proj[y + Pivot];
                Letter_F_H_Proj[y + Pivot] = 0;
            }




            //Shiftting Vertical
            for (Pivot = 0; Pivot < IMG_Learn.Width; Pivot++)
                if (Letter_F_V_Proj[Pivot] > 0) break;


            for (int x = 0; x < IMG_Learn.Width - Pivot; x++)
            {
                Letter_F_V_Proj[x] = Letter_F_V_Proj[x + Pivot];
                Letter_F_V_Proj[x + Pivot] = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < IMG_Compare.Width; x++)
                for (int y = 0; y < IMG_Compare.Height; y++)
                {
                    int RR = IMG_Compare.GetPixel(x, y).R;
                    int GG = IMG_Compare.GetPixel(x, y).G;
                    int BB = IMG_Compare.GetPixel(x, y).B;

                    int GRY = (int)(0.3 * RR + 0.58 * GG + 0.12 * BB);
                    IMG_Compare.SetPixel(x, y, Color.FromArgb(GRY, GRY, GRY));
                }

            //Generate Histogram
            int[] Hisot = new int[256];

            for (int i = 0; i < 256; i++)
            {
                Hisot[i] = 0;
            }
            for (int x = 0; x < IMG_Compare.Width; x++)
            {
                for (int y = 0; y < IMG_Compare.Height; y++)
                {
                    Hisot[IMG_Compare.GetPixel(x, y).R]++;
                }
            }

            int Peak1_Level = 64;
            int Peak1_Val = 0;
            for (int i = 64; i < 128; i++)
                if (Hisot[i] > Peak1_Val)
                {
                    Peak1_Level = i;
                    Peak1_Val = Hisot[i];
                }

            int Peak2_Level = 128;
            int Peak2_Val = 0;
            for (int i = 128; i < 192; i++)
                if (Hisot[i] > Peak2_Val)
                {
                    Peak2_Level = i;
                    Peak2_Val = Hisot[i];
                }

            int Th_Level = (Peak1_Level + Peak2_Level) / 2; //Average


            Letter_H_Proj = new int[IMG_Compare.Height];
            Letter_V_Proj = new int[IMG_Compare.Width];

            for (int y = 0; y < IMG_Compare.Height; y++)
                Letter_H_Proj[y] = 0;//buraya bak

            for (int x = 0; x < IMG_Compare.Width; x++)
                Letter_V_Proj[x] = 0;

            for (int y = 0; y < IMG_Compare.Height; y++)
                for (int x = 0; x < IMG_Compare.Width; x++)
                    if (IMG_Compare.GetPixel(x, y).R < Th_Level)
                    {
                        Letter_H_Proj[y]++;
                    }
            for (int x = 0; x < IMG_Compare.Width; x++)
                for (int y = 0; y < IMG_Compare.Height; y++)
                    if (IMG_Compare.GetPixel(x, y).R < Th_Level)
                    {
                        Letter_V_Proj[x]++;
                    }
            //Shiftting H
            int Pivot;
            for (Pivot = 0; Pivot < IMG_Compare.Height; Pivot++)
                if (Letter_H_Proj[Pivot] > 0) break;


            for (int y = 0; y < IMG_Compare.Height - Pivot; y++)
            {
                Letter_H_Proj[y] = Letter_H_Proj[y + Pivot];
                Letter_H_Proj[y + Pivot] = 0;
            }

            //Shiftting Vertical
            for (Pivot = 0; Pivot < IMG_Compare.Width; Pivot++)
                if (Letter_V_Proj[Pivot] > 0) break;

            for (int x = 0; x < IMG_Compare.Width - Pivot; x++)
            {
                Letter_V_Proj[x] = Letter_V_Proj[x + Pivot];
                Letter_V_Proj[x + Pivot] = 0;
            }
            //Find Differences
            int V_Diff = 0;
            int H_Diff = 0;
            for (int x = 0; x < IMG_Compare.Width; x++)
                V_Diff += Math.Abs(Letter_F_V_Proj[x] - Letter_V_Proj[x]);

            for (int y = 0; y < IMG_Compare.Height; y++)
                H_Diff += Math.Abs(Letter_F_H_Proj[y] - Letter_H_Proj[y]);
            textBox1.Text = V_Diff.ToString();
            textBox2.Text = H_Diff.ToString();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            String F_Name;
            openFileDialog1.ShowDialog();
            F_Name = openFileDialog1.FileName;
            IMG_Compare = new Bitmap(F_Name);
            pictureBox2.Image = IMG_Compare;
        }
    }

}
    

