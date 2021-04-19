using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class SetInitParametersFromFileCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }
        public SetInitParametersFromFileCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start retrive initialization parameters from file...";
            await ViewModel.SetInitParametersFromFile();
            ViewModel.StatusMessage = "Parameters have been retrived.";
        }
    }
}
