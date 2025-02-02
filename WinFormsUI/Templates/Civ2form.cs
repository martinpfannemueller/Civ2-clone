﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsUI
{
    public partial class Civ2form : Form
    {
        public Civ2form()
        {
            InitializeComponent();
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            BackgroundImage = Images.PanelOuterWallpaper;
            this.Paint += new PaintEventHandler(Civ2form_Paint);
        }

        private void Civ2form_Load(object sender, EventArgs e) { }

        private void Civ2form_Paint(object sender, PaintEventArgs e)
        {
            //Draw border around form
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, this.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, this.Width - 4, 2);   //3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, this.Width - 5, 3);   //4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, this.Width - 6, 4);   //5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, this.Height - 5, this.Width - 5, this.Height - 5);
        }
    }
}
