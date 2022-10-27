using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameSceneHandler : MonoBehaviour
{
    public void OnSearchGame()
    {
        SceneManager.LoadScene("Matchmaking");
    }
}
