using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static Dictionary<Player, int> matchHistory = new Dictionary<Player, int>();

    static public bool single = true;

    public static int ReadPlayerScore(Player player)
    {
        return matchHistory[player];
    }

    public static void Win(Player player)
    {
        if (matchHistory.ContainsKey(player))
        {
            matchHistory[player]++;
        }
        else
        {
            matchHistory.Add(player, 1);
        }
    }

    public static void NewGame()
    {
        matchHistory = new Dictionary<Player, int>();
    }
}
