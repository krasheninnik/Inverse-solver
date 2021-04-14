using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Inverse_solver.Model;
using Inverse_solver.ViewModel.Commands;
using Inverse_solver.ViewModel.Converters;
using Inverse_solver.Views;

namespace Inverse_solver.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public TaskViewModel()
        {
            this.task = new CalculatingTask();
            this.OpenSettingsFormCommand = new OpenSettingsFormCommand(this);
            this.InitTaskCommand = new InitTaskCommand(this);
        }

        CalculatingTask task { get; set; }

        // Commands
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; set; }

        public InitTaskCommand InitTaskCommand { get; set; }

        public void OpenSettingsForm()
        {
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
        }

        public void InitTask()
        {
            // this should call Model function and init Task with parameters
            Debug.WriteLine($"Hello, Hx {Hx}, Hy {Hy}... Xstart {Xstart}... You get it)0).");
            task.Test3(this.InitParameters);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        // Proooooops: 
        // Test developming props, need delete latter
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

        // Task settings props:
        // For measures grid:
        public int Hx { get; set; }
        public int Hy { get; set; }
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double Z0 { get; set; }
        // For Measures:
        public List<Value> MeasuredValues { get; set; }
        // For space grid:
        public double Xstart { get; set; }
        public double Xend { get; set; }
        public int XstepsAmount { get; set; }
        public double Ystart { get; set; }
        public double Yend { get; set; }
        public int YstepsAmount { get; set; }
        public double Zstart { get; set; }
        public double Zend { get; set; }
        public int ZstepsAmount { get; set; }

        public InitParameters InitParameters
        {
            get { return new InitParameters(Hx, Hy, X0, Y0, Z0, MeasuredValues, Xstart, Xend, XstepsAmount, Ystart, Yend, YstepsAmount, Zstart, Zend,ZstepsAmount); }
        }


    }
}

