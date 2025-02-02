﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using Civ2engine;
using Civ2engine.Units;
using Civ2engine.Events;
using Civ2engine.Enums;

namespace WinFormsUI
{
    public class StatusPanel : DoubleBufferedPanel
    {
        private Game Game => Game.Instance;
        private Map Map => Map.Instance;

        private readonly Main _main;
        private readonly DoubleBufferedPanel StatsPanel, UnitPanel;
        private readonly Timer Timer = new Timer();
        private bool WaitingAtEndOfTurn { get; set; }
        public static event EventHandler<MapEventArgs> OnMapEvent;

        public StatusPanel(Main parent, int _width, int _height)
        {
            _main = parent;

            BackgroundImage = Images.PanelOuterWallpaper;
            Size = new Size(_width, _height);
            Paint += StatusPanel_Paint;
            MapPanel.OnMapEvent += MapEventHappened;
            Main.OnMapEvent += MapEventHappened;
            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnPlayerEvent += PlayerEventHappened;
            Game.OnUnitEvent += UnitEventHappened;

            StatsPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 38),
                Size = new Size(240, 60),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(StatsPanel);
            StatsPanel.Paint += StatsPanel_Paint;
            StatsPanel.MouseClick += Panel_Click;

            UnitPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 106),
                Size = new Size(240, Height - 117),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(UnitPanel);
            UnitPanel.Paint += UnitPanel_Paint;
            UnitPanel.MouseClick += Panel_Click;

