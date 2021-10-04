using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkingManager : NetworkManager
{
    public Car carro;
    public GameObject muscle;
    public GameObject subaru;


    public enum Car
    {
        Muscle,
        Subaru,
    }

    void OnCreateCharacter(NetworkConnection conn)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(playerPrefab);

        if (carro == Car.Muscle)
        {
            gameobject.GetComponent<PlayerSpawn>().muscle.SetActive(true);
        }
        if(carro == Car.Subaru) {
            gameobject.GetComponent<PlayerSpawn>().subaru.SetActive(true);
        }

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }


}
