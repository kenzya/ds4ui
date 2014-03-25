using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CoreLibrary
{
    /// <summary>
    /// This class allows delegating the commanding logic to methods passed as parameters,
    /// and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region fields

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion // fields

        #region ctor

        /// <summary>
        /// Execute the parameter method when called
        /// </summary>
        /// <param name="execute"></param>
        public DelegateCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Execute the parameter method when called and when the parameter predicate is satisfied
        /// </summary>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // ctor

        #region events

        /// <summary>
        /// Event raised when the predicate change
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion // events

        #region methods

        /// <summary>
        /// Check the value of the predicate
        /// </summary>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Execute the method
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // methods
    }
}
