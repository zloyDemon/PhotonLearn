using Bolt;
using Bolt.Photon;

[BoltGlobalBehaviour]
public class NetworkCallback : GlobalEventListener
{
    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<PhotonRoomProperties>();
        BoltNetwork.RegisterTokenClass<WeaponDropToken>();
    }
}
