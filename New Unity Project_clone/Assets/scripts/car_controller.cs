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
    public static float proyecile_speed = 40;
    public Transform spawn_point;
    bool tiene_proyectile;
    public bool stunt;
    public static float stunt_time = 0.5f;
    bool boost;
    public static float boost_time = 2;

    private bool moviendose_adelante;
    public carManager Tipo_carro;
    [SyncVar]
    public carManager.Car carro;
    public PlayerSpawn spawner;

    public GameObject carro_object;

    private bool coche_sel = false;

    private Animator anim;



    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        Tipo_carro = GameObject.FindObjectOfType<carManager>();
        carro = Tipo_carro.carro;
        spawner = gameObject.GetComponent<PlayerSpawn>();

        rigi = GetComponent<Rigidbody>();
        instance = this;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFolow>().IniciarSegir();

        tiene_proyectile = false;
        boost = false;
        stunt = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!coche_sel) {
            Cmd_selCoche(carro);
            coche_sel = true;
        }
        if (!isLocalPlayer) return;
        if (!anim && carro_object) {
            anim = carro_object.GetComponent<Animator>();
        }
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
        max_speed = max_speed * 1.15f;
        acceleration = acceleration * 1.15f;
        Invoke("UnBoost", boost_time);
    }

    private void UnBoost()
    {
        print("no boost");
        max_speed = max_speed / 1.15f;
        acceleration = acceleration / 1.15f;
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

    [Command(requiresAuthority = false)]
    void Cmd_crearBala()
    {
        //Instanciamos de forma normal utilizando el network manager
        GameObject go = NetworkManager.Instantiate(proyectile, spawn_point.position, Quaternion.identity);
        //Le podemos hacer cambos al objeto
        go.GetComponent<Rigidbody>().velocity = transform.forward * proyecile_speed;
        //Ya que terminamos de hcer los cambios, podemos decirle
        NetworkServer.Spawn(go);
    }


    [ClientRpc]
    void Rpc_selCoche(carManager.Car tipo_carro)
    {
        //if (!isLocalPlayer) return;

        if (tipo_carro == carManager.Car.Muscle)
        {
            carro_object = spawner.muscle;
            acceleration = 8f;
            torque_speed = 0.06f;
            max_speed = 12f;
            max_angular_speed = 1f;
            tiempo_desaceleracion = 6f;
        }
        if (tipo_carro == carManager.Car.Subaru)
        {
            carro_object = spawner.subaru;
            acceleration = 5f;
            torque_speed = 0.08f;
            max_speed = 11f;
            max_angular_speed = 1.12f;
            tiempo_desaceleracion = 2f;
        }
        carro_object.SetActive(true);
        gameObject.GetComponent<NetworkAnimator>().animator = carro_object.GetComponent<Animator>();
    }

    [Command(requiresAuthority = false)]
    void Cmd_selCoche(carManager.Car tipo_carro)
    {
        Rpc_selCoche(tipo_carro);
    }

    private void animar_drercha() {
        anim.SetTrigger("entrar_right");
        anim.SetBool("en_derecha", true);
        anim.SetBool("en_izquierda", false);
    }

    private void animar_izquierda()
    {
        anim.SetTrigger("entrar_left");
        anim.SetBool("en_izquierda", true);
        anim.SetBool("en_derecha", false);
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
                if (Move.z < -0.5)
                {
                    if (!anim.GetBool("en_atras"))
                    {
                        anim.SetTrigger("atras");
                        anim.SetBool("en_atras", true);
                        anim.SetBool("en_adelante", false);
                        anim.SetBool("detenido", false);
                    }
                }
                else {
                    anim.SetBool("en_atras", false);
                    if (Move.z > 0.5)
                    {
                        if (!anim.GetBool("en_adelante")) {
                            moviendose_adelante = true;
                            anim.SetTrigger("adelante");
                            anim.SetBool("en_adelante", true);
                            anim.SetBool("detenido", false);
                        }
                    }
                    else {
                        anim.SetBool("en_adelante", false);
                        if ((Move.z > -0.5 || Move.z < 0.5) && !anim.GetBool("detenido"))
                        {
                            anim.SetTrigger("detener");
                            anim.SetBool("detenido", true);
                        }
                    }

                }
                if (Move.z < 0)
                {
                    moviendose_adelante = false;
                }
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
                if (moviendose_adelante)
                {
                    rigi.AddTorque(new Vector3(0, Input.GetAxis("Horizontal") * torque_speed, 0), ForceMode.Impulse);
                }
                else {
                    rigi.AddTorque(new Vector3(0, -Input.GetAxis("Horizontal") * torque_speed, 0), ForceMode.Impulse);
                }

                if (Input.GetAxis("Horizontal") < 0.5 && Input.GetAxis("Horizontal") > -0.5)
                {
                    anim.SetBool("en_derecha", false);
                    anim.SetBool("en_izquierda", false);
                }
                else {
                    if (Input.GetAxis("Horizontal") > 0.5)
                    {
                        if (!anim.GetBool("en_derecha"))
                        {
                            animar_drercha();
                        }
                    }
                    else
                    {
                        if (!anim.GetBool("en_izquierda"))
                        {
                            animar_izquierda();
                        }
                    }
                }
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
