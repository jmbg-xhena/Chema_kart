using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFolow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player = null;

    public void IniciarSegir() {
        if (car_controller.Instance) {
            player = car_controller.instance.transform;
            this.enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (player) {
            Vector3 pos = player.position;
            Quaternion rot = player.rotation;
            transform.position = pos;
            transform.rotation = rot;
        }
    }
}
