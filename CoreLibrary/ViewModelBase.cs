using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CoreLibrary
{
    /// <summary>
    /// This class implements the INotifyPropertyChanged to be notified by the UI of the change of property.
    /// This peculiar implementation avoid writing the property name as a string and thus any spelling error since it use the IntelliSense system.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Notify Property Changed
        /// </summary>
        protected virtual void NotifyPropertyChanged<T>(Expression<Func<T>> expression)
        {
            string propertyName = GetPropertyName(expression);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get the string name for the property
        /// </summary>
        protected string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Property Changed Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
