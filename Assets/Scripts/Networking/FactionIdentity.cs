using UnityEngine;
using UnityEngine.Networking;

/// Serves as an identifier for Factions
/// Required to be NetworkBehaviour to
/// run server and client code together
public class FactionIdentity : NetworkBehaviour
{

	private Faction faction;
	public Faction Faction { get => this.faction; }

	public bool IsPlayer = false;

	[SerializeField] private GameObject gameWrapPrefab;

	public void SetFaction(Faction _faction)
	{
		this.faction = _faction;
		this.Faction.SetIdentity(this);
	}

	/// Connected to a FactionListener that listens to the
	/// end of a turn
	public void OnRaiseEndTurnGameEvent(Faction faction)
	{
		/// Only sends the message if the faction is the local player
		if(this.isLocalPlayer)
		{
			if(!NetworkEventManager.Instance.BlockEndTurnTransmission)
			{
				if(faction != null)
				{
					this.CmdRaiseEndTurnGameEvent(faction.Index);
				}
			}
			else
			{
				NetworkEventManager.Instance.BlockEndTurnTransmission = false;
			}
		}
	}

	[Command]
	public void CmdTryPerformAction(SerializedData data)
	{
		this.RpcTryPerformAction(data);
	}

	[ClientRpc]
	public void RpcTryPerformAction(SerializedData data)
	{
		if(!this.isLocalPlayer)
		{
			NetworkEventManager.Instance.AddToQueue(
				new NetworkEvent(this.Faction, data)
			);
		}
	}

	[Command]
	public void CmdRaiseEndTurnGameEvent(int factionIndex)
	{
		this.RpcReceiveEndTurnGameEvent(factionIndex);
	}

	/// Loads the GameWrap prefab into the scene
	[ClientRpc]
	public void RpcLoadGame()
	{
		if(this.isLocalPlayer)
		{
			Instantiate(this.gameWrapPrefab);
		}
	}

	[ClientRpc]
	public void RpcReceiveEndTurnGameEvent(int factionIndex)
	{
		if(!this.isLocalPlayer)
		{
			NetworkEventManager.Instance.AddToQueue(
				new NetworkEvent(GameStateManager.Instance.Factions[factionIndex], new SerializedData(null, null, -1, "turn", this.Faction.Index))
			);
		}
	}

	/// Initiates setup in clients, seed is required
	[ClientRpc]
	public void RpcBeginGame(int seed)
	{
		if(this.isLocalPlayer)
		{
			Tutorial[] tutorials = FindObjectsOfType<Tutorial>();
			if(tutorials.Length > 0 && !tutorials[0].Used)
			{
				tutorials[0].BeginGame();
			}
			NetworkEventManager.Instance.Setup();
			GameSession.Instance.BeginGame(seed);
		}
	}
}