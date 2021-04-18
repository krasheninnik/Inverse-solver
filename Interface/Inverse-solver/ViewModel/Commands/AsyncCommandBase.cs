using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inverse_solver.ViewModel.Commands
{
    public abstract class AsyncCommandBase : ICommand
    {
        private Action<Exception> onExeption;
        public AsyncCommandBase(Action<Exception> _onException)
        {
            this.onExeption = _onException;
        }

        private bool isExecuting;
        public bool IsExecuting
        {
            get { return isExecuting; }
            set { isExecuting = value; }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected virtual bool CanExecuteSync()
        {
            return true;
        }

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && CanExecuteSync();
        }

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await ExecuteAsync(parameter);
            }
            catch(Exception ex)
            {
                onExeption?.Invoke(ex);
            }
            IsExecuting = false;
        }

        protected abstract Task ExecuteAsync(object parameter);
    }
}
