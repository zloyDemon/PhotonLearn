using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;
using UnityEngine;

public class WeaponDropToken : IProtocolToken
{
    public WeaponId ID;
    public int currentAmmo;
    public int totalAmmo;
    public NetworkId networkId;

    public void Read(UdpPacket packet)
    {
        ID = (WeaponId) packet.ReadInt();
        currentAmmo = packet.ReadInt();
        totalAmmo = packet.ReadInt();
        networkId = new NetworkId(packet.ReadULong());
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteInt((int)ID);
        packet.WriteInt(currentAmmo);
        packet.WriteInt(totalAmmo);
        packet.WriteULong(networkId.PackedValue);
    }
}

public enum WeaponId
{
    None = -1,
    Knife,
    Glock,
    Revolver,
    SecondaryEnd,
    AK47,
    SPAS12
}
