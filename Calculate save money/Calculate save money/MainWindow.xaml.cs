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

namespace Calculate_save_money
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



        private void StartCalculate(object sender, RoutedEventArgs e)
        {
            int Income = int.Parse(IncomeTxt.Text);
            int paid = int.Parse(PayTxt.Text);
            int Need = int.Parse(NeededItemTxt.Text);
            int mixed = 0;
            mixed = (Income - paid);
            if (mixed > 0) 
            {
                float result = Need /mixed ;
                Calculated.Text = result.ToString();
                
            }
            else
            {
                MessageBox.Show("Imposible");
            }

        }


    }
}
