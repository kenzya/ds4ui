using System;
using System.Reflection;

namespace MessengerLibrary
{
    /// <summary>
    /// This class is an implementation detail of the MessageToActionsMap class.
    /// </summary>
    internal class WeakAction
    {
        #region Fields

        internal readonly Type ParameterType;

        readonly Type _delegateType;
        readonly MethodInfo _method;
        readonly WeakReference _targetRef;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Constructs a WeakAction.
        /// </summary>
        /// <param name="target">The object on which the target method is invoked, or null if the method is static.</param>
        /// <param name="method">The MethodInfo used to create the Action.</param>
        /// <param name="parameterType">The type of parameter to be passed to the action. Pass null if there is no parameter.</param>
        internal WeakAction(object target, MethodInfo method, Type parameterType)
        {
            if (target == null)
            {
                _targetRef = null;
            }
            else
            {
                _targetRef = new WeakReference(target);
            }

            _method = method;

            this.ParameterType = parameterType;

            if (parameterType == null)
            {
                _delegateType = typeof(Action);
            }
            else
            {
                _delegateType = typeof(Action<>).MakeGenericType(parameterType);
            }
        }

        #endregion // Constructor

        #region CreateAction

        /// <summary>
        /// Creates a "throw away" delegate to invoke the method on the target, or null if the target object is dead.
        /// </summary>
        internal Delegate CreateAction()
        {
            // Rehydrate into a real Action object, so that the method can be invoked.
            if (_targetRef == null)
            {
                return Delegate.CreateDelegate(_delegateType, _method);
            }
            else
            {
                try
                {
                    object target = _targetRef.Target;
                    if (target != null)
                        return Delegate.CreateDelegate(_delegateType, target, _method);
                }
                catch
                {
                }
            }

            return null;
        }

        #endregion // CreateAction
    }
}
