using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace CoreLibrary
{
    /// <summary>
    /// CustomCollection class derived from ObservableCollection.
    /// It's an ObservableCollection threadsafe that can add and remove items from a different thread than UI.
    /// It contains also methods to add and remove automatically all its members
    /// </summary>    
    public class CustomCollection<T> : ObservableCollection<T>
    {
        #region properties

        /// <summary>
        /// Suspend/resume notification changes
        /// </summary>
        private bool suspendCollectionChangeNotification;
        public void ResumeCollectionChangeNotification()
        {
            this.suspendCollectionChangeNotification = false;
        }
        public void SuspendCollectionChangeNotification()
        {
            this.suspendCollectionChangeNotification = true;
        }

        #endregion // properties

        #region ctor

        public CustomCollection()
            : base()
        {
            this.suspendCollectionChangeNotification = false;
        }

        #endregion // ctor

        #region methods

        /// <summary>
        /// Replace an item of the collection
        /// </summary>
        public void Replace(int index, T value)
        {
            this.SuspendCollectionChangeNotification();
            try
            {
                base.SetItem(index, value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Verificare che il tipo dell'oggetto sia corretto", ex);
            }
            finally
            {
                this.NotifyChanges();
            }
        }

        /// <summary>
        /// Add a list of items to the collection
        /// </summary>
        public void AddItems(IList items)
        {
            this.SuspendCollectionChangeNotification();
            try
            {
                foreach (var i in items)
                {
                    InsertItem(Count, (T)i);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Verificare che il tipo degli oggetti sia corretto", ex);
            }
            finally
            {
                this.NotifyChanges();
            }
        }

        /// <summary>
        /// Remove a list from the collection
        /// </summary>        
        public void RemoveItems(IList items)
        {
            this.SuspendCollectionChangeNotification();
            try
            {
                foreach (var i in items)
                {
                    Remove((T)i);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Verificare che il tipo degli oggetti sia corretto", ex);
            }
            finally
            {
                this.NotifyChanges();
            }
        }

        #endregion // methods

        #region events

        /// <summary>
        /// Notification Event
        /// </summary>
        public void NotifyChanges()
        {
            this.ResumeCollectionChangeNotification();
            NotifyCollectionChangedEventArgs arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            this.OnCollectionChanged(arg);
        }
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                if (!this.suspendCollectionChangeNotification)
                {
                    NotifyCollectionChangedEventHandler eventHandler = this.CollectionChanged;
                    if (eventHandler == null)
                    {
                        return;
                    }

                    Delegate[] delegates = eventHandler.GetInvocationList();

                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                        if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                        {
                            dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                        }
                        else
                        {
                            handler(this, e);
                        }
                    }
                }
            }
        }

        #endregion // events
    }
}
