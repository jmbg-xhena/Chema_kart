using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bala : NetworkBehaviour
{
    //[ClientCallback]// Esta funcion solo se llama en el cliente
    [ServerCallback] //la funcion se llama solamente en el servidor
    void Start()
    {
        //if(isServer)
        Destroy(gameObject, 5);//El destroy se sincroniza solo (siemore y cuando tenga un NetworkIdentity)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
