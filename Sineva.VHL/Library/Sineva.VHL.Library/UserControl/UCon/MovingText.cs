using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class MovingText : UserControl
    {
        Graphics g;
        Brush b;
        int x = 0;
        Color m_TextColor = Color.Black;
        #region Properties
        public Color TextColor 
        {
            get { return m_TextColor; }
            set
            {
                m_TextColor = value;
                b.Dispose();
                b = new SolidBrush(value);
            }
        }
        public Color BgColor 
        {
            get { return this.panel1.BackColor; }
            set 
            {
                this.BackColor = value; 
                this.panel1.BackColor = value; 
            } 
        }
        public Font TextFont { get; set; }
        public string Message { get; set; }
        #endregion
        public MovingText()
        {
            InitializeComponent();
            TextFont = new System.Drawing.Font("Arial", 9F);
            g = panel1.CreateGraphics();
            b = new SolidBrush(TextColor);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Refresh();
            g.DrawString(Message, TextFont, b, x, panel1.Height/4);
            if (x > panel1.Width) x=0;
            g.DrawString(Message, TextFont, b, x - panel1.Width, panel1.Height / 4);
            x++;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            this.OnMouseHover(e);
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }
    }
}