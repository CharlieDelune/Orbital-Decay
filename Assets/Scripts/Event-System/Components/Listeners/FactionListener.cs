using UnityEngine;
using UnityEngine.Events;

/// Listener that has a Callback that is invoked when the
/// subcribed FactionGameEvent is raised
public class FactionListener : BaseListener<Faction>
{

    [SerializeField] protected FactionCallback callback;

    public override void OnRaise(Faction data)
    {
        this.callback.Invoke(data);
    }
}
