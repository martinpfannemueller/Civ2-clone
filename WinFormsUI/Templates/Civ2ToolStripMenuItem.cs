﻿using System;
using System.Windows.Forms;

namespace WinFormsUI
{
    public partial class Civ2ToolStripMenuItem : ToolStripMenuItem
    {
        public Civ2ToolStripMenuItem(string text, EventHandler onClick, string displayKey)
        {
            Text = text;
            Click += onClick;
            ShortcutKeyDisplayString = displayKey;
            //ShortcutKeys = shortcutKeys;

        }

        //private Keys myShortcut = Keys.None;
        //public new Keys ShortcutKeys
        //{
        //    get { return myShortcut; }
        //    set { myShortcut = value; }
        //}

        //private Keys myShortcut = Keys.None;
        //public Keys Shortcut
        //{
        //    get { return myShortcut; }
        //    set { myShortcut = value; UpdateShortcutText(); }
        //}

        //private string myText = string.Empty;

        //public new string Text
        //{
        //    get { return myText; }
        //    set
        //    {
        //        myText = value;
        //        UpdateShortcutText();
        //    }
        //}

        //private void UpdateShortcutText()
        //{
        //    //base.Text = myText;
        //    base.Text = "OK";

        //    if (myShortcut != Keys.None)
        //        base.Text += "\t" + myShortcut.ToString(); // you can adjust that
        //}
    }
}
