using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static List<Tuple<Player, Player, bool>> matchHistory = new List<Tuple<Player, Player, bool>>();

    static public bool single = true;

    public static int ReadPlayerScore()
    {
        return 0;
    }


    public static void Win(Player winner, Player looser, bool vsAI)
    {
        matchHistory.Add(new Tuple<Player, Player, bool>(winner, looser, vsAI));
        Debug.LogWarning($"{winner.name} won against {looser.name}!");
    }

    static void WriteMatchHistory(List<Tuple<string, string>> history)
    {
        
    }

    static List<Tuple<Player, Player>> ReadMatchHistory()
    {
        return null;
    }
}
