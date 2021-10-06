using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkingManager : NetworkManager
{
    public  Car carro;
   public enum Car
    {
        Muscle,
        Subaru,
    }

    public struct CharacterCreatorMessage : NetworkMessage
    {
        public Car tipo_carro;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CharacterCreatorMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);


        CharacterCreatorMessage characterMessage = new CharacterCreatorMessage
        {
            tipo_carro = carro
        };

        conn.Send(characterMessage);

    }

    void OnCreateCharacter(NetworkConnection conn, CharacterCreatorMessage message)
    {
        GameObject gameobject = Instantiate(playerPrefab);
        GameObject carro = playerPrefab;

        /*if (message.tipo_carro == Car.Muscle)
        {
            carro = gameobject.GetComponent<PlayerSpawn>().muscle;
            gameobject.GetComponent<PlayerSpawn>().subaru.SetActive(false);
        }
        if(message.tipo_carro == Car.Subaru) {
            carro = gameobject.GetComponent<PlayerSpawn>().subaru;
            gameobject.GetComponent<PlayerSpawn>().muscle.SetActive(false);
        }*/

        //carro.SetActive(true);
        // controlar jugador
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }


}
