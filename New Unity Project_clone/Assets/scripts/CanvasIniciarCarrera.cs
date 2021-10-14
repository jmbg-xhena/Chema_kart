using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasIniciarCarrera : MonoBehaviour
{
    private void Awake()
    {
        GameManager.iniciar = false;
    }

    public void iniciar() {
        GameManager.iniciar = true;
    }
}
