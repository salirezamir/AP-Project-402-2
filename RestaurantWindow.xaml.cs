﻿using Restaurant_Manager.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Restaurant_Manager
{
    /// <summary>
    /// Interaction logic for RestaurantWindow.xaml
    /// </summary>
    public partial class RestaurantWindow : Window
    {
        User _user;
        Restaurant _restaurant;
        public RestaurantWindow(Restaurant restaurant,User user)
        {
            InitializeComponent();
            _restaurant = restaurant;
            _user = user;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void Tb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void HistoryDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}