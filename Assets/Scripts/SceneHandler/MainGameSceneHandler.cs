using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameSceneHandler : MonoBehaviour
{
    public void OnSearchGame()
    {
        SceneManager.LoadScene("Matchmaking");
    }

    public void Logout()
    {
        FireBaseManager.instance.LogOutButton();
    }
}
