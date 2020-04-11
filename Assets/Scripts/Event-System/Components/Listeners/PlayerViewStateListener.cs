using UnityEngine;
using UnityEngine.Events;

/// Listener that has a Callback that is invoked when the
/// subcribed MonoBehaviourGameEvent is raised
public class PlayerViewManagerListener : BaseListener<PlayerViewManager>
{

    public override void OnRaise(PlayerViewManager data)
    {
    }
}
