using Inverse_solver.ViewModel;
using System;
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
using System.Windows.Shapes;

namespace Inverse_solver.Views
{
    /// <summary>
    /// Interaction logic for MagneticInductionView.xaml
    /// </summary>
    public partial class MagneticInductionView : Window
    {
        public MagneticInductionView()
        {
            InitializeComponent();
        }

        void On_Closing(object sender, CancelEventArgs e)
        {
            (this.DataContext as TaskViewModel).MagneticInductionViewOpened = false;
            this.DataContext = null;
        }
    }
}
