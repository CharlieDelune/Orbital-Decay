using UnityEngine;
using UnityEngine.Events;

/// Listener that has a Callback that is invoked when the
/// subcribed MonoBehaviourGameEvent is raised
public class PlayerViewStateListener : BaseListener<PlayerViewState>
{

    public override void OnRaise(PlayerViewState data)
    {
    }
}
