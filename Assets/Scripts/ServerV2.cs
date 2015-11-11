using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class ServerV2 : MonoBehaviour {
	
	public bool DisplayGUI = true;
	public string serverIp = "127.0.0.1";
	public int port = 2500;
	public string serverPass = "";
	public int MaxPlayers = 2;
	public GameObject[] PlayerShips;
	public Vector3 playerSpawn;
	public bool enableImageTransfer = false;
	public enum ServerState { Offline, PreGame, InGame, PostGame};
	public ServerState curState = ServerState.Offline;
	public static ServerV2 inst;
	public Texture2D DefaultIMG;
	//private
	private string curStatus = "Waiting for player input...";
	private bool usePass = false;
	
	private NetworkPeerType peerType;
	private string PlayerName = "player";
	private List<string> PlayerNames = new List<string>();
	private List<NetworkPlayer> NetPlayers = new List<NetworkPlayer>();
	private List<string> ServerLog = new List<string>();
	private bool isJoining = false;
	private bool isHosting = false;
	private bool showIP = false;
	private string curChatMsg = "";
	private Vector2 scroll = Vector2.zero;
	private bool hasSpawned = false;
	private bool noResponce = true;
	private GameObject player = null;
	private bool CheckPlayer = false;
	private bool showIMGTransferWindow = false;
	private Texture2D IMG;
	private bool loadImage = false;
	private string ImagePath = "C:/";
	private bool imageRecieved = false;
	private string IMGSender = "none";
	
	void Start()
	{
		Application.runInBackground = true;
		//Make sure there is a networkview component attached
		if(!GetComponent<NetworkView>())
		{
			gameObject.AddComponent<NetworkView>();
		}
		IMG = DefaultIMG;
		//save the current network peer type eg. server/client
		peerType = Network.peerType;
		inst = this;
	}
	void Update()
	{
		peerType = Network.peerType;
		//Check if the player has responded with the correct information before allowing movement
		if(CheckPlayer)
		{
			if(noResponce)
			{
				player.SendMessage("Information", PlayerName);
			}
			if(!noResponce)
			{
				player.SendMessage("AllowMovement");
			}
		}
		//manages the server chat log
		if(ServerLog.Count > 50)
			ServerLog.RemoveAt(0);
		//spawns the player
		if(!hasSpawned)
		{	
			if(curState == ServerState.InGame && peerType == NetworkPeerType.Client)
			{
				SpawnPlayer();
			}
		}
		//Debug.Log(hasSpawned + " : " + curState + " : " + peerType);
	}
	//Report the player's death info to the server
	public void ReportPlayerDeath(string reason, string killer)
	{
		if(reason == "Fall")
		{
			AddChatMSG(PlayerName + " fell off the track.");
		}
		else if(reason =="Killed" && killer != "-none-")
		{
			if(killer != PlayerName)
				AddChatMSG(PlayerName + " was killed by " + killer);
			else
				AddChatMSG(PlayerName + " made a mistake.");
		}else
		{
			AddChatMSG(PlayerName + " died somehow.");
		}
	}
	void OnGUI()
	{
		//displays the server GUI
		if(DisplayGUI)
		{
			GUILayout.Label("Server State: " + curState);
			if(peerType == NetworkPeerType.Disconnected)
				GUILayout.Label("Currently: " + curStatus +", " + peerType);
			if(peerType != NetworkPeerType.Disconnected && peerType != NetworkPeerType.Connecting)
			{
				GUILayout.Label("Currently: " + curStatus +", " + peerType);
				if(showIP)
					GUILayout.Label("Server IP:" + Network.player.ipAddress +" | " + Network.player.externalIP);
					
			}
			//Disconnected host/join
			if(peerType == NetworkPeerType.Disconnected)
			{
				//Choose to host or join a server
				if(!isJoining && !isHosting)
				{
					if(GUILayout.Button("Join a Server"))
					{
						isJoining = true;
					}
					if(GUILayout.Button("Host a Server"))
					{
						isHosting = true;
					}
				}
				//toggle wether to show the server's ip or not.
				showIP = GUILayout.Toggle(showIP, "Show IP");
				//Display the Join Server GUI
				if(isJoining)
				{
					GUILayout.Label("Player name:");
					PlayerName = GUILayout.TextField(PlayerName);//Player Name
					GUILayout.Label("Server IP:");
					serverIp = GUILayout.TextField(serverIp);//Server to Connect to
					GUILayout.Label("Server Port:");
					port = Convert.ToInt32(GUILayout.TextField(port.ToString()));//Server port
					GUILayout.Label("Server Password: (leave blank if none)");
					serverPass = GUILayout.PasswordField(serverPass, '*');//Server password if any
					GUI.enabled = CheckInfo();
					if(GUILayout.Button("Connect"))
					{
						if(serverPass.Length > 0)
							JoinServer(serverIp, port, serverPass);
						else
							JoinServer(serverIp, port);
					}
					GUI.enabled = true;
					if(GUILayout.Button("Cancel"))//return to host/join screen
					{
						isJoining = false;
					}
				}
				if(isHosting)//display host server screen
				{
					GUILayout.Label("Server Port:");
					port = Convert.ToInt32(GUILayout.TextField(port.ToString()));//Server port to host
					GUILayout.Label("Max Players: " + MaxPlayers);//Maximum number of connections to server
					MaxPlayers = (int)GUILayout.HorizontalSlider(MaxPlayers, 1, 20);
					usePass = GUILayout.Toggle(usePass, "Password");//password for server
					if(usePass)
					{
						GUILayout.Label("Server Password:");
						serverPass = GUILayout.PasswordField(serverPass, '*');
					}
					GUI.enabled = CheckInfo();//Checks the valilidity of the info entered
					if(GUILayout.Button("Start Server"))//Host the server
					{
						if(usePass)
							StartServer(port, MaxPlayers, serverPass);
						else
							StartServer(port, MaxPlayers);
					}
					GUI.enabled = true;
					if(GUILayout.Button("Cancel"))//Returnt to host/join screen
					{
						isHosting = false;
					}
				}
			}
			if(peerType == NetworkPeerType.Server)//Has connected and is the server
			{
				GUILayout.Label("Send Rate: " + Network.sendRate);
				Network.sendRate = GUILayout.HorizontalSlider( Network.sendRate, 10, 500);
				GUILayout.Label("Current Players: " + NetPlayers.Count);
				
				foreach(string p in PlayerNames)//Display the player list
				{
					GUILayout.BeginHorizontal();
					if(showIP)
						GUILayout.Label(p + " " + NetPlayers[PlayerNames.IndexOf(p)].ipAddress+":"+NetPlayers[PlayerNames.IndexOf(p)].port);
					else
						GUILayout.Label(p);
					GUILayout.Label(" Ping: " + Network.GetAveragePing(NetPlayers[PlayerNames.IndexOf(p)]));
					if(GUILayout.Button("Kick"))
					{
						KickPlayer(p);
					}
					GUILayout.EndHorizontal();
				}
				//Server State
				if(curState == ServerState.PreGame)
				{
					if(GUILayout.Button("Start Game"))
					{
						NextState();
					}
				}
				if(curState == ServerState.InGame)
				{
					if(GUILayout.Button("End Game"))
					{
						NextState();
					}
				}
				if(curState == ServerState.PostGame)
				{
					if(GUILayout.Button("Restart Game"))
					{
						NextState();
					}
				}
				if(GUILayout.Button("Close Server"))
				{
					Network.Disconnect();
					CleanUp();
				}
			}
			if(peerType == NetworkPeerType.Client)//is connected and is client
			{
				GUILayout.Label("Current Players: " + PlayerNames.Count);
				foreach(string p in PlayerNames)//player list
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(p);
					GUILayout.EndHorizontal();
				}
				if(GUILayout.Button("Disconnect"))
				{
					Network.Disconnect();
				}
			}
			if(peerType == NetworkPeerType.Client || peerType == NetworkPeerType.Server)//is connect and is server or client
			{
				if(enableImageTransfer)//image transfer stuff
				{
					if(GUILayout.Button("Send Image"))
					{
						showIMGTransferWindow = !showIMGTransferWindow;
					}
					if(showIMGTransferWindow)
					{
						DisplayImageTranferDialog(new Rect(Screen.width/2 - 250, Screen.height/2 - 200, 500, 400));
					}
				}
				DisplayChat(new Rect(0, Screen.height-210, 500, 200));//display chat area
			}
		}
	}
	public void DisplayImageTranferDialog(Rect rect)
	{
		GUI.Window(0, rect, TransferWindow, "Image Transfer");
	}
	void TransferWindow(int windowID)//Image Transfer window
	{
		if(!imageRecieved)
		{
			GUI.DrawTexture(new Rect(500/2-128, 20, 256, 256), IMG, ScaleMode.ScaleToFit);
			if(GUI.Button(new Rect(500/2-130, 286, 120, 20), "Take ScreenShot"))
			{
				CaptureScreen();
			}
			if(loadImage)
			{
				ImagePath = GUI.TextField(new Rect(500/2-128, 316, 256, 20), ImagePath);
				if(GUI.Button(new Rect(500/2-128, 346, 256, 20), "Load"))
				{
					LoadImageFile();
					Debug.Log("Clicked");
					loadImage = false;
				}
				
			}
			if(GUI.Button(new Rect(500/2-60+70, 286, 120, 20), "Load Image"))
			{
				loadImage = !loadImage;
			}
			if(!loadImage)
			{
				if(GUI.Button(new Rect(500/2-128, 346, 256, 20), "Send"))
				{
					networkView.RPC("RecieveIMG", RPCMode.Others, IMG.EncodeToPNG(), PlayerName);//Send image to all others
				}
			}
		}else
		{
			GUI.Label(new Rect(500/2-128, 20, 256, 20), "The Following Image Was Recieved from: " + IMGSender);
			GUI.DrawTexture(new Rect(500/2-128, 50, 256, 256), IMG, ScaleMode.ScaleToFit);
			if(GUI.Button(new Rect(500/2-128, 346, 256, 20), "Back"))
			{
				imageRecieved = false;
				IMG = DefaultIMG;
			}
		}
		if(GUI.Button(new Rect(500/2-128, 376, 256, 20), "Close"))
		{
			showIMGTransferWindow = false;
		}
		
	}
	[RPC]
	public void RecieveIMG(Byte[] img, string sender)//recives image and displays it
	{
		IMG.LoadImage(img);
		IMGSender = sender;
		showIMGTransferWindow = true;
		imageRecieved = true;
	}
	IEnumerator LoadImageFile()//Load an image, doesn't work
	{
		Debug.Log("Loading Image");
		//ImagePath = "file:///" + ImagePath;
		WWW image = new WWW(ImagePath);
		yield return image;
		IMG = image.texture;
	}
	IEnumerator CaptureScreen()//Takes  a screenshot, sortof works
	{
		yield return new WaitForEndOfFrame();
		Texture2D screenCap = new Texture2D(Screen.height, Screen.width, TextureFormat.RGB24, false);
		screenCap.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 1, 1);
		screenCap.Apply();
		IMG = screenCap;
		
	}
	public void DisplayChat(Rect pos)//displays the chat area
	{
		GUILayout.BeginArea(pos);
		GUILayout.BeginHorizontal();
		GUILayout.Space( 5.0F );
		GUILayout.BeginVertical();
		GUILayout.Space( 5.0F );
		scroll = GUILayout.BeginScrollView(scroll);
		foreach(string l in ServerLog)//All the chat items
		{
			GUILayout.Label(l);
		}
		GUILayout.EndScrollView();
		GUILayout.Space( 10.0f);
		GUILayout.EndVertical();
		GUILayout.Space( 0.0f);
		GUILayout.EndHorizontal();
		if(curState != ServerState.InGame || peerType == NetworkPeerType.Server)
		{
			curChatMsg = GUILayout.TextField(curChatMsg);
			if(Event.current.type == EventType.keyDown && Event.current.character == '\n' && curChatMsg.Length > 0 )
			{
				AddServerChat(PlayerName + ": " + curChatMsg);//sends messages
				curChatMsg = "";
				scroll.y = 100000;
			}
		}
		GUILayout.EndArea();
	}
	public void JoinServer(string serverIp, int port)//Joins server without password
	{
		curStatus = "connecting to " + serverIp+":"+port;
		Network.Connect(serverIp, port);
	}
	public void JoinServer(string serverIp, int port, string password)//Joins server with password
	{
		curStatus = "connecting to " + serverIp+":"+port;
		Network.Connect(serverIp, port, password);
	}
	public void StartServer(int serverPort, int maxPlayers)//Starts server without password
	{
		curStatus = "Starting server";
		Network.incomingPassword = "pass";
		Network.InitializeServer(MaxPlayers, port, !Network.HavePublicAddress());
	}
	public void StartServer(int serverPort, int maxPlayers, string password)//Starts server with password
	{
		curStatus = "Starting server";
		Network.incomingPassword = password;
		Network.InitializeServer(MaxPlayers, port, !Network.HavePublicAddress());
	}
	public void KickPlayer(string name)//Kicks a specified player
	{
		AddServerLog(name + " was kicked from the game.");
		Network.CloseConnection(NetPlayers[PlayerNames.IndexOf(name)], true);
	}
	public void NextState()//Changes the server state
	{
		switch(curState)
		{
			case ServerState.PreGame:
			{
				curState = ServerState.InGame;
				networkView.RPC("UpdateServerState", RPCMode.AllBuffered, (int)curState);
				AddServerLog("Game starting...");
				break;
			}
			case ServerState.InGame:
			{
				curState = ServerState.PostGame;
				networkView.RPC("UpdateServerState", RPCMode.AllBuffered, (int)curState);
				AddServerLog("Game ending...");
				break;
			}
			case ServerState.PostGame:
			{
				curState = ServerState.PreGame;
				networkView.RPC("UpdateServerState", RPCMode.AllBuffered, (int)curState);
				AddServerLog("Returning to lobby...");
				break;
			}
		}
	}
	void OnPlayerConnected( NetworkPlayer player)//Handles the connecting player
	{
		networkView.RPC("UpdateServerState", RPCMode.AllBuffered, (int)curState);//sends the currect server state to the player
		
		if(peerType == NetworkPeerType.Client)
		{
			PlayerNames.Clear();//Clear player list
		}
	}
	void OnDisconnectedFromServer( NetworkDisconnection error)//Handles players losing connection to the server
	{
		curStatus = "Lost connection: "+ error;//displays error if any
		camera.enabled = true;
		gameObject.GetComponent<AudioListener>().enabled = true;
		CleanUp();//cleanup
	}
	void CleanUp()
	{
		curState = ServerState.Offline;
		PlayerNames.Clear();
		ServerLog.Clear();
		NetPlayers.Clear();
		curStatus = "waiting for input...";
		hasSpawned = false;
		isHosting = false;
		isJoining = false;
		camera.tag = null;
		noResponce = true;
		player = null;
		foreach(NetworkPlayer np in NetPlayers)
		{
			Network.RemoveRPCs(np);
		}
	}
	void OnFailedToConnect( NetworkConnectionError error)//handles connection failure
	{
		curStatus = "Failed to connect: " + error;
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)//Handles players leaving the server
	{
		if(peerType == NetworkPeerType.Server && NetPlayers.Contains(player))
		{
			AddServerLog(PlayerNames[NetPlayers.IndexOf(player)] + " left the game.");//logs player leaving in chat
			PlayerNames.RemoveAt(NetPlayers.IndexOf(player));//removes player from list
			NetPlayers.Remove(player);//removes player's net info
			Network.DestroyPlayerObjects(player);//destroys everything that belongs to this player
			Network.RemoveRPCs(player);//remove this player's RPC calls
		}
		if(peerType == NetworkPeerType.Server)
			SendPlayerList();//sends new player list to clients
	}
	
	void OnConnectedToServer()//Sucessfully connected to the server
	{
		foreach(GameObject go in FindObjectsOfType(typeof(GameObject)))
		{
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}
		networkView.RPC("PlayerInfo", RPCMode.Server, PlayerName, Network.player);//Sends player info to the server
		if(showIP)
			curStatus = "Connected to: " + Network.player.ipAddress+":"+Network.player.port;
		else
			curStatus = "Connected to server";
	}
	void OnServerInitialized()//Server started successfully
	{
		foreach(GameObject go in FindObjectsOfType(typeof(GameObject)))
		{
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
		}
		camera.tag = "MainCamera";
		PlayerName = "SERVER";
		curState = ServerState.PreGame;
		curStatus = "Server started, Waiting for connections...";
	}
	bool CheckInfo()//Checks the player info validity
	{
		if(PlayerName.Contains(" ") || PlayerName == "player" && isJoining)
			return false;
		else if(serverIp.Contains(" ") || port <= 0 || serverPass.Contains(" "))
		{
			return false;
		}else
			return true;
	}
	void SendPlayerList()//Sends player list to all connected users
	{
		networkView.RPC("UpdatePlayerList", RPCMode.AllBuffered, BinarySerialize(PlayerNames));
	}
	string BinarySerialize(List<string> list)//conversion of List<> to a string to be send via RPC
	{
		MemoryStream stream = new MemoryStream();
		BinaryFormatter bFormat = new BinaryFormatter();
		bFormat.Serialize(stream, list);
		string ListData = Convert.ToBase64String(stream.GetBuffer());
		return ListData;
	}
	List<string> BinaryDeserialize(string listdata)//converting the string recieved back to a List<>
	{
		BinaryFormatter bFormat = new BinaryFormatter();
		MemoryStream ListData = new MemoryStream(Convert.FromBase64String(listdata));
		return (List<string>)bFormat.Deserialize(ListData);
	}
	void AddServerChat(string msg)//Adds a message to the chat log as player chat
	{
		msg = msg.Replace("\n", "");
		networkView.RPC("AddChatMSG", RPCMode.All, msg);//sends the message
	}
	void AddServerLog(string log)//Adds a message to the chat los as server log
	{
		ServerLog.Add("> " + log);
		scroll.y = 100000;
	}
	void SpawnPlayer()
	{
		//SpawnPlayers
		Debug.Log("Spawning Player");
		camera.enabled = false;
		gameObject.GetComponent<AudioListener>().enabled = false;
		player = Network.Instantiate(PlayerShips[UnityEngine.Random.Range(0, PlayerShips.Length)], new Vector3(playerSpawn.x + UnityEngine.Random.Range(-3, 3), playerSpawn.y, playerSpawn.z) , Quaternion.identity, 1) as GameObject;
		player.SendMessage("Information", PlayerName);
		CheckPlayer = true;
		AddServerChat(PlayerName + " has spawned");
		hasSpawned = true;
		
	}
	public List<string> GetPlayers()//returns player list
	{
		return PlayerNames;
	}
	public void Respond()//Respond to the server
	{
		noResponce = false;
	}
	[RPC]
	void AddChatMSG(string msg)//RPC adds the recieved chat message to chat log
	{
		ServerLog.Add("> " + msg);
		scroll.y = 100000;
	}
	[RPC]
	void PlayerInfo( string name, NetworkPlayer player)//Recives player info
	{
		if(curState == ServerState.PreGame)//allow player to join if preGame
		{
			PlayerNames.Add(name);
			NetPlayers.Add(player);
			SendPlayerList();
			AddChatMSG(name + " joined the game.");
			networkView.RPC("UpdateServerState", RPCMode.AllBuffered, (int)curState);
		}else
		{
			Network.CloseConnection(player, true);
		}
	}
	[RPC]
	void UpdatePlayerList( string list)//RPC update player list gets called be the network
	{
		if(peerType == NetworkPeerType.Client)
		{
			PlayerNames = BinaryDeserialize(list);
		}
	}
	[RPC]
	void UpdateServerState(int state)//RPC update server state
	{
		if(peerType == NetworkPeerType.Client)
		{
			curState = (ServerState)state;
			if((ServerState)state != ServerState.InGame)
				camera.enabled = true;
		}
	}
//	[RPC]
//	void UpdateServerLog(string log)
//	{
//		if(peerType == NetworkPeerType.Client)
//		{
//			ServerLog = BinaryDeserialize(log);
//			scroll.y = 100000;
//		}
//	}
}
