using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.ViewModel.Commands
{
    // 

    public class SolveWithAlphaCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }

        public SolveWithAlphaCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start solve task with setted alpha...";
            await ViewModel.SolveTaskWithAlphaSetted();
            ViewModel.StatusMessage = "Task has been solved.";
        }

        protected override bool CanExecuteSync()
        {
            return this.ViewModel.IsMatrixBuilded && !this.ViewModel.IsSolvedWithAlphaSetted;
        }
    }
}
