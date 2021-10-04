using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bala : NetworkBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.transform.CompareTag("wall"))
        {
            Destroy(gameObject);
        }
    }
}
