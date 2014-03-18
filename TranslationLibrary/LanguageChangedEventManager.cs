using System;
using System.Windows;

namespace TranslationLibrary
{
    /// <summary>
    /// WeakEventManager for handling the language change
    /// </summary>
    internal class LanguageChangedEventManager : WeakEventManager
    {
        #region properties

        /// <summary>
        /// Get the event manager for the current thread.
        /// </summary
        private static LanguageChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(LanguageChangedEventManager);
                LanguageChangedEventManager manager = GetCurrentManager(managerType) as LanguageChangedEventManager;
                if (manager == null)
                {
                    manager = new LanguageChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Event handler for the LanguageChanged event.
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }

        #endregion // event handlers

        #region WeakEventManager members

        /// <summary>
        /// Add a listener for the given source's event.
        /// </summary>
        internal static void AddListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        /// <summary>
        /// Remove a listener for the given source's event.
        /// </summary>
        internal static void RemoveListener(TranslationManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        /// <summary>
        /// Start listening to the given source for the event.
        /// </summary>
        protected override void StartListening(object source)
        {
            TranslationManager manager = source as TranslationManager;
            manager.LanguageChanged += OnLanguageChanged;
        }

        /// <summary>
        /// Stop listening to the given source for the event.
        /// </summary>
        protected override void StopListening(Object source)
        {
            TranslationManager manager = source as TranslationManager;
            manager.LanguageChanged -= OnLanguageChanged;
        }

        #endregion // WeakEventManager members
    }
}
