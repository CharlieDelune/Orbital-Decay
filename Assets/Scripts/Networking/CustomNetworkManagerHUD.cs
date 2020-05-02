using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// Handles UI for players to get into a game

[RequireComponent(typeof(NetworkManager))]
public class CustomNetworkManagerHUD : MonoBehaviour
{
	public NetworkManager manager;
	[SerializeField] public bool showGUI = true;
	[SerializeField] public int offsetX;
	[SerializeField] public int offsetY;

	[SerializeField] private GUIStyle hudStyle;
	[SerializeField] private GUIStyle textFieldStyle;

	private SelectedGame selectedGame = SelectedGame.None;

	private bool showServer = false;
	private bool hostingGame = false;
	private bool noGameSelected = true;
	private bool singleplayer = false;
	private bool awaitingGame = false;
	private bool showMatches = false;
	private bool hasMatches = false;

	private enum SelectedGame
	{
		None,
		LAN,
		Online
	}

	private void Awake()
	{
		this.manager = GetComponent<NetworkManager>();
	}

	private void OnGUI()
	{

		if (!this.showGUI)
			return;

		int ypos = 40 + offsetY;
		int upperYPos = 40;
		int height = 100;
		int width = 400;
		int spacing = 120;

		if(this.noGameSelected)
		{
			if(GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Singleplayer", this.hudStyle))
			{
				this.manager.StartHost();
				this.noGameSelected = false;
				this.singleplayer = true;
				GameSession.Instance.IsSinglePlayer = true;
			}
			ypos += spacing;

			if(GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Multiplayer", this.hudStyle))
			{
				this.noGameSelected = false;
			}
			ypos += spacing;
		}
		else
		{
			if(!this.awaitingGame && !NetworkClient.active && !NetworkServer.active && this.manager.matchMaker == null)
			{
				if(this.selectedGame == SelectedGame.None)
				{

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "LAN Game", this.hudStyle))
					{
						this.selectedGame = SelectedGame.LAN;
					}
					ypos += spacing;

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Online Game", this.hudStyle))
					{
						this.selectedGame = SelectedGame.Online;
					}
					ypos += spacing;
				}
				else if(this.selectedGame == SelectedGame.LAN)
				{

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Start LAN Game", this.hudStyle))
					{
						this.hostingGame = true;
						this.awaitingGame = true;
						this.manager.StartHost();
					}
					ypos += spacing;

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Join LAN Game", this.hudStyle))
					{
						this.hostingGame = false;
						this.awaitingGame = true;
						this.manager.StartClient();
					}
					ypos += spacing;
				}
				else if(this.selectedGame == SelectedGame.Online)
				{

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Start Online Game", this.hudStyle))
					{
						this.hostingGame = true;
						this.awaitingGame = true;
					}
					ypos += spacing;

					if(GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Join Online Game", this.hudStyle))
					{
						this.hostingGame = false;
						this.awaitingGame = true;
					}
					ypos += spacing;
				}

				if(GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Back", this.hudStyle))
				{
					this.selectedGame = SelectedGame.None;
					this.noGameSelected = true;
					this.awaitingGame = false;
					this.hostingGame = false;
					this.showMatches = false;
					this.hasMatches = false;
				}
				ypos += spacing;
			}
			else
			{

				if(this.singleplayer && NetworkClient.active && ClientScene.ready)
				{
					this.manager.SetMatchHost("localhost", 1337, false);
				}

				if(this.selectedGame == SelectedGame.Online)
				{
					if(this.hostingGame)
					{
						this.displayOnlineHost(ref ypos, spacing, width, height);
					}
					else
					{
						this.displayOnlineJoin(ref ypos, spacing, width, height);
					}
				}

				if(NetworkServer.active || NetworkClient.active)
				{
					if(GUI.Button(new Rect(0, upperYPos, width, height), "Stop", this.hudStyle))
					{
						this.hostingGame = false;
						this.selectedGame = SelectedGame.None;
						this.noGameSelected = true;
						this.awaitingGame = false;
						this.showMatches = false;
						this.hasMatches = false;
						this.manager.StopHost();
						Application.LoadLevel(Application.loadedLevel);
					}
					upperYPos += spacing;
				}
			}

			if(NetworkClient.active && !ClientScene.ready && !this.hostingGame)
			{
				ClientScene.Ready(this.manager.client.connection);
			}

			if(NetworkServer.active)
			{
				GUI.Label(new Rect(0, upperYPos, width * 3, height), "Server: port=" + this.manager.networkPort, this.hudStyle);
				upperYPos += spacing;
			}
			if(NetworkClient.active)
			{
				if(this.hostingGame)
				{
					GUI.Label(new Rect(0, upperYPos, width * 3, height), "Players In Room=" + this.manager.numPlayers, this.hudStyle);
					upperYPos += spacing;
				}
				GUI.Label(new Rect(0, upperYPos, width * 3, height), "Client: address=" + this.manager.networkAddress + " port=" + this.manager.networkPort, this.hudStyle);
				upperYPos += spacing;
			}
		}
	}

	private void displayOnlineHost(ref int ypos, int spacing, int width, int height)
	{
		if (!NetworkServer.active && !NetworkClient.active)
		{
			ypos += 10;
			if (this.manager.matchMaker == null)
			{
				this.manager.StartMatchMaker();
			}
			else
			{
				if (this.manager.matchInfo == null)
				{
					if (this.manager.matches == null)
					{
						if (GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Start Match", this.hudStyle))
						{
							this.manager.matchMaker.CreateMatch(this.manager.matchName, this.manager.matchSize, true, "", "", "", 0, 0, this.manager.OnMatchCreate);
						}
						ypos += spacing;

						GUI.Label(new Rect(Screen.width / 2 - width * 2, ypos, width * 2, height), "Room Name:", this.hudStyle);
						this.manager.matchName = GUI.TextField(new Rect(Screen.width / 2, ypos, width * 2, height), this.manager.matchName, this.textFieldStyle);
						ypos += spacing;
					}
				}
				if (GUI.Button(new Rect(Screen.width / 2 - width * 3 / 2, ypos, width * 3, height), "Change MM server", this.hudStyle))
				{
					showServer = !showServer;
				}
				if (showServer)
				{
					ypos += spacing;
					if (GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Local", this.hudStyle))
					{
						this.manager.SetMatchHost("localhost", 1337, false);
						showServer = false;
					}
					ypos += spacing;
					if (GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Internet", this.hudStyle))
					{
						this.manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
						showServer = false;
					}
				}

				ypos += spacing;

				if(GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Back", this.hudStyle))
				{
					this.selectedGame = SelectedGame.None;
					if(this.manager.matchMaker != null)
					{
						this.manager.StopMatchMaker();
					}
					this.noGameSelected = true;
					this.awaitingGame = false;
					this.hostingGame = false;
				}
			}
		}
	}

	private void displayOnlineJoin(ref int ypos, int spacing, int width, int height)
	{
		
		if (!NetworkServer.active && !NetworkClient.active)
		{
			ypos += 10;

			if (this.manager.matchMaker == null)
			{
				this.manager.StartMatchMaker();
			}
			else
			{
				if (this.manager.matchInfo == null)
				{
					if(this.showMatches && !this.hasMatches)
					{
						GUI.Label(new Rect(Screen.width / 2 - width * 3 / 2, ypos, width * 3, height), "No matches found", this.hudStyle);
						ypos += spacing;
					}

					if (this.manager.matches == null)
					{

						ypos += 10;

						if (GUI.Button(new Rect(Screen.width / 2 - width * 3 / 2, ypos, width * 3, height), "Find Internet Match", this.hudStyle))
						{
							this.showMatches = true;
							this.manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, this.manager.OnMatchList);
						}
						ypos += spacing;
					}
					else
					{
						this.hasMatches = false;
						foreach (var match in this.manager.matches)
						{
							this.hasMatches = true;
							if (GUI.Button(new Rect(Screen.width / 2 - width, ypos, width * 2, height), "Join Match:" + match.name, this.hudStyle))
							{
								this.manager.matchName = match.name;
								this.manager.matchSize = (uint)match.currentSize;
								this.manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, this.manager.OnMatchJoined);
							}
							ypos += spacing;
						}
					}
				}

				ypos += spacing;

				GUI.Label(new Rect(Screen.width / 2 - width * 2, ypos, width * 4, height), "MM Uri: " + this.manager.matchMaker.baseUri, this.hudStyle);
				ypos += spacing;

				if(GUI.Button(new Rect(Screen.width / 2 - width / 2, ypos, width, height), "Back", this.hudStyle))
				{
					this.selectedGame = SelectedGame.None;
					if(this.manager.matchMaker != null)
					{
						this.manager.StopMatchMaker();
					}
					this.noGameSelected = true;
					this.awaitingGame = false;
					this.hostingGame = false;
				}
				ypos += spacing;
			}
		}
	}
}
