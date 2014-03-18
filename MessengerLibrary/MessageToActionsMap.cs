using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessengerLibrary
{
    /// <summary>
    /// This class is an implementation detail of the Messenger class.
    /// </summary>
    internal class MessageToActionsMap
    {
        #region Fields

        // Stores a hash where the key is the message and the value is the list of callbacks to invoke.
        readonly Dictionary<string, List<WeakAction>> _map = new Dictionary<string, List<WeakAction>>();

        #endregion // Fields

        #region Constructor

        internal MessageToActionsMap()
        {
        }

        #endregion // Constructor

        #region AddAction

        /// <summary>
        /// Adds an action to the list.
        /// </summary>
        /// <param name="message">The message to register.</param>
        /// <param name="target">The target object to invoke, or null.</param>
        /// <param name="method">The method to invoke.</param>
        /// <param name="actionType">The type of the Action delegate.</param>
        internal void AddAction(string message, object target, MethodInfo method, Type actionType)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (method == null)
                throw new ArgumentNullException("method");

            lock (_map)
            {
                if (!_map.ContainsKey(message))
                    _map[message] = new List<WeakAction>();

                _map[message].Add(new WeakAction(target, method, actionType));
            }
        }

        #endregion // AddAction

        #region GetActions

        /// <summary>
        /// Gets the list of actions to be invoked for the specified message
        /// </summary>
        /// <param name="message">The message to get the actions for</param>
        /// <returns>Returns a list of actions that are registered to the specified message</returns>
        internal List<Delegate> GetActions(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            List<Delegate> actions;
            lock (_map)
            {
                if (!_map.ContainsKey(message))
                {
                    return null;
                }

                List<WeakAction> weakActions = _map[message];
                actions = new List<Delegate>(weakActions.Count);
                for (int i = weakActions.Count - 1; i > -1; --i)
                {
                    WeakAction weakAction = weakActions[i];
                    if (weakAction == null)
                    {
                        continue;
                    }

                    Delegate action = weakAction.CreateAction();
                    if (action != null)
                    {
                        actions.Add(action);
                    }
                    else
                    {
                        // The target object is dead, so get rid of the weak action.
                        weakActions.Remove(weakAction);
                    }
                }

                // Delete the list from the map if it is now empty.
                if (weakActions.Count == 0)
                {
                    _map.Remove(message);
                }
            }

            // Reverse the list to ensure the callbacks are invoked in the order they were registered.
            actions.Reverse();

            return actions;
        }

        #endregion // GetActions

        #region TryGetParameterType

        /// <summary>
        /// Get the parameter type of the actions registered for the specified message.
        /// </summary>
        /// <param name="message">The message to check for actions.</param>
        /// <param name="parameterType">
        /// When this method returns, contains the type for parameters 
        /// for the registered actions associated with the specified message, if any; otherwise, null.
        /// This will also be null if the registered actions have no parameters.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if any actions were registered for the message</returns>
        internal bool TryGetParameterType(string message, out Type parameterType)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            parameterType = null;
            List<WeakAction> weakActions;
            lock (_map)
            {
                if (!_map.TryGetValue(message, out weakActions) || weakActions.Count == 0)
                {
                    return false;
                }
            }
            parameterType = weakActions[0].ParameterType;
            return true;
        }

        #endregion // TryGetParameterType
    }
}
