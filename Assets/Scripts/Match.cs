public struct Match
{
    public bool tie;
    public Player winner;
    public Player looser;

    public Match(bool tie, Player winner, Player looser)
    {
        this.tie = tie;
        this.winner = winner;
        this.looser = looser;
    }
}
