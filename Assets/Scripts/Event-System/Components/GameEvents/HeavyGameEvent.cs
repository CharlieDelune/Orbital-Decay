using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for subscribable Scriptable Objects
[Serializable]
[CreateAssetMenu(menuName = "GameEvent/Heavy")]
public class HeavyGameEvent : ScriptableObject, ISubscribable<HeavyGameEventData>
{

	protected SortedSet<HeavyGameEventListener> listeners;

	protected virtual void Awake()
	{
		this.listeners = new SortedSet<HeavyGameEventListener>(
            Comparer<HeavyGameEventListener>.Create(
                (a, b) => a.CompareTo(b)
            )
        );
	}

    public virtual void Subscribe(IListener<HeavyGameEventData> listener)
    {
        if(this.listeners == null)
        {
            this.listeners = new SortedSet<HeavyGameEventListener>(
                Comparer<HeavyGameEventListener>.Create(
                    (a, b) => a.CompareTo(b)
                )
            );
        }
    	this.listeners.Add((HeavyGameEventListener)listener);
    }

    public virtual void Unsubscribe(IListener<HeavyGameEventData> listener)
    {
    	this.listeners.Remove((HeavyGameEventListener)listener);
    }

    public virtual void Raise(HeavyGameEventData data)
    {
        foreach(HeavyGameEventListener listener in this.listeners)
        {
            listener.OnRaise(data);
        }
    }
}
