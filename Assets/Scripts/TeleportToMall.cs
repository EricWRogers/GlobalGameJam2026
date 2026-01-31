using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TeleportToMall : NetworkBehaviour
{
    [SerializeField] private Transform destination;

    TeleportationProvider m_LocalTeleportProvider;

    void Start()
    {
        m_LocalTeleportProvider = FindFirstObjectByType<TeleportationProvider>();
    }

    public void HostTeleportAll()
    {
        if (!IsOwner) return; 
        TeleportAllRpc();
    }

    
    [Rpc(SendTo.Everyone)]
    void TeleportAllRpc()
    {
        if (m_LocalTeleportProvider == null)
            return;

        TeleportRequest request = new TeleportRequest
        {
            destinationPosition = destination.position,
            destinationRotation = destination.rotation,
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        m_LocalTeleportProvider.QueueTeleportRequest(request);
    }
}
