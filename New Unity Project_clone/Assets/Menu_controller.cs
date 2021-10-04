using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class Menu_controller : MonoBehaviour
{
    public MyNetworkingManager network;
    public GameObject[] players;
    private int index;

    // Start is called before the first frame update

    private void Start()
    {
        index = 0;
        //network.playerPrefab = players[index];
    }

    public void Left()
    {
        if (index-1 >= 0)
        {
            index--;
        }
        else {
            index = players.Length - 1;
        }

        network.playerPrefab = players[index];
    }

    public void Right()
    {
        if (index+1 < players.Length)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        network.playerPrefab = players[index];
    }

}
