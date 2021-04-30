using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    // Class for read JSON file and init task from it
    public class InitParameters: IDataErrorInfo, INotifyPropertyChanged
    {
        private bool RPCfromPmin = false;
        private bool RPCfromPmax = false;
        private bool RPCfromNx = false;
        private bool RPCfromNy = false;
        private bool RPCfromMeasuredValues = false;
        private bool RPCfromXstart = false;
        private bool RPCfromXend = false;
        private bool RPCfromYstart = false;
        private bool RPCfromYend = false;
        private bool RPCfromZstart = false;
        private bool RPCfromZend = false;
        private void updateCoupledVariable(ref bool coupledFlag, ref bool sourceFlag, string coupledName)
        {
            if (!coupledFlag)
            {
                sourceFlag = true;
                OnPropertyChanged(coupledName);
                sourceFlag = false;
            }
        }

        private void updateCoupledVariableByOneOf2(ref bool coupledFlag, ref bool sourceFlag1, ref bool sourceFlag2, string coupledName)
        {
            if (!coupledFlag)
            {
                sourceFlag1 = true;
                sourceFlag2 = true;
                OnPropertyChanged(coupledName);
                sourceFlag1 = false;
                sourceFlag2 = false;
            }
        }

        public string this[string columnName]
        {  
            get
            {
                string errorString = string.Empty;
                switch (columnName)
                {
                    // alpha fitting parameters:
                    case "Pmin":
                        if (this.Pmin >= this.Pmax) errorString = "Pmin must be less that Pmax.";
                        updateCoupledVariable(ref RPCfromPmax, ref RPCfromPmin, "Pmax");
                        break;
                    case "Pmax":
                        if (this.Pmin >= this.Pmax) errorString = "Pmin must be less that Pmax.";
                        updateCoupledVariable(ref RPCfromPmin, ref RPCfromPmax, "Pmin");
                        break;

                    case "AlphaStep": if (this.AlphaStep <= 0) errorString = "Hx must be positive double."; break;
                    case "FittingProcentThreshold": if (this.FittingProcentThreshold <= 0 || this.FittingProcentThreshold > 100) errorString = "FittingProcentThreshold must be positive double: less or equal 100"; break;

                    case "Hx": if (this.Hx <= 0) errorString = "Hx must be positive double."; break;
                    case "Hy": if (this.Hy <= 0) errorString = "Hy must be positive double."; break;

                    case "Nx": if (this.Nx <= 0) errorString = "Nx must be positive integer.";
                        updateCoupledVariableByOneOf2(ref RPCfromMeasuredValues, ref RPCfromNx, ref RPCfromNy, "MeasuredValues");
                        break;
                    case "Ny": if (this.Ny <= 0) errorString = "Ny must be positive integer.";
                        updateCoupledVariableByOneOf2(ref RPCfromMeasuredValues, ref RPCfromNx, ref RPCfromNy, "MeasuredValues");
                        break;
                    case "Alpha": if (this.Alpha < 0) errorString = "Alpha must be greater that zero."; break;

                    case "Xstart":
                        if (this.Xstart >= this.Xend) errorString = "Xstart must be less that Xend.";
                        updateCoupledVariable(ref RPCfromXend, ref RPCfromXstart, "Xend");
                        break;

                    case "Xend":                         
                        if (this.Xstart >= this.Xend) errorString = "Xstart must be less that Xend.";
                        updateCoupledVariable(ref RPCfromXstart, ref RPCfromXend, "Xstart");
                        break;


                    case "Ystart":         
                        if (this.Ystart >= this.Yend) errorString = "Ystart must be less that Yend.";
                        updateCoupledVariable(ref RPCfromYend, ref RPCfromYstart, "Yend");
                        break;

                    case "Yend":
                        if (this.Ystart >= this.Yend) errorString = "Ystart must be less that Yend.";
                        updateCoupledVariable(ref RPCfromYstart, ref RPCfromYend, "Ystart");
                        break;

                    case "Zstart":                 
                        if (this.Zstart >= this.Zend) errorString = "Zstart must be less that Zend.";
                        updateCoupledVariable(ref RPCfromZend, ref RPCfromZstart, "Zend");
                        break;

                    case "Zend":                        
                        if (this.Zstart >= this.Zend) errorString = "Zstart must be less that Zend.";
                        updateCoupledVariable(ref RPCfromZstart, ref RPCfromZend, "Zstart");
                        break;

                    case "XstepsAmount": if (this.XstepsAmount <= 0) errorString = "XstepsAmount must be positive integer."; break;
                    case "YstepsAmount": if (this.YstepsAmount <= 0) errorString = "YstepsAmount must be positive integer."; break;
                    case "ZstepsAmount": if (this.ZstepsAmount <= 0) errorString = "ZstepsAmount must be positive integer."; break;

                    case "MeasuredValues":
                        if (this.MeasuredValues?.Count != (Nx + 1) * (Ny + 1)) errorString = "Size of MeasredValues must be equal (Nx+1)*(Ny+1).";
                        break;
                }

                return errorString;
            }
        }

        public string Error
        {
            get { return null; }
        }

        // Task settings props
        // For measures grid:
        public double Hx { get; set; }
        public double Hy { get; set; }
        public int Nx { get; set; }
        public int Ny { get; set; }
        public double Alpha { get; set; }

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

        // alpha fitting parameters:
        public double Pmin { get; set; }
        public double Pmax { get; set; }
        public double AlphaStep { get; set; }
        public double FittingProcentThreshold { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
