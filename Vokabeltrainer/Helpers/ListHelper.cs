using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.Helpers;

public static class ListHelper
{
    static readonly object _listLock = new object();
    
    public static void AddListEntry<TList, T>(TList data, T entry, bool onTop = true)
        where T : IEntity
        where TList : IList<T>
    {
        lock (_listLock)
        {
            if (onTop)
                data.Insert(0, entry);
            else
                data.Add(entry);
        }
    }

    public static void DeleteListEntry<TList, T>(TList data, T entry)
        where T : IEntity
        where TList : ICollection<T>
    {
        lock (_listLock)
        {
            T? oldEntity = data.FirstOrDefault(x => x.Id == entry.Id);
            if (oldEntity != null)
                data.Remove(oldEntity);
        }
    }

    public static void UpdateListEntry<TList, T>(TList data, T newEntity)
        where T : IEntity
        where TList : IList<T>
    {
        lock (_listLock)
        {
            T? oldEntity = data.FirstOrDefault(x => x.Id == newEntity.Id);
            if (oldEntity != null)
            {
                int pos = data.IndexOf(oldEntity);
                if (pos != -1)
                {
                    data.RemoveAt(pos);
                    data.Insert(pos, newEntity);
                }
            }
            else
                data.Add(newEntity);
        }
    }
}