using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.ViewModel.Commands
{
    // 

    public class SolveWithAlphaFittingCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }

        public SolveWithAlphaFittingCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start solve task with fitting alpha...";
            await ViewModel.SolveTaskWithAlphaFitting();
            ViewModel.StatusMessage = $"Task has been solved with fitted alpha: {ViewModel.FittedAlpha}";
        }

        protected override bool CanExecuteSync()
        {
            return this.ViewModel.IsMatrixBuilded && !this.ViewModel.IsSolvedWithAlphaFitting;
        }
    }
}
