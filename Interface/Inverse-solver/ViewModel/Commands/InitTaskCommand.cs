using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class InitTaskCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }
        public InitTaskCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start task initialization...";
            await ViewModel.InitTask(/*parameter as IClosable*/);
            ViewModel.StatusMessage = "Task has been initialized.";
        }
    }
}
