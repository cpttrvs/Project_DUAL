using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //singleton
    public static GameManager instance;

    private const string PLAYER_ID_PREFIX = "Player ";

    public MatchSettings matchSettings;

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
   
	// Use this for initialization
	void Start () {
		if(instance != null) { Debug.LogError("GameManager : singleton error"); }
        else { instance = this; }
	}

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerID = PLAYER_ID_PREFIX + netID;
        players.Add(playerID, player);
        player.transform.name = playerID;
        player.SetTeam(1);
    }

    public static void UnregisterPlayer(string playerID)
    {
        if(players.ContainsKey(playerID))
        {
            players.Remove(playerID);
        } else
        {
            Debug.LogError("GameManager : unregistering unknown player");
        }
    }

    public static Player GetPlayer(string playerID) { return players[playerID]; }
}
