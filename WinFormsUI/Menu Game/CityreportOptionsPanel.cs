﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Civ2engine;

namespace WinFormsUI
{
    public class CityreportOptionsPanel : Civ2panel
    {
        private Game Game => Game.Instance;

        private readonly Main _main;
        private readonly List<DoubleBufferedPanel> _clickPanels;
        private readonly string[] _textOptions;
        private readonly bool[] _choiceOptions;

        public CityreportOptionsPanel(Main parent, int _width, int _height) : base(_width, _height, "Select City Report Options", 38, 46)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            // OK button
            var _OKButton = new Civ2button
            {
                Location = new Point(9, 398),
                Size = new Size(363, 36),
                Text = "OK"
            };
            Controls.Add(_OKButton);
            _OKButton.Click += OKButton_Click;

            // Cancel button
            var _cancelButton = new Civ2button
            {
                Location = new Point(374, 398),
                Size = new Size(363, 36),
                Text = "Cancel"
            };
            Controls.Add(_cancelButton);
            _cancelButton.Click += CancelButton_Click;

            // Make an options array
            _choiceOptions = new bool[11] { Game.Options.WarnWhenCityGrowthHalted, Game.Options.ShowCityImprovementsBuilt, Game.Options.ShowNonCombatUnitsBuilt, Game.Options.ShowInvalidBuildInstructions, Game.Options.AnnounceCitiesInDisorder, Game.Options.AnnounceOrderRestored, Game.Options.AnnounceWeLoveKingDay, Game.Options.WarnWhenFoodDangerouslyLow, Game.Options.WarnWhenPollutionOccurs, Game.Options.WarnChangProductWillCostShields, Game.Options.ZoomToCityNotDefaultAction};
            // Write here individual options
            _textOptions = new string[11] {
                "Warn when city growth halted (Aqueduct/Sewer System).",
                "Show city improvements built.",
                "Show non-combat units built.",
                "Show invalid build instructions.",
                "Announce cities in disorder.",
                "Announce order restored in city.",
                "Announce \"We Love The King Day\".",
                "Warn when food dangerously low.",
                "Warn when new pollution occurs.",
                "Warn when changing production will cost shields.",
                "\"Zoom-to-City\" NOT default action." };
            // Make click panels for each options
            _clickPanels = new List<DoubleBufferedPanel>();
            for (int i = 0; i < 11; i++)
            {
                var panel = new DoubleBufferedPanel
                {
                    Location = new Point(10, (32 * i) + 4),
                    Size = new Size(100, 27),   // You will set the correct width once you measure text size below
                    BackColor = Color.Transparent
                };
                DrawPanel.Controls.Add(panel);
                panel.Click += ClickPanels_Click;
                _clickPanels.Add(panel);
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Show all options
            var stringSize = new SizeF[11];
            using var _font1 = new Font("Times New Roman", 18);
            using var _font2 = new Font("Wingdings", 18);
            using var _brush1 = new SolidBrush(Color.FromArgb(51, 51, 51));
            using var _brush2 = new SolidBrush(Color.White);
            using var _brush3 = new SolidBrush(Color.FromArgb(128, 128, 128));
            using var _brush4 = new SolidBrush(Color.Black);
            using var _brush5 = new SolidBrush(Color.FromArgb(192, 192, 192));
            using var _pen = new Pen(Color.Black);
            for (int row = 0; row < 11; row++)
            {
                // Text
                e.Graphics.DrawString(_textOptions[row], _font1, _brush1, new Point(36, 32 * row + 4));
                stringSize[row] = e.Graphics.MeasureString(_textOptions[row], _font1);  // Measure size of text
                _clickPanels[row].Size = new Size(30 + (int)(stringSize[row].Width), _clickPanels[row].Height);   // Set the correct size of click panel

                // Draw checkbox
                e.Graphics.FillRectangle(_brush2, new Rectangle(13, 8 + 32 * row, 15, 17));
                e.Graphics.FillRectangle(_brush2, new Rectangle(12, 9 + 32 * row, 17, 15));
                e.Graphics.FillRectangle(_brush3, new Rectangle(14, 9 + 32 * row, 13, 15));
                e.Graphics.FillRectangle(_brush3, new Rectangle(13, 10 + 32 * row, 15, 13));
                e.Graphics.DrawLine(_pen, 14, 9 + 32 * row, 26, 9 + 32 * row);
                e.Graphics.DrawLine(_pen, 14, 25 + 32 * row, 27, 25 + 32 * row);
                e.Graphics.DrawLine(_pen, 13, 10 + 32 * row, 13, 22 + 32 * row);
                e.Graphics.DrawLine(_pen, 29, 10 + 32 * row, 29, 24 + 32 * row);
                e.Graphics.DrawLine(_pen, 13, 10 + 32 * row, 14, 10 + 32 * row);
                e.Graphics.DrawLine(_pen, 28, 24 + 32 * row, 28, 25 + 32 * row);

                // Draw check marks
                if (_choiceOptions[row])
                {
                    e.Graphics.DrawString("ü", _font2, _brush4, new Point(10 + 1, 32 * row + 3 + 2));
                    e.Graphics.DrawString("ü", _font2, _brush5, new Point(10, 32 * row + 3));
                }
            }
        }

        // If OK is pressed --> update the options and close
        private void OKButton_Click(object sender, EventArgs e)
        {
            Game.Options.WarnWhenCityGrowthHalted = _choiceOptions[0];
            Game.Options.ShowCityImprovementsBuilt = _choiceOptions[1];
            Game.Options.ShowNonCombatUnitsBuilt = _choiceOptions[2];
            Game.Options.ShowInvalidBuildInstructions = _choiceOptions[3];
            Game.Options.AnnounceCitiesInDisorder = _choiceOptions[4];
            Game.Options.AnnounceOrderRestored = _choiceOptions[5];
            Game.Options.AnnounceWeLoveKingDay = _choiceOptions[6];
            Game.Options.WarnWhenFoodDangerouslyLow = _choiceOptions[7];
            Game.Options.WarnWhenPollutionOccurs = _choiceOptions[8];
            Game.Options.WarnChangProductWillCostShields = _choiceOptions[9];
            Game.Options.ZoomToCityNotDefaultAction = _choiceOptions[10];
            this.Visible = false;
            this.Dispose();
        }

        // If cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        // When an option is clicked, update the bool array of options
        private void ClickPanels_Click(object sender, EventArgs e)
        {
            Control clickedControl = (Control)sender;   // Sender gives you which control is clicked.
            int index = _clickPanels.FindIndex(a => a == clickedControl);    // Get which control is clicked in a list
            _choiceOptions[index] = !_choiceOptions[index];
            DrawPanel.Invalidate();
        }
    }
}