            // Timer for "end of turn" message
            Timer.Tick += Timer_Tick;
            Timer.Interval = 500;   // ms
        }

        private void StatusPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            using var _font = new Font("Times New Roman", 17, FontStyle.Bold);
            Draw.Text(e.Graphics, "Status", _font, StringAlignment.Center, StringAlignment.Center, Color.FromArgb(135, 135, 135), new Point(this.Width / 2, 20), Color.Black, 1, 1);
            // Outer border
            using var _pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var _pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var _pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var _pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var _pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var _pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var _pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(_pen1, 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(_pen1, 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(_pen2, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen2, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen3, 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(_pen3, 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(_pen4, this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen4, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen5, 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(_pen5, 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(_pen5, this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen5, 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen6, 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(_pen6, 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(_pen7, this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen7, 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen6, 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(_pen6, 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(_pen7, this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(_pen7, 4, this.Height - 5, this.Width - 5, this.Height - 5);
            // Draw line borders of stats panel
            e.Graphics.DrawLine(_pen7, 9, 36, 252, 36);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 36, 9, 98);
            e.Graphics.DrawLine(_pen6, 9, 99, 252, 99);
            e.Graphics.DrawLine(_pen6, 252, 36, 252, 99);
            e.Graphics.DrawLine(_pen7, 10, 37, 250, 37);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 38, 10, 97);
            e.Graphics.DrawLine(_pen6, 10, 98, 251, 98);
            e.Graphics.DrawLine(_pen6, 251, 37, 251, 98);
            // Draw line borders of unit panel
            e.Graphics.DrawLine(_pen7, 9, 104, 252, 104);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 104, 9, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(_pen6, 9, 107 + UnitPanel.Height, 252, 107 + UnitPanel.Height);
            e.Graphics.DrawLine(_pen6, 252, 104, 252, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(_pen7, 9, 105, 250, 105);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 104, 10, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(_pen6, 10, 106 + UnitPanel.Height, 252, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(_pen6, 251, 105, 251, 105 + UnitPanel.Height);
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            using var _font = new Font("Times New Roman", 12, FontStyle.Bold);
            string showYear = (Game.GameYear < 0) ? $"{Math.Abs(Game.GameYear)} B.C." : $"A.D. {Math.Abs(Game.GameYear)}";
            Draw.Text(e.Graphics, Game.PlayerCiv.Population.ToString("###,###", new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " People", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(5, 2), Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, showYear, _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(5, 20), Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, $"{Game.PlayerCiv.Money} Gold 5.0.5", _font, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(51, 51, 51), new Point(5, 38), Color.FromArgb(191, 191, 191), 1, 1);
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            using var _font = new Font("Times new roman", 12, FontStyle.Bold);
            var _frontColor = Color.FromArgb(51, 51, 51);
            var _backColor = Color.FromArgb(191, 191, 191);
            List<IUnit> _unitsOnThisTile = Game.UnitsHere(Game.ActiveXY[0], Game.ActiveXY[1]);

            string _cityName, _wholeText, _roadText, _irrigText, _airbaseText;
            int _column;

            // View piece mode
            if (_main.ViewPieceMode)
            {
                Draw.Text(e.Graphics, "Viewing Pieces", _font, StringAlignment.Center, StringAlignment.Center, Color.White, new Point(119, 10), Color.Black, 1, 0);

                // Draw location & tile type on active square
                Draw.Text(e.Graphics, $"Loc: ({Game.ActiveXY[0]}, {Game.ActiveXY[1]}) {Map.Tile[(Game.ActiveXY[0] - Game.ActiveXY[1] % 2) / 2, Game.ActiveXY[1]].Island}", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, 27), _backColor, 1, 1);
                Draw.Text(e.Graphics, $"({Map.Tile[(Game.ActiveXY[0] - Game.ActiveXY[1] % 2) / 2, Game.ActiveXY[1]].Type})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, 45), _backColor, 1, 1);

                int count;
                //for (count = 0; count < Math.Min(_unitsOnThisTile.Count, maxUnitsToDraw); count++)
                //{
                //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
                //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), 0), 6, 70 + count * 56);  // TODO: do this again!!!
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].HomeCity.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 70 + count * 56), _backColor, 1, 1);
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Order.ToString(), _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 88 + count * 56), _backColor, 1, 1); // TODO: give proper conversion of orders to string
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 106 + count * 56), _backColor, 1, 1);
                //}
                //if (count < _unitsOnThisTile.Count)
                //{
                //    string _moreUnits = (_unitsOnThisTile.Count - count == 1) ? "More Unit" : "More Units";
                //    Draw.Text(e.Graphics, $"({_unitsOnThisTile.Count() - count} {_moreUnits})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, UnitPanel.Height - 27), _backColor, 1, 1);
                //}
            }
            // Moving units mode
            else
            {
                Draw.Text(e.Graphics, "Moving Units", _font, StringAlignment.Center, StringAlignment.Center, Color.White, new Point(119, 10), Color.Black, 1, 0);

                // Show active unit info
                Draw.Unit(e.Graphics, Game.ActiveUnit, false, 1, new Point(7, 27));
                // Show move points correctly
                int _fullMovPts = Game.ActiveUnit.MovePoints / 3;
                int _remMovPts = Game.ActiveUnit.MovePoints % 3;
                string _text = $"Moves: {_fullMovPts} {_remMovPts}/3";
                if (_remMovPts == 0) _text = $"Moves: {_fullMovPts}";
                Draw.Text(e.Graphics, _text, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 25), _backColor, 1, 1);
                // Show other unit info
                _cityName = (Game.ActiveUnit.HomeCity == null) ? "NONE" : Game.ActiveUnit.HomeCity.Name;
                Draw.Text(e.Graphics, _cityName, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 43), _backColor, 1, 1);
                Draw.Text(e.Graphics, Game.ActiveCiv.Adjective, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 61), _backColor, 1, 1);
                _column = 83;
                Draw.Text(e.Graphics, Game.ActiveUnit.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, _column), _backColor, 1, 1);
                _column += 18;
                Draw.Text(e.Graphics, $"({Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Type})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, _column), _backColor, 1, 1);
                // If road/railroad/irrigation/farmland/mine present
                _wholeText = null;
                _roadText = null;
                _irrigText = null;
                if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Road || Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Railroad || Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Irrigation || Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Farmland || Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Mining)
                {
                    _column += 18;
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Road) _roadText = "Road";
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Railroad) _roadText = "Railroad";
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Irrigation) _irrigText = "Irrigation";
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Farmland) _irrigText = "Farmland";
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Mining) _irrigText = "Mining";
                    if (_roadText != null && _irrigText == null) _wholeText = $"({_roadText})";
                    else if (_roadText == null && _irrigText != null) _wholeText = $"({_irrigText})";
                    else if (_roadText != null && _irrigText != null) _wholeText = $"({_roadText}, {_irrigText})";
                    Draw.Text(e.Graphics, _wholeText, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, _column), _backColor, 1, 1);
                }
                // If airbase/fortress present
                _airbaseText = null;
                if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Airbase || Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Fortress)
                {
                    _column += 18;
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Fortress) _airbaseText = "Fortress";
                    if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Airbase) _airbaseText = "Airbase";
                    Draw.Text(e.Graphics, $"({_airbaseText})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, _column), _backColor, 1, 1);
                }
                // If pollution present
                if (Map.TileC2(Game.ActiveXY[0], Game.ActiveXY[1]).Pollution)
                {
                    _column += 18;
                    Draw.Text(e.Graphics, "(Pollution)", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, _column), _backColor, 1, 1);
                }
                _column += 5;

                // Show info for other units on the tile
                int drawCount = 0;
                foreach (IUnit unit in _unitsOnThisTile.Where(u => u != Game.ActiveUnit))
                {
                    // First check if there is vertical space still left for drawing in panel
                    if (_column + 69 > UnitPanel.Height) break;

                    // Draw unit
                    Draw.Unit(e.Graphics, unit, false, 1, new Point(7, _column + 27));
                    // Show other unit info
                    _column += 20;
                    _cityName = (unit.HomeCity == null) ? "NONE" : Game.ActiveUnit.HomeCity.Name;
                    Draw.Text(e.Graphics, _cityName, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(80, _column), _backColor, 1, 1);
                    _column += 18;
                    Draw.Text(e.Graphics, Order2string(unit.Order), _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(80, _column), _backColor, 1, 1);
                    _column += 18;
                    Draw.Text(e.Graphics, unit.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(80, _column), _backColor, 1, 1);

                    System.Diagnostics.Debug.WriteLine($"{unit.Name} drawn");

                    drawCount++;
                }

                // If not all units were drawn print a message
                if (_unitsOnThisTile.Count - 1 != drawCount)    // -1 because you must not count in active unit
                {
                    _column += 22;
                    _text = _unitsOnThisTile.Count - 1 - drawCount == 1 ? "Unit" : "Units";
                    Draw.Text(e.Graphics, $"({_unitsOnThisTile.Count - 1 - drawCount} More {_text})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(9, _column), _backColor, 1, 1);
                }
            }

            // Blinking "end of turn" message
            if (WaitingAtEndOfTurn)
            {
                using var _font2 = new Font("Times New Roman", 12, FontStyle.Bold);
                Color _EoTcolor = BoolSwitcher ? Color.White : Color.FromArgb(135, 135, 135);
                Draw.Text(e.Graphics, "End of Turn", _font2, StringAlignment.Near, StringAlignment.Near, _EoTcolor, new Point(5, UnitPanel.Height - 51), Color.Black, 1, 0);
                Draw.Text(e.Graphics, "(Press ENTER)", _font2, StringAlignment.Near, StringAlignment.Near, _EoTcolor, new Point(10, UnitPanel.Height - 33), Color.Black, 1, 0);
            }
        }

        private void Panel_Click(object sender, MouseEventArgs e)
        {
            if (WaitingAtEndOfTurn)
            {
                WaitingAtEndOfTurn = false;
                Game.NewPlayerTurn();
            }
            else
            {
                _main.ViewPieceMode = !_main.ViewPieceMode;
                UnitPanel.Refresh();
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
            }
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        UnitPanel.Refresh();
                        break;
                    }
                default: break;
            }
        }

        private void PlayerEventHappened(object sender, PlayerEventArgs e)
        {
            switch (e.EventType)
            {
                case PlayerEventType.NewTurn:
                    {
                        WaitingAtEndOfTurn = false;
                        StatsPanel.Refresh();
                        UnitPanel.Refresh();
                        break;
                    }
                default: break;
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                //Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        UnitPanel.Refresh();
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        UnitPanel.Refresh();
                        break;
                    }
                default:
                    break;
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            WaitingAtEndOfTurn = true;
            Timer.Start();
            UnitPanel.Refresh();
        }

        private void Timer_Tick (object sender, EventArgs e)
        {
            UnitPanel.Refresh();
        }

        private bool _boolSwitcher;
        private bool BoolSwitcher
        {
            get
            {
                if (this == null) _boolSwitcher = true;
                _boolSwitcher = !_boolSwitcher;   // Change state when this is called
                return _boolSwitcher;
            }
        }

        // Concert an order enum to string
        private string Order2string(OrderType order)
        {
            if (order == OrderType.BuildAirbase) return "Build Airbase";
            else if (order == OrderType.BuildFortress) return "Build Fortress";
            else if (order == OrderType.BuildIrrigation) return "Build Irrigation";
            else if (order == OrderType.BuildMine) return "Build Mine";
            else if (order == OrderType.BuildRoad) return "Build Road";
            else if (order == OrderType.CleanPollution) return "Clean Pollution";
            else if (order == OrderType.Fortified) return "Fortify";
            else if (order == OrderType.GoTo) return "Go to xxx";
            else if (order == OrderType.NoOrders) return "No Orders";
            else if (order == OrderType.Sleep) return "Sleep";
            else if (order == OrderType.Transform) return "Transform";
            else return null;
        }
    }
}
