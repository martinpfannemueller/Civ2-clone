﻿using Eto.Drawing;
using EtoFormsUIExtensionMethods;
using Civ2engine;
using Civ2engine.Enums;
using System.Diagnostics;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        // Draw an image of city
        public static void City(Graphics g, City city, bool isCitySizeWindow, int zoom, Point dest)
        {
            // Determine city style
            // For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            // If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            CityStyleType style = city.Owner.CityStyle;
            int sizeStyle;
            if (style != CityStyleType.Industrial && style != CityStyleType.Modern)
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // Palace exists
                {
                    if (city.Size <= 3) sizeStyle = 1;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 3) sizeStyle = 0;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 1;
                    else if (city.Size > 5 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }
            // If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (style == CityStyleType.Industrial)
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // Palace exists
                {
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 1;
                    else if (city.Size > 7 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }
            // If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // Palace exists
                {
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 1;
                    else if (city.Size > 10 && city.Size <= 18) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }

            // Depending on the presence of a wall, get images of city and locations of size window & flag
            var cityPic = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWall[(int)style, sizeStyle] : Images.City[(int)style, sizeStyle];
            var sizeWinLoc = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWallSizeWindowLoc[(int)style, sizeStyle] : Images.CitySizeWindowLoc[(int)style, sizeStyle];
            var flagLoc = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWallFlagLoc[(int)style, sizeStyle] : Images.CityFlagLoc[(int)style, sizeStyle];

            // Draw city
            using var _cityPic = cityPic.Resize(zoom);
            g.DrawImage(_cityPic, new Point(dest.X, dest.Y));

            // Draw city size window
            if (isCitySizeWindow)
            {
                // Rectangle
                using var _pen = new Pen(Colors.Black);
                g.DrawRectangle(_pen,
                    dest.X + sizeWinLoc.X.ZoomScale(zoom) - 1,
                    dest.Y + sizeWinLoc.Y.ZoomScale(zoom) - 1,
                    9.ZoomScale(zoom),
                    13.ZoomScale(zoom));

                // Fill rectangle
                using var _brush1 = new SolidBrush(CivColors.Light[city.OwnerId]);
                g.FillRectangle(_brush1,
                    dest.X + sizeWinLoc.X.ZoomScale(zoom),
                    dest.Y + sizeWinLoc.Y.ZoomScale(zoom),
                    8.ZoomScale(zoom),
                    12.ZoomScale(zoom));

                // Size text
                var formattedText = new FormattedText()
                {
                    Font = new Font("Times New Roman", 10.ZoomScale(zoom), FontStyle.Bold),
                    ForegroundBrush = new SolidBrush(Colors.Black),
                    Text = city.Size.ToString()
                };
                var textSize = formattedText.Measure();
                g.DrawText(formattedText, new Point(dest.X + (sizeWinLoc.X + 4).ZoomScale(zoom) - (int)textSize.Width / 2, dest.Y + (sizeWinLoc.Y + 6).ZoomScale(zoom) - (int)textSize.Height / 2));
            }

            // Draw city flag if units are present in the city
            if (city.AnyUnitsPresent())
            {
                using var _flagPic = Images.CityFlag[city.OwnerId].Resize(zoom);
                g.DrawImage(_flagPic,
                    dest.X + (flagLoc.X - 3).ZoomScale(zoom),
                    dest.Y + (flagLoc.Y - 17).ZoomScale(zoom));
            }
        }

        public static void CityName(Graphics g, City city, int zoom, int[] offsetSqXY)
        {
            // Define text characteristics for zoom levels
            int shadowOffset, fontSize;
            switch (zoom)
            {
                case -7: shadowOffset = 0; fontSize = 1; break;
                case -6: shadowOffset = 0; fontSize = 3; break;
                case -5: shadowOffset = 0; fontSize = 5; break;
                case -4: shadowOffset = 1; fontSize = 7; break;
                case -3: shadowOffset = 1; fontSize = 10; break;
                case -2: shadowOffset = 1; fontSize = 11; break;
                case -1: shadowOffset = 1; fontSize = 13; break;
                case 0: shadowOffset = 2; fontSize = 14; break;
                case 1: shadowOffset = 2; fontSize = 16; break;
                case 2: shadowOffset = 2; fontSize = 17; break;
                case 3: shadowOffset = 2; fontSize = 19; break;
                case 4: shadowOffset = 2; fontSize = 21; break;
                case 5: shadowOffset = 2; fontSize = 24; break;
                case 6: shadowOffset = 2; fontSize = 25; break;
                case 7: shadowOffset = 2; fontSize = 26; break;
                case 8: shadowOffset = 2; fontSize = 28; break;
                default: shadowOffset = 2; fontSize = 14; break;
            }

            Point dest = new Point(Map.Xpx * offsetSqXY[0] + Map.Xpx, Map.Ypx * (offsetSqXY[1] + 2) + 2);

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            var formattedFrontText = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(CivColors.CityTextColor[city.OwnerId]),
                Text = city.Name
            };
            var formattedShadow1Text = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(Colors.Black),
                Text = city.Name
            };
            var formattedShadow2Text = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(Colors.Black),
                Text = city.Name
            };

            var textSize = formattedFrontText.Measure();
            g.DrawText(formattedShadow1Text, new Point(dest.X + shadowOffset - (int)textSize.Width / 2, dest.Y - (int)textSize.Height / 2));
            g.DrawText(formattedShadow2Text, new Point(dest.X - (int)textSize.Width / 2, dest.Y + shadowOffset - (int)textSize.Height / 2));
            g.DrawText(formattedFrontText, new Point(dest.X - (int)textSize.Width / 2, dest.Y - (int)textSize.Height / 2));
        }
    }
}
