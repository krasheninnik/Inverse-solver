using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class OpenMagneticInductionViewCommand : ICommand
    {
        public TaskViewModel ViewModel { get; set; }
        public OpenMagneticInductionViewCommand(TaskViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this.ViewModel.IsTaskCalculated;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.OpenMagneticInductionView();
        }
    }
}
