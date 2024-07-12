using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Gamemode
{
    VsAI,
    COOP,
    Secret
}

[Serializable]
public class MatchHistory
{
    public List<Match> matches;
    public Dictionary<string, List<string>> players;

    public MatchHistory()
    {
        matches = new();
        players = new();
    }
}

public static class GameManager
{
    static readonly string savePath = Application.persistentDataPath + "/matches.json";
    static readonly MatchHistory matchHistory = LoadMatchHistory();

    static public Gamemode gamemode;
    static public bool aiFirst;
    static public bool swapStarting;
    static public string PlayerOneName = "Player 1";
    static public string PlayerTwoName = "Player 2";


    public static void Win(Player winner, Player loser)
    {
        Match match = new(false, winner, loser);
        matchHistory.matches.Add(match);

        //Add win for winner
        if (matchHistory.players.ContainsKey(winner.name))
        {
            matchHistory.players[winner.name].Add(match.id);
        }
        else
        {
            matchHistory.players.Add(winner.name, new() { match.id });
        }

        //Add loss for loser
        if (matchHistory.players.ContainsKey(loser.name))
        {
            matchHistory.players[loser.name].Add(match.id);
        }
        else
        {
            matchHistory.players.Add(loser.name, new() { match.id });
        }

        SaveMatchHistory();
    }

    public static void Tie(Player playerOne, Player playerTwo)
    {
        Match match = new(true, playerOne, playerTwo);
        matchHistory.matches.Add(match);

        //Add tie for player 1
        if (matchHistory.players.ContainsKey(playerOne.name))
        {
            matchHistory.players[playerOne.name].Add(match.id);
        }
        else
        {
            matchHistory.players.Add(playerOne.name, new() { match.id });
        }

        //Add tie for player 2
        if (matchHistory.players.ContainsKey(playerTwo.name))
        {
            matchHistory.players[playerTwo.name].Add(match.id);
        }
        else
        {
            matchHistory.players.Add(playerTwo.name, new() { match.id });
        }
        SaveMatchHistory();
    }

    public static void NewGame(bool vsAi, bool aiFirst)
    {
        if(vsAi)
        {
            if (aiFirst)
            {

            }
        }
    }

    static MatchHistory LoadMatchHistory()
    {
        if (File.Exists(savePath))
        {
            return JsonConvert.DeserializeObject<MatchHistory>(File.ReadAllText(savePath)) ?? new();
        }
        Debug.LogWarning("Save file not found. Making a new one");
        return new();
    }

    static void SaveMatchHistory()
    {
        File.WriteAllText(savePath, JsonConvert.SerializeObject(matchHistory));
    }
}
