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
        /// <summary>
        /// variabile che blocca o sblocca l'invio di notifiche dell'aggiornamento
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

        /// <summary>
        /// Costruttore della classe
        /// </summary>
        public CustomCollection()
            : base()
        {
            this.suspendCollectionChangeNotification = false;
        }

        /// <summary>
        /// Sostituisce un elemento della collection
        /// </summary>
        /// <param name="index">indice della sostituzione</param>
        /// <param name="value">valore da sostituire</param>
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
        /// Aggiunge una lista di elementi alla collection
        /// </summary>
        /// <param name="items">lista da aggiungere</param>
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
        /// Rimuove una lista di elementi dalla collection
        /// </summary>
        /// <param name="items">lista da rimuovere</param>
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

        /// <summary>
        /// Evento di notifica di un cambio della collection
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
    }
}
