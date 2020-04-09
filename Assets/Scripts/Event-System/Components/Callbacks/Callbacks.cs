using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FactionCallback : UnityEvent<Faction>
{}

[Serializable]
public class MonoBehaviourCallback : UnityEvent<MonoBehaviour>
{}

[Serializable]
public class HeavyGameEventCallback : UnityEvent<HeavyGameEventData>
{}