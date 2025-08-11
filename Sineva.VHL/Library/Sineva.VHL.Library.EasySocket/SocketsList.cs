using System;
using System.Collections;
using System.ComponentModel;
using System.Net.Sockets;

namespace Sineva.VHL.Library.EasySocket
{
    /// <summary>
    /// Event handler for the <c>SocketsList</c> events.
    /// </summary>
    public delegate void SocketsListEventHandler();

    /// <summary>
    /// A collection of <c>SocketStream</c> objects.
    /// </summary>
    public class SocketsList : CollectionBase, IDisposable
    {
        #region Delegates

        ///<summary>
        ///Called when ever the <c>SocketsList</c> gets empty.
        ///</summary>
        private SocketsListEventHandler OnEmpty;

        ///<summary>
        ///Called when the first item added(after was <c>SocketsList</c> empty).
        ///</summary>
        private SocketsListEventHandler OnFirstItemAdded;

        #endregion

        #region Public Events

        /// <summary>
        /// Called when ever the <c>SocketsList</c> gets empty.
        /// </summary>
        [Category("Action"), Description("Is event is called when ever " +
                            "the SocketsList gets empty.")
        ]
        public event SocketsListEventHandler EmptyList
        {
            add { OnEmpty += value; }
            remove { OnEmpty -= value; }
        }

        /// <summary>
        /// Called when the first item added(after was <c>SocketsList</c> empty).
        /// </summary>
        [Category("Action"), Description("Called when the first item " +
                            "added(after was SocketsList empty).")
        ]
        public event SocketsListEventHandler FirstItemAdded
        {
            add { OnFirstItemAdded += value; }
            remove { OnFirstItemAdded -= value; }
        }

        #endregion

        #region Public functions
        /// <summary>
        /// Adds an Client object to the <c>SocketsList</c> collection.
        /// </summary>
        /// <param name="SocketStreamObject">
        /// <c>SocketStream</c> object to add to the collection.
        /// </param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(SocketStream SocketStreamObject)
        {

            int temp = List.Add(SocketStreamObject);

            //If first item added and the event has handlers call them.
            if (List.Count == 1 && OnFirstItemAdded != null) OnFirstItemAdded();

            return temp;
        }

        /// <summary>
        /// Inserts an item to the <c>SocketsList</c> at the specified position.
        /// </summary>
        /// <param name="Index">
        /// The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="SocketStreamObject">
        /// The item to insert into the <c>SocketsList</c>.
        /// </param>
        public void Insert(int Index, Socket SocketStreamObject)
        {
            List.Insert(Index, SocketStreamObject);

            //If first item added and the event has handlers call them.
            if (List.Count == 1 && OnFirstItemAdded != null) OnFirstItemAdded();
        }


        /// <summary>
        /// Removes the first occurrence of a specific object from the <c>SocketsList</c>.
        /// </summary>
        /// <param name="SocketStreamObject">
        /// The <c>SocketStream</c> to remove from the <c>SocketsList</c>. 
        /// </param>
        public void Remove(SocketStream SocketStreamObject)
        {
            List.Remove(SocketStreamObject);

            //If list is empty and has event handlers call them.
            if (List.Count == 0 && OnEmpty != null) OnEmpty();
        }

        /// <summary>
        /// Removes from the <c>SocketsList</c> an item at the specified index.
        /// </summary>
        /// <param name="Index">The zero-based index of the item to remove.
        /// </param>
        public new void RemoveAt(int Index)
        {
            base.RemoveAt(Index);

            //If list is empty and has event handlers call them.
            if (List.Count == 0 && OnEmpty != null) OnEmpty();
        }

        /// <summary>
        /// Removes all items from the <c>SocketsList</c>;
        /// </summary>
        public new void Clear()
        {
            base.Clear();

            //If has event handlers call them.
            if (OnEmpty != null) OnEmpty();
        }

        /// <summary>
        /// Determines whether two <c>SocketsList</c> instances are equal.
        /// </summary>
        /// <param name="socketsList">
        /// <c>SocketsList</c> object to compare with the current one.
        /// </param>
        /// <returns>Boolean flag indicates whether the elements are equal.
        /// </returns>
        public bool Equals(SocketsList socketsList)
        {
            return (List.Equals(socketsList.List));
        }

        /// <summary>
        /// Gets or sets the <c>SocketStream</c> object at the given location.
        /// </summary>
        public SocketStream this[int indexer]
        {
            get { return (SocketStream)List[indexer]; }
            set { List[indexer] = value; }
        }

        /// <summary>
        /// Determines whether the <c>SocketsList</c> contains a specific value.
        /// </summary>
        /// <param name="SocketStreamObject">
        /// The <c>SocketStream</c> object to locate in the <c>SocketsList</c>.
        /// </param>
        /// <returns>
        /// True if the <c>SocketStream</c> is found in the <c>SocketsList</c>; otherwise, false.
        /// </returns>
        public bool Contains(SocketStream SocketStreamObject)
        {
            return List.Contains(SocketStreamObject);
        }

        /// <summary>
        /// Determines the index of a specific item in the <c>SocketsList</c>.
        /// </summary>
        /// <param name="SocketStreamObject">
        /// The <c>SocketStream</c> object to locate in the <c>SocketsList</c>.
        /// </param>
        /// <returns>
        /// The index of value if found in the <c>SocketsList</c>; otherwise, -1.
        /// </returns>
        public int IndexOf(SocketStream SocketStreamObject)
        {
            return List.IndexOf(SocketStreamObject);
        }

        /// <summary>
        /// Removes all the <c>SocketStream</c> that are not connected.
        /// </summary>
        public void RemoveUnpluggedSockets()
        {
            for (int i = 0; i < List.Count; ++i)
            {
                if (!this[i].GetSocket.Connected)
                {
                    Remove(this[i]);
                    --i;
                }
                //If list is empty and has event handlers call them.
                if (List.Count == 0 && OnEmpty != null) OnEmpty();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            foreach (SocketStream Sock in List)
            {
                Sock.Dispose();
            }
            List.Clear();
        }

        #endregion
    }
}
