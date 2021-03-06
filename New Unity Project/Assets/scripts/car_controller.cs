using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

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
    public GameObject boostAura;
    public GameObject proyectile;
    public static float proyecile_speed = 40;
    public Transform spawn_point;
    bool tiene_proyectile;
    public bool stunt;
    public static float stunt_time = 0.5f;
    private float boost_speed;
    private float boost_accel;
    private float normal_speed;
    private float normal_accel;
    bool boost;
    public bool boosting;
    bool speed_up;
    public static float boost_time = 3;

    private bool moviendose_adelante;
    public carManager Tipo_carro;
    [SyncVar]
    public carManager.Car carro;
    public PlayerSpawn spawner;

    public GameObject carro_object;

    private bool coche_sel = false;

    private Animator anim;
    private NetworkAnimator NAnim;

    [Header("condicion de victoria")]
    public int car_id;
    [SyncVar]
    public int next_car_id;
    public int vueltas = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;
        GameManager.iniciar = false;
        vueltas = 0;
        car_id = next_car_id;
        Tipo_carro = GameObject.FindObjectOfType<carManager>();
        anim = gameObject.GetComponent<Animator>();
        NAnim = gameObject.GetComponent<NetworkAnimator>();
        carro = Tipo_carro.carro;
        spawner = gameObject.GetComponent<PlayerSpawn>();

        rigi = GetComponent<Rigidbody>();
        instance = this;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFolow>().IniciarSegir();

        tiene_proyectile = false;
        boost = false;
        speed_up = false;
        stunt = false;

        if (isServer)
        {
            return;
        }
        else {
            GameManager.index_ganador = -1;
            print("descativar canvas");
            GameObject.FindObjectOfType<CanvasIniciarCarrera>().button.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!coche_sel) {
            Cmd_id();
            Cmd_selCoche(carro);
            coche_sel = true;
        }
        if (!isLocalPlayer) return;

        if (!GameManager.iniciar) return;

        drive();

        if (Input.GetButtonDown("Power")) {
            if (boost)
            {
                Cmd_SetBoosting();
                Boost();
            }
            else
            {
                if (tiene_proyectile)
                {
                    if (isClientOnly)
                    {
                        Rcp_crearBala();
                    }
                    else
                    {
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

        if (vueltas == GameManager.vueltas_totales) {
            Cmd_over(car_id);
        }
    }

    [Command(requiresAuthority = false)]
    void Cmd_SetBoost()
    {
        Rpc_SetBoost();
    }

    [ClientRpc]
    void Rpc_SetBoost()
    {
        boost = true;
    }

    [Command(requiresAuthority = false)]
    void Cmd_UnSetBoost()
    {
        Rpc_UnSetBoost();
    }

    [ClientRpc]
    void Rpc_UnSetBoost()
    {
        boost = false;
    }

    [Command(requiresAuthority = false)]
    void Cmd_SetBoosting()
    {
        Rpc_SetBoosting();
    }

    [ClientRpc]
    void Rpc_SetBoosting()
    {
        boosting = true;
    }

    [Command(requiresAuthority = false)]
    void Cmd_UnSetBoosting()
    {
        Rpc_UnSetBoosting();
    }

    [ClientRpc]
    void Rpc_UnSetBoosting()
    {
        boosting = false;
    }

    [Command(requiresAuthority = false)]
    void Cmd_boost()
    {
        Rpc_boost();
    }

    [ClientRpc]
    void Rpc_boost()
    {
        if (boost || speed_up) {
            boost = false;
            boostAura.SetActive(true);
        }
    }

    [Command(requiresAuthority = false)]
    void Cmd_UnBoost()
    {
        Rpc_UnBoost();
    }

    [ClientRpc]
    void Rpc_UnBoost()
    {
        if (!boost && !speed_up)
        {
            boostAura.SetActive(false);
        }
    }

    [Command(requiresAuthority = false)]
    void Cmd_SpeedUp()
    {
        Rpc_SpeedUp();
    }

    [ClientRpc]
    void Rpc_SpeedUp()
    {
        speed_up = true;
    }

    [Command(requiresAuthority = false)]
    void Cmd_UnSpeedUp()
    {
        Rpc_UnSpeedUp();
    }

    [ClientRpc]
    void Rpc_UnSpeedUp()
    {
        speed_up = false;
    }

    private void Boost() {
        Cmd_boost();
        print("boost");
        max_speed = boost_speed;
        acceleration = boost_accel;
        Invoke("UnBoost", boost_time);
    }

    private void UnBoost()
    {
        print("no boost");
        max_speed = normal_speed;
        acceleration = normal_accel;
        if (boosting) {
            Cmd_UnBoost();
        }
        Cmd_UnBoost();
    }

    [Command(requiresAuthority = false)]
    void Cmd_id()
    {
        Rpc_id();
    }

    [ClientRpc]
    void Rpc_id()
    {
        next_car_id++;
    }

    [Command(requiresAuthority = false)]
    void Cmd_over(int id)
    {
        Rpc_over(id);
    }

    [ClientRpc]
    void Rpc_over(int id)
    {
        GameManager.index_ganador = id;
        SceneManager.LoadScene("over");
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
        normal_speed = max_speed;
        normal_accel = acceleration;
        boost_speed = max_speed * 1.15f;
        boost_accel = acceleration * 1.15f;
        carro_object.SetActive(true);
        //gameObject.GetComponent<NetworkAnimator>().animator = carro_object.GetComponent<Animator>();
    }

    [Command(requiresAuthority = false)]
    void Cmd_selCoche(carManager.Car tipo_carro)
    {
        Rpc_selCoche(tipo_carro);
    }

    private void animar_drercha() {
        NAnim.SetTrigger("entrar_right");
        anim.SetBool("en_derecha", true);
        anim.SetBool("en_izquierda", false);
    }

    private void animar_izquierda()
    {
        NAnim.SetTrigger("entrar_left");
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
                        NAnim.SetTrigger("atras");
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
                            NAnim.SetTrigger("adelante");
                            anim.SetBool("en_adelante", true);
                            anim.SetBool("detenido", false);
                        }
                    }
                    else {
                        anim.SetBool("en_adelante", false);
                        if ((Move.z > -0.5 || Move.z < 0.5) && !anim.GetBool("detenido"))
                        {
                            NAnim.SetTrigger("detener");
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
            Cmd_SpeedUp();
            Boost();
        }

        if (other.CompareTag("boost"))
        {
            Cmd_SetBoost();
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
            if (speed_up) {
                Cmd_UnSpeedUp();
                if (!boosting) {
                    UnBoost();
                }
            }
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
