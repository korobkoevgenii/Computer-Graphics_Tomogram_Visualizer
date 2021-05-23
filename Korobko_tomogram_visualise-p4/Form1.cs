using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Korobko_tomogram_visualise_p4
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        int currentLayer = 0;
        
        public Form1()
        {
            
            InitializeComponent();
            Load += new System.EventHandler(Form1_Load);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.Z-1;
                
                loaded = true;
                glControl1.Invalidate();
               
            }
        }
        bool needReload = false;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                if (radioButton1.Checked)
                    view.DrawQuads(currentLayer);
                else
                {
                    if (radioButton2.Checked)
                    {
                        if (needReload)
                        {

                            view.generateTextureImage(currentLayer);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                    }
                    if (radioButton3.Checked)
                    {
                        view.DrawQuadStrip(currentLayer);
                    }
                    
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
                
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            Application.Idle += Application_Idle;
        }
        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            view.minimum = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            view.window = trackBar3.Value;
            needReload = true;
        }
    }
}
