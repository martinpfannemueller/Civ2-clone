﻿using System;
using System.Drawing;
using Civ2engine.Enums;

namespace Civ2engine.Events
{
    public class MapEventArgs : EventArgs
    {
        public MapEventType EventType { get; }

        public int[] CentrXY;
        public int[] CentrOffset;
        public int[] ActiveOffset;
        public int[] PanelMap_offset;
        public int[] MapPanel_offset;
        public Rectangle MapRect1;
        public Rectangle MapRect2;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }

        public MapEventArgs(MapEventType eventType, int[] centrXY)
        {
            EventType = eventType;
            CentrXY = centrXY;
        }

        //public MapEventArgs(MapEventType eventType, int[] centrXY, int[] centrOffset, int[] activeOffset, int[] panelMap_offset, int[] mapPanel_offset, Rectangle mapRect1, Rectangle mapRect2)
        //{
        //    _eventType = eventType;
        //    CentrXY = centrXY;
        //    CentrOffset = centrOffset;
        //    ActiveOffset = activeOffset;
        //    PanelMap_offset = panelMap_offset;
        //    MapPanel_offset = mapPanel_offset;
        //    MapRect1 = mapRect1;
        //    MapRect2 = mapRect2;
        //}
    }
}
