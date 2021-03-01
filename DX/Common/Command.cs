using System;
using System.Windows.Input;

namespace DX.Common
{
    class Command : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public Command(Action<object> execute, Func<object, bool> canexcute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            this.execute = execute;
            this.canExecute = canexcute;
        }

        public Command(Action<object> execute) : this(execute, null) { }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
