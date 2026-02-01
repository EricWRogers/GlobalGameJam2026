using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RadioScript : NetworkBehaviour 
{
    [SerializeField] private XRKnob knob;
    private bool activated;
    private bool completed;

    [SerializeField] private XRSimpleInteractable simpleInteractable;

    public override void OnNetworkSpawn()
    {
        if (simpleInteractable != null)
            simpleInteractable.activated.AddListener(_ => OnActivated());
        
        if (knob != null)
            knob.onValueChange.AddListener(_ => OnKnobTurned()); 
    }

    public void OnActivated()
    {
        Debug.Log($"OnActivated called by ClientId: {OwnerClientId}");
        
        if (!activated)
        {
            activated = true;
            Debug.Log("Radio activated! (local)");
            
            
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = GetOtherClientIds()
                }
            };
            ActivateRadioClientRpc(clientRpcParams);
        }
    }

    [ClientRpc]
    private void ActivateRadioClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Activate RPC received - turning radio ON");
        activated = true;
        Debug.Log("Radio activated!");
    }

    public void OnKnobTurned()
    {
        if (!activated) 
        {
            Debug.Log($"Knob turned to {knob.value} but radio not activated");
            return;
        }
        
        Debug.Log($"Knob turned to: {knob.value}");
        
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = GetOtherClientIds()
            }
        };
        CheckKnobClientRpc(knob.value, clientRpcParams);
    }

    [ClientRpc]
    private void CheckKnobClientRpc(float knobValue, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log($"CheckKnob RPC received: {knobValue}");
        if (knobValue > 0.9f && !completed)
        {
            completed = true;
            Debug.Log("RADIO COMPLETED!");
        }
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

    public override void OnNetworkDespawn()
    {
        if (simpleInteractable != null)
            simpleInteractable.activated.RemoveListener(_ => OnActivated());
        if (knob != null)
            knob.onValueChange.RemoveListener(_ => OnKnobTurned());
    }
}
