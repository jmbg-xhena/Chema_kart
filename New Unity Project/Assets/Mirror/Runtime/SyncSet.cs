using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror
{
    public class SyncSet<T> : ISet<T>, SyncObject
    {
        public delegate void SyncSetChanged(Operation op, T item);

        protected readonly ISet<T> objects;

        public int Count => objects.Count;
        public bool IsReadOnly { get; private set; }
        public event SyncSetChanged Callback;

        // OnDirty sets owner NetworkBehaviour's dirty mask when changed.
        public Action OnDirty { get; set; }

        // used to stop recording ever growing changes while we have no observers
        public Func<bool> IsRecording { get; set; } = () => true;

        public enum Operation : byte
        {
            OP_ADD,
            OP_CLEAR,
            OP_REMOVE
        }

        struct Change
        {
            internal Operation operation;
            internal T item;
        }

        // list of changes.
        // -> insert/delete/clear is only ONE change
        // -> changing the same slot 10x caues 10 changes.
        // -> note that this grows until next sync(!)
        // TODO Dictionary<key, change> to avoid ever growing changes / redundant changes!
        readonly List<Change> changes = new List<Change>();

        // how many changes we need to ignore
        // this is needed because when we initialize the list,
        // we might later receive changes that have already been applied
        // so we need to skip them
        int changesAhead;

        public SyncSet(ISet<T> objects)
        {
            this.objects = objects;
        }

        public void Reset()
        {
            IsReadOnly = false;
            changes.Clear();
            changesAhead = 0;
            objects.Clear();
        }

        // throw away all the changes
        // this should be called after a successful sync
        public void ClearChanges() => changes.Clear();

        // Deprecated 2021-09-17
        [Obsolete("Deprecated: Use ClearChanges instead.")]
        public void Flush() => changes.Clear();

        void AddOperation(Operation op, T item)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("SyncSets can only be modified at the server");
            }

            Change change = new Change
            {
                operation = op,
                item = item
            };

            if (IsRecording())
            {
                changes.Add(change);
                OnDirty?.Invoke();
            }

            Callback?.Invoke(op, item);
        }

        void AddOperation(Operation op) => AddOperation(op, default);

        public void OnSerializeAll(NetworkWriter writer)
        {
            // if init,  write the full list content
            writer.WriteUInt((uint)objects.Count);

            foreach (T obj in objects)
            {
                writer.Write(obj);
            }

            // all changes have been applied already
            // thus the client will need to skip all the pending changes
            // or they would be applied again.
            // So we write how many changes are pending
            writer.WriteUInt((uint)changes.Count);
        }

        public void OnSerializeDelta(NetworkWriter writer)
        {
            // write all the queued up changes
            writer.WriteUInt((uint)changes.Count);

            for (int i = 0; i < changes.Count; i++)
            {
                Change change = changes[i];
                writer.WriteByte((byte)change.operation);

                switch (change.operation)
                {
                    case Operation.OP_ADD:
                        writer.Write(change.item);
                        break;

                    case Operation.OP_CLEAR:
                        break;

                    case Operation.OP_REMOVE:
                        writer.Write(change.item);
                        break;
                }
            }
        }

        public void OnDeserializeAll(NetworkReader reader)
        {
            // This list can now only be modified by synchronization
            IsReadOnly = true;

            // if init,  write the full list content
            int count = (int)reader.ReadUInt();

            objects.Clear();
            changes.Clear();

            for (int i = 0; i < count; i++)
            {
                T obj = reader.Read<T>();
                objects.Add(obj);
            }

            // We will need to skip all these changes
            // the next time the list is synchronized
            // because they have already been applied
            changesAhead = (int)reader.ReadUInt();
        }

        public void OnDeserializeDelta(NetworkReader reader)
        {
            // This list can now only be modified by synchronization
            IsReadOnly = true;

            int changesCount = (int)reader.ReadUInt();

            for (int i = 0; i < changesCount; i++)
            {
                Operation operation = (Operation)reader.ReadByte();

                // apply the operation only if it is a new change
                // that we have not applied yet
                bool apply = changesAhead == 0;
                T item = default;

                switch (operation)
                {
                    case Operation.OP_ADD:
                        item = reader.Read<T>();
                        if (apply)
                        {
                            objects.Add(item);
                        }
                        break;

                    case Operation.OP_CLEAR:
                        if (apply)
                        {
                            objects.Clear();
                        }
                        break;

                    case Operation.OP_REMOVE:
                        item = reader.Read<T>();
                        if (apply)
                        {
                            objects.Remove(item);
                        }
                        break;
                }

                if (apply)
                {
                    Callback?.Invoke(operation, item);
                }
                // we just skipped this change
                else
                {
                    changesAhead--;
                }
            }
        }

        public bool Add(T item)
        {
            if (objects.Add(item))
            {
                AddOperation(Operation.OP_ADD, item);
                return true;
            }
            return false;
        }

        void ICollection<T>.Add(T item)
        {
            if (objects.Add(item))
            {
                AddOperation(Operation.OP_ADD, item);
            }
        }

        public void Clear()
        {
            objects.Clear();
            AddOperation(Operation.OP_CLEAR);
        }

        public bool Contains(T item) => objects.Contains(item);

        public void CopyTo(T[] array, int index) => objects.CopyTo(array, index);

        public bool Remove(T item)
        {
            if (objects.Remove(item))
            {
                AddOperation(Operation.OP_REMOVE, item);
                return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator() => objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == this)
            {
                Clear();
                return;
            }

            // remove every element in other from this
            foreach (T element in other)
            {
                Remove(element);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (other is ISet<T> otherSet)
            {
                IntersectWithSet(otherSet);
            }
            else
            {
                HashSet<T> otherAsSet = new HashSet<T>(other);
                IntersectWithSet(otherAsSet);
            }
        }

        void IntersectWithSet(ISet<T> otherSet)
        {
            List<T> elements = new List<T>(objects);

            foreach (T element in elements)
            {
                if (!otherSet.Contains(element))
                {
                    Remove(element);
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => objects.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => objects.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => objects.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => objects.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => objects.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => objects.SetEquals(other);

        // custom implementation so we can do our own Clear/Add/Remove for delta
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == this)
            {
                Clear();
            }
            else
            {
                foreach (T element in other)
                {
                    if (!Remove(element))
                    {
                        Add(element);
                    }
                }
            }
        }

        // custom implementation so we can do our own Clear/Add/Remove for delta
        public void UnionWith(IEnumerable<T> other)
        {
            if (other != this)
            {
                foreach (T element in other)
                {
                    Add(element);
                }
            }
        }
    }

    public class SyncHashSet<T> : SyncSet<T>
    {
        public SyncHashSet() : this(EqualityComparer<T>.Default) {}
        public SyncHashSet(IEqualityComparer<T> comparer) : base(new HashSet<T>(comparer ?? EqualityComparer<T>.Default)) {}

        // allocation free enumerator
        public new HashSet<T>.Enumerator GetEnumerator() => ((HashSet<T>)objects).GetEnumerator();
    }

    public class SyncSortedSet<T> : SyncSet<T>
    {
        public SyncSortedSet() : this(Comparer<T>.Default) {}
        public SyncSortedSet(IComparer<T> comparer) : base(new SortedSet<T>(comparer ?? Comparer<T>.Default)) {}

        // allocation free enumerator
        public new SortedSet<T>.Enumerator GetEnumerator() => ((SortedSet<T>)objects).GetEnumerator();
    }
}
