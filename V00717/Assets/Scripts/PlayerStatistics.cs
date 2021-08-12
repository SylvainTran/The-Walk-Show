using System;

[Serializable]
public struct PlayerStatistics
{
    public int characterUUIDCount;

    public PlayerStatistics(int characterUUIDCount)
    {
        this.characterUUIDCount = characterUUIDCount;
    }
}