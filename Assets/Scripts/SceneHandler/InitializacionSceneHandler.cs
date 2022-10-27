using System.Collections;
using System.Collections.Generic;
using APIs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializacionSceneHandler : MonoBehaviour
{
    private void Start()
    {
        DatabaseAPI.InitializeDatabase();
        SceneManager.LoadScene("MainMenu");
    }
}
