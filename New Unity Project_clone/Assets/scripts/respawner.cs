using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawner : MonoBehaviour
{
    static float spawn_time = 4;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            gameObject.SetActive(false);
            Invoke("respawn", spawn_time);
        }
    }

    private void respawn() {
        gameObject.SetActive(true);
    }
}
