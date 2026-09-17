using NUnit.Framework;
using UnityEngine;

public interface IStatus<T>
{
    public T SetCurrentStatus(T newStatus);
}
