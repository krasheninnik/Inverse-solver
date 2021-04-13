using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Inverse_solver.Model;
using Inverse_solver.ViewModel.Commands;

namespace Inverse_solver.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public SimpleCommand SimpleCommand { get; set; }
        public TaskViewModel()
        {
            this.SimpleCommand = new SimpleCommand(this);
        }
         
        public void SimpleMethod()
        {
            Debug.WriteLine("Hello");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private int _number1;
        public int Number1
        {
            get { return _number1; }
            set { _number1 = value; OnPropertyChanged("Number3"); }
        }

        private int _number2;
        public int Number2
        {
            get { return _number2; }
            set { _number2 = value; OnPropertyChanged("Number3"); }
        }

        public int Number3 { get => CalculatingTask.GetSumOf(Number1, Number2); }

    }
}

