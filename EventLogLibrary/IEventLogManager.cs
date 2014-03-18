using System;
using System.Diagnostics;

namespace EventLogLibrary
{
    public interface IEventLogManager
    {
        void Initialize(string source);

        void Subscribe(Action<EntryWrittenEventArgs> action);

        void Unsubscribe(Action<EntryWrittenEventArgs> action);

        void Unsubscribe();
        
    }
}
