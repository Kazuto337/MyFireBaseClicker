using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Managers;
using Serializables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingSceneHandler : MonoBehaviour
{
    public GameObject searchingPanel;
    public GameObject foundPanel;

    private void Start() => JoinQueue();

    private void JoinQueue()
    {
        MatchmakingManager.instance.JoinQueue();
    }

    private void Update()
    {
        searchingPanel.SetActive(!MatchmakingManager.instance.gameFound);
        foundPanel.SetActive(MatchmakingManager.instance.gameFound);
    }

    public void LeaveQueue()
    {
        MatchmakingManager.instance.LeaveQueue();
        SceneManager.LoadScene("Main");
    }

    public void SetReady()
    {
        MatchmakingManager.instance.SetReady();
    }
}