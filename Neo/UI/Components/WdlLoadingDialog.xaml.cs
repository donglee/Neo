﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neo.UI.Components
{
    /// <summary>
    /// Interaction logic for WdlLoadingDialog.xaml
    /// </summary>
    public partial class WdlLoadingDialog
    {
        public WdlLoadingDialog()
        {
            InitializeComponent();
	        this.ProgressIndicator.Minimum = 0;
	        this.ProgressIndicator.Maximum = 100;
        }

        public float Progress
        {
            get { return (float) this.ProgressIndicator.Value; }
            set { this.ProgressIndicator.Value = value; }
        }

        public string Action { set { this.ActionIndicator.Content = value; } }
        public bool ShouldClose { get; set; }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (!this.ShouldClose)
            {
	            e.Cancel = true;
            }
        }
    }
}
