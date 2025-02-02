﻿using System;
//using System.Drawing;
using Eto.Drawing;
using EtoFormsUIExtensionMethods;
using Civ2engine.Units;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static void UnitSprite(Graphics g, UnitType type, bool isSleeping, bool isFortified, int zoom, Point dest)
        {
            using var _unitPic = Images.Units[(int)type].Resize(zoom);
            if (!isSleeping)
            {
                g.DrawImage(_unitPic, new Rectangle(dest.X, dest.Y, _unitPic.Width, _unitPic.Height));
            }
            else     // Sentry
            {
                using var _attr = ModifyImage.ConvertToGray();
                g.DrawImage(_unitPic, new Rectangle(dest.X, dest.Y, _unitPic.Width, _unitPic.Height));
            }

            // Draw fortification
            using var _fortPic = Images.Fortified.Resize(zoom);
            if (isFortified) g.DrawImage(_fortPic, dest.X, dest.Y);
        }

        public static void UnitShield(Graphics g, UnitType unitType, int ownerId, OrderType unitOrder, bool isStacked, int unitHP, int unitMaxHP, int zoom, Point dest)
        {
            // Draw unit shields. First determine if the shield is on the left or right side
            Point frontLoc = Images.UnitShieldLoc[(int)unitType];
            Point backLoc = frontLoc;
            if (frontLoc.X < 32) backLoc.X -= 4;
            else backLoc.X += 4;
            int shadowXoffset = frontLoc.X < 32 ? -1 : 1;
            // Scale locations according to zoom (shadow is always offset by 1)
            frontLoc.X = frontLoc.X.ZoomScale(zoom);
            frontLoc.Y = frontLoc.Y.ZoomScale(zoom);
            backLoc.X = backLoc.X.ZoomScale(zoom);
            backLoc.Y = backLoc.Y.ZoomScale(zoom);

            // If unit stacked --> draw back shield with its shadow
            using var _shadowPic = Images.ShieldShadow.Resize(zoom);
            if (isStacked)
            {
                // Back shield shadow
                g.DrawImage(_shadowPic, new Rectangle(dest.X + backLoc.X + shadowXoffset, dest.Y + backLoc.Y + 1, _shadowPic.Width, _shadowPic.Height));
                // Back shield
                using var _backPic = Images.ShieldBack[ownerId].Resize(zoom);
                g.DrawImage(_backPic, new Rectangle(dest.X + backLoc.X, dest.Y + backLoc.Y, _backPic.Width, _backPic.Height));
            }

            // Front shield shadow
            g.DrawImage(_shadowPic, new Rectangle(dest.X + frontLoc.X + shadowXoffset, dest.Y + frontLoc.Y, _shadowPic.Width, _shadowPic.Height));

            // Front shield
            using var _frontPic = Images.ShieldFront[ownerId].Resize(zoom);
            g.DrawImage(_frontPic, new Rectangle(dest.X + frontLoc.X, dest.Y + frontLoc.Y, _frontPic.Width, _frontPic.Height));

            // Determine hitpoints bar size
            int hpBarX = (int)Math.Floor((float)unitHP * 12 / unitMaxHP);
            Color hpColor;
            if (hpBarX <= 3)
                hpColor = Color.FromArgb(243, 0, 0); // Red
            else if (hpBarX >= 4 && hpBarX <= 8)
                hpColor = Color.FromArgb(255, 223, 79);  // Yellow
            else
                hpColor = Color.FromArgb(87, 171, 39);   // Green

            // Draw black background for hitpoints bar
            using var _brush1 = new SolidBrush(Color.FromArgb(0, 0, 0));
            g.FillRectangle(_brush1, new Rectangle(dest.X + frontLoc.X, dest.Y + frontLoc.Y, 12.ZoomScale(zoom), 7.ZoomScale(zoom)));

            // Draw hitpoints bar
            using var _brush2 = new SolidBrush(hpColor);
            g.FillRectangle(_brush2, new Rectangle(dest.X + frontLoc.X, dest.Y + frontLoc.Y + 2.ZoomScale(zoom), hpBarX.ZoomScale(zoom), 3.ZoomScale(zoom)));

            // Text on front shield
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            string shieldText;
            switch (unitOrder)
            {
                case OrderType.Fortify:
                case OrderType.Fortified: shieldText = "F"; break;
                case OrderType.Sleep: shieldText = "S"; break;
                case OrderType.BuildFortress: shieldText = "F"; break;
                case OrderType.BuildRoad: shieldText = "R"; break;
                case OrderType.BuildIrrigation: shieldText = "I"; break;
                case OrderType.BuildMine: shieldText = "m"; break;
                case OrderType.Transform: shieldText = "O"; break;
                case OrderType.CleanPollution: shieldText = "p"; break;
                case OrderType.BuildAirbase: shieldText = "E"; break;
                case OrderType.GoTo: shieldText = "G"; break;
                case OrderType.NoOrders: shieldText = "-"; break;
                default: shieldText = "-"; break;
            }
            var formattedText = new FormattedText()
            {
                Font = new Font("Arial", 8.ZoomScale(zoom)),
                ForegroundBrush = new SolidBrush(Colors.Black),
                Text = shieldText
            };
            var textSize = formattedText.Measure();
            g.DrawText(formattedText, new Point(dest.X + frontLoc.X + 6.ZoomScale(zoom) - (int)textSize.Width / 2, dest.Y + frontLoc.Y + 12.ZoomScale(zoom) - (int)textSize.Height / 2));
        }

        public static void Unit(Graphics g, IUnit unit, bool isStacked, int zoom, Point dest)
        {
            UnitShield(g, unit.Type, unit.Owner.Id, unit.Order, isStacked, unit.HitPoints, unit.HitpointsBase, zoom, dest);
            UnitSprite(g, unit.Type, unit.Order == OrderType.Sleep, unit.Order == OrderType.Fortified, zoom, dest);
        }
    }
}
