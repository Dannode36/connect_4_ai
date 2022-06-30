using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static Dictionary<Player, List<Tuple<Player, Player>>> matchHistory = new Dictionary<Player, List<Tuple<Player, Player>>>();

    static public bool single = true;
    static public string PlayerOneName = "Player 1";
    static public string PlayerTwoName = "Player 2";

    public static List<Tuple<Player, Player>> ReadPlayerScore(Player player)
    {
        return matchHistory[player];
    }

    public static void Win(Player winner, Player loser)
    {
        //Add win for winner
        if (matchHistory.ContainsKey(winner))
        {
            matchHistory[winner].Add(Tuple.Create(winner, loser));
        }
        else
        {
            matchHistory.Add(winner, new List<Tuple<Player, Player>>() { Tuple.Create(winner, loser) });
        }

        //Add loss for loser
        if (matchHistory.ContainsKey(loser))
        {
            matchHistory[loser].Add(Tuple.Create(winner, loser));
        }
        else
        {
            matchHistory.Add(loser, new List<Tuple<Player, Player>>() { Tuple.Create(winner, loser) });
        }
    }

    public static void NewGame()
    {
        matchHistory = new Dictionary<Player, List<Tuple<Player, Player>>>();
    }
}
