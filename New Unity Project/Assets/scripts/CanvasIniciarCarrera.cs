using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CanvasIniciarCarrera : NetworkBehaviour
{
    public GameObject button;
    [SyncVar]
    public bool iniciado;

    private void Update()
    {
        if (GameManager.iniciar != iniciado) {
            GameManager.iniciar = iniciado;
        }
        if (GameManager.iniciar == true) {
            gameObject.SetActive(false);
        }
    }

    [Command(requiresAuthority = false)]
    void Cmd_Ininciar(bool _iniciado) {
        Rpc_Ininciar(_iniciado);
    }

    [ClientRpc]
    void Rpc_Ininciar(bool _iniciado)
    {
        iniciado = _iniciado;
    }

    private void Start()
    {
        Cmd_Ininciar(false);
    }

    public void iniciar() {
        Cmd_Ininciar(true);
    }
}
