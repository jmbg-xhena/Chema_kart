using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class car_controller : MonoBehaviour
{

    Rigidbody rigi;
    public float acceleration = 10;
    public float torque_speed = 10;
    public float max_speed = 5;
    public float max_angular_speed = 4;
    public float tiempo_desaceleracion = 4;
    public bool pressed = false;
    Vector3 move;
    Vector2 axis;



    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rigi.velocity.z > max_speed || rigi.velocity.z < -max_speed || rigi.velocity.x > max_speed || rigi.velocity.x < -max_speed)
        {
            move.z = 0;
        }
        else
        {
            if ((axis.x * torque_speed / tiempo_desaceleracion * 10) < Mathf.Abs(axis.y * acceleration))
            {
                move.z = axis.y * acceleration - (axis.x * torque_speed / tiempo_desaceleracion * 10);
            }
            else
            {
                move.z = 0;
            }

        }
        if (acceleration > 0)
        {
            if (rigi.angularVelocity.y < max_angular_speed && rigi.angularVelocity.y > -max_angular_speed)
            {
                //Move.x = Input.GetAxis("Horizontal") * torque_speed;
                rigi.AddTorque(new Vector3(0, axis.x * torque_speed, 0), ForceMode.Impulse);
            }
            else
            {
                move.x = 0;
            }
        }
        else
        {
            move.x = 0;
        }
        move.y = 0;
        if (axis.y == 0 && Mathf.Abs(move.z) > 0)
        {
            move.z -= acceleration / tiempo_desaceleracion;
        }
        if (Input.GetButton("Jump") && rigi.velocity.z - acceleration * 2 / tiempo_desaceleracion > 0)
        {
            move.z -= acceleration * 2 / tiempo_desaceleracion;
        }
        rigi.AddRelativeForce(move, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("speed")) {
            max_speed = max_speed*1.2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("speed"))
        {
            max_speed = max_speed/1.2f;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        axis = context.ReadValue<Vector2>();
        print(axis);
    }
}
