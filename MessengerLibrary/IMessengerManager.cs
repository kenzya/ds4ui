﻿using System;

namespace MessengerLibrary
{
    public interface IMessengerManager
    {
        void Register(string message, Action callback);
        void Register<T>(string message, Action<T> callback);
        void NotifyColleagues(string message, object parameter);
        void NotifyColleagues(string message);
    }
}
