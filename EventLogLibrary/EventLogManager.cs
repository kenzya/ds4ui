
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EventLogLibrary
{
    public class EventLogManager : IEventLogManager
    {
        #region fields

        private Collection<Action<EntryWrittenEventArgs>> list;
        private EventLog log;

        #endregion

        #region constructor

        public EventLogManager()
        {
            
        }
        
        #endregion

        #region methods

        public void Initialize(string source)
        {
            log = new EventLog("Application", Environment.MachineName, source);
            log.EnableRaisingEvents = true;

            list = new Collection<Action<EntryWrittenEventArgs>>();
        }
        public void Subscribe(Action<EntryWrittenEventArgs> action)
        {
            list.Add(action);
            log.EntryWritten += (s, e) => { action(e); };
        }
        public void Unsubscribe(Action<EntryWrittenEventArgs> action)
        {
            log.EntryWritten -= (s, e) => { action(e); };
        }
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
