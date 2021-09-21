using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_controller : MonoBehaviour
{

    Rigidbody rigi;
    public float acceleration = 10;
    public float torque_speed = 10;
    public float max_speed = 5;
    public float max_angular_speed = 4;


    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Move;
        if (rigi.velocity.z > max_speed || rigi.velocity.z < -max_speed || rigi.velocity.x > max_speed || rigi.velocity.x < -max_speed)
        {
            Move.z = 0;
        }
        else
        {
            Move.z = Input.GetAxis("Vertical") * acceleration;
        }
        if (acceleration > 0)
        {
            if (rigi.angularVelocity.y < max_angular_speed && rigi.angularVelocity.y > -max_angular_speed)
            {
                Move.x = Input.GetAxis("Horizontal") * torque_speed;
                rigi.AddTorque(new Vector3(0, Move.x * torque_speed, 0), ForceMode.Impulse);
            }
            else {
                Move.x = 0;
            }
        }
        else {
            Move.x = 0;
        }
        Move.y = 0;
        rigi.AddRelativeForce(Move, ForceMode.Force);
    }
}
