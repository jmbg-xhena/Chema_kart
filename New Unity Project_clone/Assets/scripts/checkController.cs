using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class checkController : NetworkBehaviour
{
    public GameObject[] checks;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer) {
            checks = GameObject.FindGameObjectsWithTag("check");
            foreach (GameObject i in checks)
            {
                i.SetActive(false);
            }

            index = 0;
            checks[index].SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer && other.CompareTag("check")) {
            checks[index].SetActive(false);
            index++;
            if (index < checks.Length)
            {
                checks[index].SetActive(true);
            }
            else {
                index = 0;
                checks[index].SetActive(true);
            }
        }
    }
}
