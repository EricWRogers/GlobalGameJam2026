using Unity.Netcode;
using UnityEngine;

public class NetworkedLight : NetworkBehaviour
{
    public Light targetLight;

    public void TurnOn()
    {
        TurnOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TurnOnServerRpc()
    {
        if (targetLight != null)
        {
            targetLight.enabled = true;
            TurnOnClientRpc();
        }
    }

    [ClientRpc]
    private void TurnOnClientRpc()
    {
        if (targetLight != null)
            targetLight.enabled = true;
    }
}
