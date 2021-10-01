using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class car_controller : NetworkBehaviour
{
    [Header("Stats")]
    public Rigidbody rigi;
    public float acceleration = 10;
    public float torque_speed = 10;
    public float max_speed = 5;
    public float max_angular_speed = 4;
    public float tiempo_desaceleracion = 4;
    Vector3 Move;
    public static car_controller instance;
    [Header("Power-ups")]
    public GameObject proyectile;
    public Transform spawn_point;
    bool tiene_proyectile;
    bool boost;
    public static float proyecile_speed = 2;
    public static float boost_time = 2;



    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            rigi = GetComponent<Rigidbody>();
            instance = this;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFolow>().IniciarSegir();

            tiene_proyectile = false;
            boost = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        drive();
        if (Input.GetButton("Power")) {
            if (boost)
            {
                Boost();
            }
            else {
                if(tiene_proyectile){
                    Disparar();
                }
            }
        }
    }

    private void Boost() {
        print("boost");
        max_speed = max_speed * 1.4f;
        Invoke("UnBoost", boost_time);
    }

    private void UnBoost()
    {
        print("no boost");
        max_speed = max_speed / 1.4f;
    }

    private void Disparar()
    {

    }

    private void drive() {
        if (rigi.velocity.z > max_speed || rigi.velocity.z < -max_speed || rigi.velocity.x > max_speed || rigi.velocity.x < -max_speed)
        {
            Move.z = 0;
        }
        else
        {
            if ((Input.GetAxis("Horizontal") * torque_speed / tiempo_desaceleracion * 10) < Mathf.Abs(Input.GetAxis("Vertical") * acceleration))
            {
                Move.z = Input.GetAxis("Vertical") * acceleration - (Input.GetAxis("Horizontal") * torque_speed / tiempo_desaceleracion * 10);
            }
            else
            {
                Move.z = 0;
            }

        }
        if (Mathf.Abs(Move.z) > 0)
        {
            if (Mathf.Abs(rigi.angularVelocity.y) < max_angular_speed)
            {
                //Move.x = Input.GetAxis("Horizontal") * torque_speed;
                rigi.AddTorque(new Vector3(0, Input.GetAxis("Horizontal") * torque_speed, 0), ForceMode.Impulse);
            }
            else
            {
                Move.x = 0;
            }
        }
        else
        {
            Move.x = 0;
        }
        Move.y = 0;
        if (Input.GetAxis("Vertical") == 0 && Mathf.Abs(Move.z) > 0)
        {
            Move.z -= acceleration / tiempo_desaceleracion;
        }
        if (Input.GetButton("Jump") && rigi.velocity.z - acceleration * 2 / tiempo_desaceleracion > 0)
        {
            Move.z -= acceleration * 2 / tiempo_desaceleracion;
        }
        rigi.AddRelativeForce(Move, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("speed")) {
            max_speed = max_speed*1.15f;
            acceleration = acceleration * 1.15f;
        }

        if (other.CompareTag("boost"))
        {
            boost = true;
            tiene_proyectile = false;
        }

        if (other.CompareTag("proy"))
        {
            tiene_proyectile = true;
            boost = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("speed"))
        {
            max_speed = max_speed/1.15f;
            acceleration = acceleration / 1.15f;
        }
    }

    public static car_controller Instance
    {
        get
        {
            return instance;
        }
    }
}
