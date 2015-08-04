using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PET
{
    public partial class Feinstaubplaketten_Programm_Info : Form
    {
        public Feinstaubplaketten_Programm_Info()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strEmail = "info@newdirection.de";
            System.Diagnostics.Process.Start("mailto:" + strEmail);
        }
    }
}