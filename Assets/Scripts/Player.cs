using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public readonly int turnNumber;
    public string name;

    public Player(int id, string name)
    {
        this.turnNumber = id;
        this.name = name;
    }
}
