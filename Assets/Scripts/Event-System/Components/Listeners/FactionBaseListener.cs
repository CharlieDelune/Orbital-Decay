using System;
using UnityEngine;

public abstract class FactionBaseListener : MonoBehaviour, IListener<Faction>, IComparable
{

    [SerializeField] protected FactionGameEvent target;
    public FactionGameEvent Target { get => this.target; }

    [SerializeField] protected int priority;
    public int Priority { get => this.priority; }

    void OnEnable()
    {
        this.target.Subscribe(this);
    }

    void OnDisable()
    {
        this.target.Unsubscribe(this);
    }

    public int CompareTo(object obj) {
        if(obj == null)
        {
            return 1;
        }
        FactionBaseListener other = obj as FactionBaseListener;
        if(this.Priority > other.Priority)
        {
            return -1;
        }
        return 1;
    }

    public abstract void OnRaise(Faction data);
}