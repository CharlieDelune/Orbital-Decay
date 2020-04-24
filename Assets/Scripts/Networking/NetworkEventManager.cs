using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// Connects client to server and server to client

/// Keeps a queue of selectable actions performed by players
/// As well as end turn signaling for remote PlayerFactions
public class NetworkEventManager : MonoBehaviour
{

	private static NetworkEventManager _instance;

	public static NetworkEventManager Instance { get { return _instance; } }

	private uint localPlayerNetId;
	public uint LocalPlayerNetId { get => this.localPlayerNetId; }

	[SerializeField] private FactionGameEvent onEndTurnEvent;

	[HideInInspector] public FactionIdentity LocalIdentity;
	public int LocalIndex { get => this.LocalIdentity.Faction.Index; }

	private List<NetworkEvent> networkEventQueue = new List<NetworkEvent>();

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple NetworkEventManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}

	public void Setup()
	{

		FactionIdentity[] identities = FindObjectsOfType<FactionIdentity>();
		GameSession.Instance.SetIdentities(identities);

		for(int i = 0; i < identities.Length; i++)
		{
			if(identities[i].isLocalPlayer)
			{
				this.LocalIdentity = identities[i];
				break;
			}
		}

		this.localPlayerNetId = this.LocalIdentity.netId.Value;
	}

	public void TryPerformAction(Selectable selectable, SelectableActionType selectedAction, GridCell targetCell, string param)
	{
		SerializedData data = new SerializedData(selectable.ParentCell, targetCell, (int)selectedAction, param, this.LocalIndex);
		this.LocalIdentity.CmdTryPerformAction(data);
	}

	public void AddToQueue(NetworkEvent networkEvent)
	{
		this.networkEventQueue.Add(networkEvent);
	}

	public bool BlockEndTurnTransmission = false;

	private void Update()
	{
		if(this.networkEventQueue.Count > 0)
		{
			/// Only performs action if the action's Faction matches the current Faction in play
			if(!GameStateManager.Instance.AnimationPresent
				&& GameStateManager.Instance.NextTurn > 0
				&& GameStateManager.Instance.NextTurn - 1 == this.networkEventQueue[0].SourceFaction.Index)
			{
				NetworkEvent currentEvent = this.networkEventQueue[0];
				this.networkEventQueue.RemoveAt(0);

				/// Ends the turn
				if(currentEvent.Data.StringValue == "turn")
				{
					this.BlockEndTurnTransmission = true;
					GameStateManager.Instance.Factions[currentEvent.SourceFaction.Index].EndTurn();
				}
				/// Performs a selectable action
				else
				{

					/// Deserealization

					SerializedData data = currentEvent.Data;
					Selectable selectable;
					if(data.SourceCellPlanetIndex > -2)
					{
						CircularGrid grid;
						if(data.SourceCellPlanetIndex == -1)
						{
							grid = GameStateManager.Instance.solarSystemGrid;
						}
						else
						{
							grid = PlanetManager.Instance.planets[data.SourceCellPlanetIndex].grid;
						}
						selectable = grid.GetGridCell(data.SourceCellLayer, data.SourceCellSlice).Selectable;
					}
					else
					{
						selectable = null;
					}

					GridCell targetCell;

					if(data.TargetCellPlanetIndex > -2)
					{
						CircularGrid grid;
						if(data.TargetCellPlanetIndex == -1)
						{
							grid = GameStateManager.Instance.solarSystemGrid;
						}
						else
						{
							grid = PlanetManager.Instance.planets[data.TargetCellPlanetIndex].grid;
						}
						targetCell = grid.GetGridCell(data.TargetCellLayer, data.TargetCellSlice);
					}
					else
					{
						targetCell = null;
					}

					SelectableActionType selectedAction = (SelectableActionType) data.ActionType;
					selectable.TryPerformAction(selectedAction, targetCell, data.StringValue);
				}
			}
		}
	}
}

public struct NetworkEvent
{
	public Faction SourceFaction;
	public SerializedData Data;

	public NetworkEvent(Faction sourceFaction, SerializedData data)
	{
		this.SourceFaction = sourceFaction;
		this.Data = data;
	}
}