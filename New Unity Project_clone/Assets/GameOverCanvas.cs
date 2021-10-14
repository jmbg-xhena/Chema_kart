using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public Text win_text;
    // Start is called before the first frame update
    void Start()
    {
        win_text.text = "Player " + ((int)GameManager.index_ganador + (int)1) + " Won";
    }
}
