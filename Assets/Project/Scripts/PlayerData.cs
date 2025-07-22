using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public bool playerPlaced;

    public PlayerData(ulong clientId, bool playerPlaced)
    {
        this.clientId = clientId;
        this.playerPlaced = playerPlaced;
    }
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && playerPlaced == other.playerPlaced;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerPlaced);
    }
}
