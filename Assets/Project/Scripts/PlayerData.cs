using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public float health;
    public bool playerPlaced;

    public PlayerData(ulong clientId, float health, bool playerPlaced)
    {
        this.clientId = clientId;
        this.health = health;
        this.playerPlaced = playerPlaced;
    }
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && other.health == health && playerPlaced == other.playerPlaced;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref health);
        serializer.SerializeValue(ref playerPlaced);
    }
}
