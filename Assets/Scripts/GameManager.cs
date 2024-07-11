using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Gamemode
{
    VsAI,
    COOP,
}

public static class GameManager
{
    static Dictionary<Player, List<Match>> matchHistory = new();

    static public Gamemode gamemode;
    static public bool aiFirst;
    static public bool swapStarting;
    static public string PlayerOneName = "Player 1";
    static public string PlayerTwoName = "Player 2";

    public static List<Match> ReadPlayerScore(Player player)
    {
        return matchHistory[player];
    }

    public static void Win(Player winner, Player loser)
    {
        //Add win for winner
        if (matchHistory.ContainsKey(winner))
        {
            matchHistory[winner].Add(new(false, winner, loser));
        }
        else
        {
            matchHistory.Add(winner, new() { new(false, winner, loser) });
        }

        //Add loss for loser
        if (matchHistory.ContainsKey(loser))
        {
            matchHistory[loser].Add(new(false, winner, loser));
        }
        else
        {
            matchHistory.Add(loser, new() { new(false, winner, loser) });
        }
    }

    public static void Tie(Player winner, Player loser)
    {
        //Add win for winner
        if (matchHistory.ContainsKey(winner))
        {
            matchHistory[winner].Add(new(true, winner, loser));
        }
        else
        {
            matchHistory.Add(winner, new() { new(true, winner, loser) });
        }

        //Add loss for loser
        if (matchHistory.ContainsKey(loser))
        {
            matchHistory[loser].Add(new(true, winner, loser));
        }
        else
        {
            matchHistory.Add(loser, new() { new(true, winner, loser) });
        }
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
}
