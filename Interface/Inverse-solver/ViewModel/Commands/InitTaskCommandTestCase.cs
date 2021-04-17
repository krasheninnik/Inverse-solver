using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class InitTaskCommandTestCase : ICommand
    {
        public TaskViewModel ViewModel { get; set; }
        public InitTaskCommandTestCase(TaskViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            // there must be condition: all fields in form have been filled
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.InitTaskTestCase();
        }
    }
}
