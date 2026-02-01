using Unity.Netcode;
using UnityEngine;

public class NetworkedLight : NetworkBehaviour 
{
    public Light targetLight;

    readonly NetworkVariable<bool> m_IsOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public void TurnOn()
    {
        if (m_IsOn.Value) return;

        Debug.Log($"Turning light ON from client {NetworkManager.Singleton.LocalClientId}");
        m_IsOn.Value = true;

        if (targetLight) targetLight.enabled = true;

        // Owner -> Everyone RPC pattern
        TurnOnOwnerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Owner)]
    void TurnOnOwnerRpc(ulong clientId)
    {
        m_IsOn.Value = true;
        TurnOnRpc(clientId);
    }

    [Rpc(SendTo.Everyone)]
    void TurnOnRpc(ulong clientId)
    {
        Debug.Log($"TurnOnRpc invoked on {NetworkManager.Singleton.LocalClientId} from {clientId}");
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            m_IsOn.Value = true;
            if (targetLight) targetLight.enabled = true;
        }
    }
    
    // Kept for compatibility, not used by Rpc(SendTo.*) pattern
    private ulong[] GetOtherClientIds()
    {
        var clients = NetworkManager.Singleton.ConnectedClients;
        var otherClients = new System.Collections.Generic.List<ulong>();

        foreach (var client in clients)
        {
            if (client.Key != OwnerClientId)
                otherClients.Add(client.Key);
        }
        return otherClients.ToArray();
    }
}
