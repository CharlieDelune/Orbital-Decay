using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// Replaces EntryPoint class

/// Separate from the GameStateManager because it has network-specific functionality
/// Handles pre-game state and multiplayer lobby

/// This is where match settings would likely be set
public class GameSession : NetworkManager
{

	private static GameSession _instance;

	public static GameSession Instance { get { return _instance; } }

	private List<FactionIdentity> identities = new List<FactionIdentity>();
	public List<FactionIdentity> Identities { get => this.identities; }

	// [SerializeField] private GameObject gameWrapPrefab;
	private LevelCreator levelCreator;

	public bool IsSinglePlayer = false;

	[SerializeField] private FactionIdentity AIFactionPrefab;

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple GameSession instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}

	// public void OnStartClient()
	// {
	// 	this.GetComponent<NetworkManagerHUD>().enabled = false;
	// }

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		GameObject playerObject = (GameObject)Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, playerObject, playerControllerId);

        FactionIdentity player = playerObject.GetComponent<FactionIdentity>();

        /// only true for initial player
        if(FindObjectsOfType<FactionIdentity>().Length == 1)
        {
        	this.CanStart = true;
        }
        if(this.CanStart)
        {
        	this.identities.Add(player);
        }

        player.RpcLoadGame();

        if(this.IsSinglePlayer)
        {
        	this.GetComponent<CustomNetworkManagerHUD>().enabled = false;
        }
	}

	public bool CanStart = false;

	public void Ready()
	{
		/// Only the initial player may start the game
		if(this.CanStart)
		{

			int levelCreatorSeed = FindObjectsOfType<LevelCreator>()[0].Seed;
			/// Selects game seed
			int seed = (levelCreatorSeed != -1) ? levelCreatorSeed : new System.Random().Next(999999999);

			/// All clients need to begin the game semi-simultaneously

			for(int i = 0; i < this.identities.Count; i++)
			{
				this.identities[i].RpcBeginGame(seed);
			}
			this.CanStart = false;
		}
	}

	public void BeginGame(int seed)
	{
		/// Where AI Factions are added
		this.GetComponent<CustomNetworkManagerHUD>().enabled = false;
        for(int i = 0; i < 2; i++)
        {
           	this.identities.Add(Instantiate(this.AIFactionPrefab).GetComponent<FactionIdentity>());
        }
		StartCoroutine(this.delayedStart(1.0f, seed));
	}

	public void SetIdentities(FactionIdentity[] _identities)
	{
		this.identities = new List<FactionIdentity>(_identities);
	}

	private IEnumerator delayedStart(float time, int seed)
	{
		this.levelCreator = FindObjectsOfType<LevelCreator>()[0];
		yield return new WaitForSeconds(time);
		this.levelCreator.StartLevelCreation(seed);
		yield return new WaitForSeconds(time);
		GameStateManager.Instance.StartGame();
	}
}