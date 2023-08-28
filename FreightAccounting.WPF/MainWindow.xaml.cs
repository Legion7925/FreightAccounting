using FreightAccounting.Core.Interfaces.Repositories;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
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

namespace FreightAccounting.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDebtorRepository debtorRepository;

        public MainWindow(IDebtorRepository debtorRepository)
        {
            InitializeComponent();
            this.debtorRepository = debtorRepository;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void gridHears_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnChangeTheme_Checked(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            ITheme theme = paletteHelper.GetTheme();

            //Change the base theme to Dark
            theme.SetBaseTheme(Theme.Dark);
            //or theme.SetBaseTheme(Theme.Light);

            //Change all of the primary colors to Red
            theme.SetPrimaryColor(Colors.Blue);

            //Change all of the secondary colors to Blue
            theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue]);

            ////You can also change a single color on the theme, and optionally set the corresponding foreground color
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

            //Change the app's current theme
            paletteHelper.SetTheme(theme);

            //todo -- save to app setting for when user open after change theme -- todo
        }

        private void btnChangeTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            ITheme theme = paletteHelper.GetTheme();

            //Change the base theme to Dark
            theme.SetBaseTheme(Theme.Light);
            //or theme.SetBaseTheme(Theme.Light);

            //Change all of the primary colors to Red
            theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.LightBlue]);

            ////Change all of the secondary colors to Blue
            //theme.SetSecondaryColor(Colors.Blue);

            ////You can also change a single color on the theme, and optionally set the corresponding foreground color
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

            //Change the app's current theme
            paletteHelper.SetTheme(theme);

            //todo -- save to app setting for when user open after change theme -- todo
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddRemittance_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgReport_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void btnAddDebtors_Click(object sender, RoutedEventArgs e)
        {
            var addDebtorWindow = new AddDebtorWindow(debtorRepository, false, null, null);
            addDebtorWindow.ShowDialog();
        }

        private void dgDebtorsReport_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
