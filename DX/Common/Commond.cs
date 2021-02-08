using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DX.Common
{
    public delegate void excute();

    class Commond : ICommand
    {
        private excute ex {get;set;}
        public Commond(excute ex) 
        {
            this.ex = ex;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            ex.Invoke();
        }
    }
}
