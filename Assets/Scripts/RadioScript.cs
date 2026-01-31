using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class RadioScript : NetworkBehaviour 
{
    [SerializeField] private XRKnob knob;
    private bool activated;
    private bool completed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            knob.onValueChange.AddListener(_ => OnKnobTurned());
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

    public override void OnNetworkDespawn()
    {
        if (IsOwner && knob != null)
            knob.onValueChange.RemoveListener(_ => OnKnobTurned());
    }
}
