using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class respawner : NetworkBehaviour
{
    static float spawn_time = 4;
    public Collider col;
    public Renderer rend;
    [SyncVar]
    public bool activo = true;


    private void Update()
    {
        if (col.enabled != activo) {
            col.enabled = activo;
            rend.enabled = activo;
        }
    }

    private void Start()
    {
        activo = true;
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            activo = false;
            Invoke("respawn", spawn_time);
        }
    }

    private void respawn() {
        activo = true;
    }
}
