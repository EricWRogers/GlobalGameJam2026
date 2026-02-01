using Unity.Netcode;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{


    public override void OnNetworkSpawn()
    {
        
    }

    void Update()
    {
        // TestClientRPC();
        // Test2ClientRPC();
        // Test3ClientRPC();
        // TestServerRpc();
        // Test2ServerRpc();
        // Test3ServerRpc();
        TestRpc();
        Test2Rpc();
        Test3Rpc();
        Test44();
    }


    // [ClientRpc]
    // private void TestClientRPC()
    // {
    //     Debug.Log("Test Client RPC called");
    // }

    // [ClientRpc]
    // private void Test2ClientRPC(ClientRpcParams clientRpcParams = default)
    // {
    //     Debug.Log("Test2 Client RPC called");
    // }
    // [ClientRpc(RequireOwnership = false)]
    // private void Test3ClientRPC(ClientRpcParams clientRpcParams = default)
    // {
    //     Debug.Log("Test3 Client RPC called");
    // }        
    // [ServerRpc]
    // private void TestServerRpc()
    // {
    //     Debug.Log("Test Server RPC called");
    // }

    // [ServerRpc]
    // private void Test2ServerRpc(ServerRpcParams serverRpcParams = default)
    // {
    //     Debug.Log("Test2 Server RPC called");
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void Test3ServerRpc(ServerRpcParams serverRpcParams = default)
    // {
    //     Debug.Log("Test3 Server RPC called");
    // }

    [Rpc(SendTo.Everyone)]
    private void TestRpc() //Legit everyone
    {
        Debug.Log("Test RPC called");
    }
    [Rpc(SendTo.Owner)]
    private void Test2Rpc() //The network owner of the object with this script can see this.
    {
        Debug.Log("Test2 RPC called");
    }
    [Rpc(SendTo.Server)]
    private void Test3Rpc() //The 'host' or the person who started the lobby can see this
    {
        Debug.Log("Test3 RPC called");
    }

    [Rpc(SendTo.Everyone)]
    private void Test4ClientRpc(ulong _id) //Use this to specifically target someone
    {
       if(NetworkManager.Singleton.LocalClientId == _id)
       {
            Debug.Log("Test4 Client RPC called on owner");
       }
    }

    private void Test44()
    {
        ulong[] id = GetOtherClientIds();
        Test4ClientRpc(id[0]);
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
