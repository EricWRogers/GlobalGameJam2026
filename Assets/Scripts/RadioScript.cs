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
    {
        simpleInteractable.activated.AddListener(OnActivated); 
    }
    
    if (IsOwner && knob != null)
        knob.onValueChange.AddListener(_ => OnKnobTurned()); 
}

private void OnActivated(ActivateEventArgs args)
{
    NetworkObject.RequestOwnership();
}

public override void OnNetworkDespawn()
{
    if (simpleInteractable != null)
        simpleInteractable.activated.RemoveListener(OnActivated);
    
    if (IsOwner && knob != null)
        knob.onValueChange.RemoveListener(_ => OnKnobTurned());
}


    public void TogglePower() 
    {
        if (!IsOwner) return;
        activated = true;
        Debug.Log("Radio activated!");
        TogglePowerClientRpc();
    }

    private void OnKnobTurned()
    {
        if (!IsOwner || !activated) return;

        if(knob.value == 1){

        
        completed = true;
        Debug.Log("Radio completed!");
        CompleteRadioClientRpc();
        }
    }

    [ClientRpc] private void TogglePowerClientRpc() {

        Debug.Log("Radio activated!");

        activated = true;
    }
    [ClientRpc] private void CompleteRadioClientRpc() { 
        
        Debug.Log("Radio completed!");
        completed = true;
    
    }


}
