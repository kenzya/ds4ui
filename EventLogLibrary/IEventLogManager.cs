using System;
using System.Diagnostics;

namespace EventLogLibrary
{
    /// <summary>
    /// Interface of the EventLogManager
    /// </summary>
    public interface IEventLogManager
    {
        void Initialize(string source);

        void Subscribe(Action<EntryWrittenEventArgs> action);

        void Unsubscribe(Action<EntryWrittenEventArgs> action);

        void Unsubscribe();
        
    }
}
