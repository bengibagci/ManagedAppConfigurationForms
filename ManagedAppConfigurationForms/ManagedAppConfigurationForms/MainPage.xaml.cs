using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedAppConfigurationForms.Interfaces;
using Xamarin.Forms;

namespace ManagedAppConfigurationForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            logText.Text = DependencyService.Get<ILogService>().Log();
        }
    }
}
