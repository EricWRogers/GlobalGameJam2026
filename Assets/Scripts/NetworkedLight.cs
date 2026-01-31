using Unity.Netcode;
using UnityEngine;

public class NetworkedLight : NetworkBehaviour 
{
    public Light targetLight;
    private bool isOn = false; 

    public void TurnOn()
    {
        if (isOn) return; 
        
        Debug.Log($"Turning light ON from client {OwnerClientId}");
        isOn = true;
        
        if (targetLight) targetLight.enabled = true;
        
        
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = GetOtherClientIds()
            }
        };
        
        TurnOnClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void TurnOnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("ClientRpc received - turning light ON");
        isOn = true;
        if (targetLight) targetLight.enabled = true;
    }
    
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
