﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using Shell32;

namespace EasyRename
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            wnd.Show();

            try
            {
                foreach (string file in e.Args)
                {
                    wnd.fileList.Add(file);
                }

                wnd.RefreshDisplayBox();
                wnd.RefreshExampleText();
                wnd.SetExampleFile();
            } catch (Exception ex)
            {
                wnd.NotifyException(ex);
            }
        }
    }
}
