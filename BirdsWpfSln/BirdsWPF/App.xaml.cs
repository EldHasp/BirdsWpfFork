﻿using BirdsCommon;
using BirdsCommon.Repository;
using BirdsRepository;
using BirdsViewModels;
using BirdsWPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BirdsWPF
{
    public partial class App : Application
    {
        public App()
        {
            Startup += async (s, e) =>
            {
                MainBirdsViewModel vm = (MainBirdsViewModel)FindResource("mainVM");
                vm.Birds.EnableCollectionSynchronization();
                await vm.LoadAsync();
            };
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}