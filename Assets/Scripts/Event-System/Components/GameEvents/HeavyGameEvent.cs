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
    protected HeavyGameEventListener[] listenersCache;

    private bool modifiedSinceLastRaise = true;

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
        this.modifiedSinceLastRaise = true;
    }

    public virtual void Unsubscribe(IListener<HeavyGameEventData> listener)
    {
    	this.listeners.Remove((HeavyGameEventListener)listener);
        this.modifiedSinceLastRaise = true;
    }

    public virtual void Raise(HeavyGameEventData data)
    {
        /// Necessary in case this.listeners gets modified during the execution of the OnRaise methods
        HeavyGameEventListener[] listenersCopy;
        if(this.modifiedSinceLastRaise)
        {
            this.listenersCache = new HeavyGameEventListener[this.listeners.Count];
            this.listeners.CopyTo(this.listenersCache);
        }
        listenersCopy = this.listenersCache;

        foreach(HeavyGameEventListener listener in listenersCopy)
        {
            listener.OnRaise(data);
        }
    }
}
