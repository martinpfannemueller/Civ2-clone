﻿using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Units;
using Civ2engine.Events;
using ExtensionMethods;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static event EventHandler<PlayerEventArgs> OnPlayerEvent;

        //Update stats of all cities
        private void CitiesTurn()
        {
            foreach (City city in _cities.Where(a => a.Owner == _activeCiv))
            {
                //city.NewTurn();
            }
        }

        private void ChoseNextCiv()
        {
            // Make a list of active civs
            var civIds = new List<int>();
            foreach (var civ in GetActiveCivs) civIds.Add(civ.Id);

            // Increase game turn
            if (_activeCiv.Id == civIds.Last()) _turnNumber++;

            // Chose next civ
            for (int id = 0; id < civIds.Count; id++)
            {
                if (civIds[id] == _activeCiv.Id) 
                {
                    if (civIds[id] == civIds.Last()) _activeCiv = GetActiveCivs[0];
                    else _activeCiv = GetActiveCivs[id + 1];
                    break;
                }
            }

            // Reset turns of all units
            foreach (var unit in GetActiveUnits.Where(n => n.Owner == _activeCiv))
            {
                unit.TurnEnded = false;
                unit.MovePointsLost = 0;

                // Increase counters
                if ((unit.Order == OrderType.BuildIrrigation) || (unit.Order == OrderType.BuildRoad) || (unit.Order == OrderType.BuildMine)) unit.Counter += 1;
            }

            // Update all cities
            CitiesTurn();

            // Choose next unit
            ChooseNextUnit();

            OnPlayerEvent?.Invoke(null, new PlayerEventArgs(PlayerEventType.NewTurn));
        }

        public void BuildCity(string cityName)
        {
            int x = _activeUnit.X;
            int y = _activeUnit.Y;
            bool[] improvements = new bool[34];
            bool[] wonders = new bool[28];
            for (int i = 0; i < 34; i++) improvements[i] = false;
            for (int i = 0; i < 28; i++) wonders[i] = false;
            //Game.CreateCity(x, y, false, false, false, false, false, false, false, false, false, Game.Instance.ActiveUnit.Civ, 1, Game.Instance.ActiveUnit.Civ, 0, 0, 0, cityName, 0, 0, 0, 0, improvements, 0, 0, 0, 0, 0, 0, 0, 0, 0, wonders);

            DeleteUnit(_activeUnit);
        }

        public void DeleteUnit(IUnit unit)
        {
            if (_activeUnit == unit)
            {
                Game.GetUnits.Remove(unit);
                ChooseNextUnit();
            }
            else
            {
                Game.GetUnits.Remove(unit);
            }
        }

        //Make visible (potential) hidden tiles when active unit has completed movement
        public void UpdateWorldMapAfterUnitHasMoved()
        {
            //Offsets of tiles around active unit
            List<int[]> offsets = new List<int[]>
            {
                new int[] {0, -2},
                new int[] {1, -1},
                new int[] {2, 0},
                new int[] {1, 1},
                new int[] {0, 2},
                new int[] {-1, 1},
                new int[] {-2, 0},
                new int[] {-1, -1}
            };

            //For each offset make the tile visible if it isn't yet
            foreach (int[] offset in offsets)
            {
                int[] coords = new int[] { _activeUnit.X, _activeUnit.Y }.Civ2xy();
                coords[0] += offset[0];
                coords[1] += offset[1];
                Map.Visibility[coords[0], coords[1]][_activeCiv.Id] = true;
            }

            //Update the map image
            //Draw.RedrawMap(new int[] { Game.ActiveUnit.X, Game.ActiveUnit.Y });
        }

        //public static void GiveOrder(OrderType order)
        //{
        //    switch (order)
        //    {
        //        //case OrderType.MoveSW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, 1)));
        //        //    break;
        //        //case OrderType.MoveS:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, 2)));
        //        //    break;
        //        //case OrderType.MoveSE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, 1)));
        //        //    break;
        //        //case OrderType.MoveW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-2, 0)));
        //        //    break;
        //        //case OrderType.MoveE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(2, 0)));
        //        //    break;
        //        //case OrderType.MoveNW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, -1)));
        //        //    break;
        //        //case OrderType.MoveN:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, -2)));
        //        //    break;
        //        //case OrderType.MoveNE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, -1)));
        //        //    break;
        //        case OrderType.BuildIrrigation: 
        //            Game.Instance.ActiveUnit.BuildIrrigation();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildMine: 
        //            Game.Instance.ActiveUnit.BuildMines();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.GoTo:
        //            //TODO: goto key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Fortify: 
        //            Game.Instance.ActiveUnit.Fortify();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Sleep: 
        //            Game.Instance.ActiveUnit.Sleep();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.GoHome:
        //            //TODO: gohome key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.SkipTurn: 
        //            Game.Instance.ActiveUnit.SkipTurn();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildCity: 
        //            Game.Instance.ActiveUnit.BuildCity();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildRoad: 
        //            Game.Instance.ActiveUnit.BuildRoad();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Transform: 
        //            Game.Instance.ActiveUnit.Transform();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Automate:
        //            //TODO: automate key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.ActivateUnit:
        //            //TODO: activate unit key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        default: break;
        //    }
        //}


    }
}
