using UnityEngine;

public class FactionListener : FactionBaseListener
{

    [SerializeField] protected FactionCallback callback;

    void OnEnable()
    {
        this.target.Subscribe(this);
    }

    void OnDisable()
    {
        this.target.Unsubscribe(this);
    }

    public override void OnRaise(Faction data)
    {
    	this.callback.Invoke(data);
    }
}
