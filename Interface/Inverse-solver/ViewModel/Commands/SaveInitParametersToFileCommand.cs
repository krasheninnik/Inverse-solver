using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.ViewModel.Commands
{
    public class SaveInitParametersToFileCommand : AsyncCommandBase
    {
        public TaskViewModel ViewModel { get; set; }
        public SaveInitParametersToFileCommand(TaskViewModel viewModel, Action<Exception> onException) :
            base(onException)
        {
            ViewModel = viewModel;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            ViewModel.StatusMessage = "Start save initialization parameters to file...";
            await ViewModel.SaveInitParametersToFile();
            ViewModel.StatusMessage = "Parameters have been saved.";
        }
    }
}
