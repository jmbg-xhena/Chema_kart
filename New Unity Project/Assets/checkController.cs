using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkController : MonoBehaviour
{
    public GameObject[] checks;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        checks = GameObject.FindGameObjectsWithTag("check");
        foreach (GameObject i in checks) {
            i.SetActive(false);
        }

        index = 0;
        checks[index].SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("check")) {
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
