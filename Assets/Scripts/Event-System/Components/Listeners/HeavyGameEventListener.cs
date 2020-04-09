using UnityEngine;

public class HeavyGameEventListener : MonoBehaviour, IListener<HeavyGameEventData>
{

    [SerializeField] public HeavyGameEvent target;
    [SerializeField] protected HeavyGameEventCallback callback;

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

    public void OnRaise(HeavyGameEventData data)
    {
    	this.callback.Invoke(data);
    }
}
