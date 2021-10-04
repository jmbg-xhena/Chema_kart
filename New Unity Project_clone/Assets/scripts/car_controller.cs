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
    public bool stunt;
    public static float proyecile_speed = 40;
    public static float boost_time = 2;
    public static float stunt_time = 0.5f;



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
            stunt = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        drive();

        if (Input.GetButtonDown("Power")) {
            if (boost)
            {
                Boost();
            }
            else {
                if(tiene_proyectile){
                    if (isClientOnly)
                    {
                        Rcp_crearBala();
                    }
                    else {
                        Cmd_crearBala();
                    }
                    tiene_proyectile = false;
                }
            }
        }

        if (stunt) {
            Move.z = 0;
            Move.x = 0;
            rigi.velocity = Vector3.zero;
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

    [ClientRpc]
    void Rcp_crearBala()
    {
        //Instanciamos de forma normal utilizando el network manager
        GameObject go = NetworkManager.Instantiate(proyectile, spawn_point.position, Quaternion.identity);
        //Le podemos hacer cambos al objeto
        go.GetComponent<Rigidbody>().velocity = transform.forward * proyecile_speed;
        //Ya que terminamos de hcer los cambios, podemos decirle
        NetworkServer.Spawn(go);
    }

    [Command]
    void Cmd_crearBala()
    {
        //Instanciamos de forma normal utilizando el network manager
        GameObject go = NetworkManager.Instantiate(proyectile, spawn_point.position, Quaternion.identity);
        //Le podemos hacer cambos al objeto
        go.GetComponent<Rigidbody>().velocity = transform.forward * proyecile_speed;
        //Ya que terminamos de hcer los cambios, podemos decirle
        NetworkServer.Spawn(go);
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
        if (Mathf.Abs(rigi.velocity.z) > 0)
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
        if (!isLocalPlayer) return;
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
        if (!isLocalPlayer) return;
        if (other.CompareTag("speed"))
        {
            max_speed = max_speed/1.15f;
            acceleration = acceleration / 1.15f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer) return;
        if (collision.transform.CompareTag("bala")) {
            stunt = true;
            Invoke("UnStunt",stunt_time);
        }
    }

    void UnStunt() {
        if (!isLocalPlayer) return;
        stunt = false;
    }

    public static car_controller Instance
    {
        get
        {
            return instance;
        }
    }
}
