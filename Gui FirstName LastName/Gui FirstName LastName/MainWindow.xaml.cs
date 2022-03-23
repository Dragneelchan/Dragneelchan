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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gui_FirstName_LastName
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SentData(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello " + NameText.Text + " " + LastNameText.Text);
            MessageBox.Show("Your Age is " + AgeText.Text + " Years old " + "You are " + GenderCbb.Text);
            NameText.Text = "AAA";
        }

        private void SentDota(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("How are yoou");
        }

        private void SentMaina(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bye");
        }
    }
}
