using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public class BuildMatrixCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }

        public BuildMatrixCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start build matrix for task...";
            await ViewModel.BuildMatrix();
            ViewModel.StatusMessage = "Matrix has been builded.";
        }

        protected override bool CanExecuteSync()
        {
            return this.ViewModel.IsTaskInitializated && !this.ViewModel.IsMatrixBuilded;
        }
    }
}
