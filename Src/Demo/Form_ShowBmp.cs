using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form_ShowBmp : Form
    {
        Bitmap m_bmp;
        public Form_ShowBmp()
        {
            InitializeComponent();
        }
        public Form_ShowBmp(Bitmap bmp)
        {
            m_bmp = bmp;
            InitializeComponent();
        }
        private void Form_ShowBmp_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = m_bmp;
        }
    }
}
