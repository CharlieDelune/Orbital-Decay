using UnityEngine;
using UnityEngine.Events;

/// Listener that has a Callback that is invoked when the
/// subcribed MonoBehaviourGameEvent is raised
public class MonoBehaviourListener : BaseListener<MonoBehaviour>
{

    [SerializeField] private MonoBehaviourCallback callback;

    public override void OnRaise(MonoBehaviour data)
    {
        this.callback.Invoke(data);
    }
}
