using System;
using Unity.Netcode;

[Serializable]
public struct NetworkPerkType : IEquatable<NetworkPerkType>, INetworkSerializable
{
    public NetworkPlayerPerksManager.TypeOfPerks Value;

    public bool Equals(NetworkPerkType other)
    {
        return Value == other.Value;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref Value);
    }

    public static implicit operator NetworkPerkType(
        NetworkPlayerPerksManager.TypeOfPerks value)
    {
        return new NetworkPerkType { Value = value };
    }

    public static implicit operator NetworkPlayerPerksManager.TypeOfPerks(
        NetworkPerkType value)
    {
        return value.Value;
    }
}