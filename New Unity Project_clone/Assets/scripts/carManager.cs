using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Car carro;
    static GameObject instance;
    public enum Car
    {
        Muscle,
        Subaru,
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            if (this.gameObject != instance) {
                Destroy(this.gameObject);
            }
        }

    }
}
