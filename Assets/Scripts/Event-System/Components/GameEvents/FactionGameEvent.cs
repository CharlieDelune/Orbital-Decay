using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for subscribable Scriptable Objects
[Serializable]
[CreateAssetMenu(menuName = "GameEvent/Faction")]
public class FactionGameEvent : ScriptableObject, ISubscribable<Faction>
{

	protected SortedSet<FactionBaseListener> listeners;

	protected virtual void Awake()
	{
		this.listeners = new SortedSet<FactionBaseListener>(
            Comparer<FactionBaseListener>.Create(
                (a, b) => a.CompareTo(b)
            )
        );
	}

    public virtual void Subscribe(IListener<Faction> listener)
    {
        if(this.listeners == null)
        {
            this.listeners = new SortedSet<FactionBaseListener>(
                Comparer<FactionBaseListener>.Create(
                    (a, b) => a.CompareTo(b)
                )
            );
        }
    	this.listeners.Add((FactionBaseListener)listener);
    }

    public virtual void Unsubscribe(IListener<Faction> listener)
    {
    	this.listeners.Remove((FactionBaseListener)listener);
    }

    public virtual void Raise(Faction data)
    {
        foreach(FactionBaseListener listener in this.listeners)
        {
            listener.OnRaise(data);
        }
    }
}
