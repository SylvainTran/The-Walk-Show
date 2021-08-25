using System;

[Serializable]
public class PlayerStatistics
{
    public int characterUUIDCount;
    public int currentGameState;

    public PlayerStatistics()
    {
        this.characterUUIDCount = 0;
        this.currentGameState = 0;
    }

    public PlayerStatistics(int characterUUIDCount, int currentGameState)
    {
        this.characterUUIDCount = characterUUIDCount;
        this.currentGameState = currentGameState;
    }
}