﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Civ2engine;

namespace WinFormsUI
{
    public partial class _ForeignCreateUnitForm : Civ2form
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        DoubleBufferedPanel MainPanel;
        RadioButton[] RadioButton;// = new RadioButton[Game.CivsInPlay.Sum(x => x ? 1 : 0)];
        int ChosenUnit { get; }
        bool IsVeteran { get; }
        int[] ActiveCivId;// = new int[Game.CivsInPlay.Sum()];

        public _ForeignCreateUnitForm(int chosenUnit, bool isVeteran)
        {
            InitializeComponent();
            ChosenUnit = chosenUnit;
            IsVeteran = IsVeteran;

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(668, Game.CivsInPlay.Sum(x => x ? 1 : 0) * 32 + 4),
                BackgroundImage = Images.PanelOuterWallpaper
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, Game.CivsInPlay.Sum(x => x ? 1 : 0) * 32 + 42),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(344, Game.CivsInPlay.Sum(x => x ? 1 : 0) * 32 + 42),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            //Radio buttons
            int count = 0;
            for (int civ = 0; civ < 8; civ++)
            {
                if (Game.CivsInPlay[civ])
                {
                    //Make a radio button
                    RadioButton[count] = new RadioButton
                    {
                        Text = Game.GetCivs[civ].TribeName,
                        Location = new Point(40, 2 + 32 * count),
                        BackColor = Color.Transparent,
                        Font = new Font("Times New Roman", 18.0f),
                        ForeColor = Color.FromArgb(51, 51, 51),
                        AutoSize = true
                    };
                    MainPanel.Controls.Add(RadioButton[count]);

                    //Also get indexes of active civs
                    ActiveCivId[count] = civ;

                    count++;
                }
            }
            RadioButton[0].Checked = true;  //set initial value
        }

        private void ForeignCreateUnitForm_Load(object sender, EventArgs e) { }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Game.CivsInPlay.Sum(x => x ? 1 : 0); i++)
            {
                if (RadioButton[i].Checked)
                {
                    //unit should be placed on activebox coords (convert it from civ2-style)
                    //int x = (MapForm.ActiveBoxX - MapForm.ActiveBoxY % 2) / 2; 
                    //int y = MapForm.ActiveBoxY;

                    //Game.CreateUnit((UnitType)ChosenUnit, x, y, false, true, false, IsVeteran, ActiveCivId[i], 0, 0, 0, 0, OrderType.NoOrders, 0, 0, 0, 0, 0);
                    //Application.OpenForms.OfType<MapForm>().First().RefreshMapForm();
                    //Application.OpenForms.OfType<StatusForm>().First().RefreshStatusForm();
                    Close();

                    break;
                }
            }
        }

        //if cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e) { Close(); }
    }
}
