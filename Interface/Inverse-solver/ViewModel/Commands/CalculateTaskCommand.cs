using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class CalculateTaskCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }

        public CalculateTaskCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start calclulating task...";
            await ViewModel.CalculateTask();
            ViewModel.StatusMessage = "Task has been calculated.";
        }

        protected override bool CanExecuteSync()
        {
            return this.ViewModel.IsTaskInitializated && !this.ViewModel.IsTaskCalculated;
        }
    }
}
