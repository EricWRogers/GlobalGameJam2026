using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RadioScript : NetworkBehaviour 
{
    [SerializeField] private XRKnob knob;
    
    
    readonly NetworkVariable<bool> m_Activated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    readonly NetworkVariable<bool> m_Completed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private XRSimpleInteractable simpleInteractable;

    public override void OnNetworkSpawn()
    {
        if (simpleInteractable != null)
            simpleInteractable.activated.AddListener(OnActivated);  
        
        if (knob != null)
            knob.onValueChange.AddListener(_ =>OnKnobTurned());  

        m_Activated.OnValueChanged += (oldVal, newVal) => {
            if (newVal)
            {
                Debug.Log("Radio activated (network)");
            }
        };

        m_Completed.OnValueChanged += (oldVal, newVal) => {
            if (newVal)
            {
                Debug.Log("RADIO COMPLETED! (network)");
            }
        };
    }

    public void OnActivated(ActivateEventArgs args)  
    {
        Debug.Log($"OnActivated called locally by ClientId: {NetworkManager.Singleton.LocalClientId}");

        
        if (!m_Activated.Value)  
            ActivateOwnerRpc(NetworkManager.Singleton.LocalClientId);
    }

    
    [Rpc(SendTo.Owner)]
    void ActivateOwnerRpc(ulong clientId)
    {
        m_Activated.Value = true;
        ActivateRpc(clientId);
    }

    
    [Rpc(SendTo.Everyone)]
    void ActivateRpc(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"ActivateRpc received from {clientId}");
            m_Activated.Value = true;
        }
    }

    public void OnKnobTurned()  
    {
        if (!m_Activated.Value)
        {
            Debug.Log($"Knob turned to {knob.value} but radio not activated");
            return;
        }

        float value = knob.value;  
        Debug.Log($"Knob turned to: {value}");


        KnobChangedOwnerRpc(value, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Owner)]
    void KnobChangedOwnerRpc(float newValue, ulong clientId)
    {
        if (newValue > .9f && !m_Completed.Value)
        {
            m_Completed.Value = true;
            Debug.Log("RADIO COMPLETED! (owner rpc)");
        }
        KnobChangedRpc(newValue, clientId);
    }

    [Rpc(SendTo.Everyone)]
    void KnobChangedRpc(float newValue, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Knob change from {clientId}: {newValue}");
            if (newValue > .9f && !m_Completed.Value)
            {
                m_Completed.Value = true;
                Debug.Log("RADIO COMPLETED! (rpc)");
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (simpleInteractable != null)
            simpleInteractable.activated.RemoveListener(OnActivated);
        if (knob != null)
            knob.onValueChange.RemoveListener(_ =>OnKnobTurned());
    }
}
