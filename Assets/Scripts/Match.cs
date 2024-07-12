using System;

[Serializable]
public struct Match
{
    public string id;
    public bool tie;
    public Player winner;
    public Player looser;

    public Match(bool tie, Player winner, Player looser)
    {
        id = Guid.NewGuid().ToString();
        this.tie = tie;
        this.winner = winner;
        this.looser = looser;
    }
}
