using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EventLogLibrary
{
    /// <summary>
    /// This class is used to register to a System Event Log to receive notifications from the service
    /// </summary>
    public class EventLogManager : IEventLogManager
    {
        #region fields

        private Collection<Action<EntryWrittenEventArgs>> list;
        private EventLog log;

        #endregion

        #region ctor

        public EventLogManager()
        {
            
        }
        
        #endregion

        #region methods

        /// <summary>
        /// Initialize the LogService
        /// </summary>
        public void Initialize(string source)
        {
            log = new EventLog("Application", Environment.MachineName, source);
            log.EnableRaisingEvents = true;

            list = new Collection<Action<EntryWrittenEventArgs>>();
        }

        /// <summary>
        /// Subscribe to the log event. When a new log is received the action will be executed
        /// </summary>
        public void Subscribe(Action<EntryWrittenEventArgs> action)
        {
            list.Add(action);
            log.EntryWritten += (s, e) => { action(e); };
        }

        /// <summary>
        /// Unsubscribe to the log event with a precise action
        /// </summary>
        public void Unsubscribe(Action<EntryWrittenEventArgs> action)
        {
            log.EntryWritten -= (s, e) => { action(e); };
        }

        /// <summary>
        /// Unsubscribe every action
        /// </summary>
        public void Unsubscribe()
        {
            foreach (Action<EntryWrittenEventArgs> action in list)
            {
                log.EntryWritten -= (s, e) => { action(e); };
            }
        }

        #endregion
    }
}
