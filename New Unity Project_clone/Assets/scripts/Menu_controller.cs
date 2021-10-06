using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class Menu_controller : MonoBehaviour
{
    public MyNetworkingManager network;
    public GameObject[] players;
    public int index;

    // Start is called before the first frame update

    private void Start()
    {
        //network.playerPrefab = players[index];
        activar_desactivar();
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

        network.carro = (MyNetworkingManager.Car)index;
        activar_desactivar();
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

        network.carro = (MyNetworkingManager.Car)index;
        activar_desactivar();
    }

    private void activar_desactivar() {
        for (int i = 0; i < players.Length; i++)
        {
            if (i == index)
            {
                players[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                players[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

}
