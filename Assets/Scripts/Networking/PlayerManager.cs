using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Obsolete("Using UNET")]
public class PlayerManager : NetworkManager
{

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
     //   player.GetComponent<Player>().color = Color.red;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
