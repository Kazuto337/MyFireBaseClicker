using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneHandler : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null && auth.CurrentUser == user) return;
        
        user = auth.CurrentUser;
    }

    private void Update()
    {
        if (MainManager.Instance.currentLocalPlayerId == auth.CurrentUser.UserId)
        {
            SceneManager.LoadScene("Main");
        }
    }
}
