using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public string username;
    public string userId;

    public event Action OnDataReady;

    private void Start()
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        GetUserData();
    }

    private void GetUserData()
    {
        FirebaseDatabase.DefaultInstance.
            GetReference("users/" + userId).
            GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    Dictionary<string, object> userData = (Dictionary<string, object>)snapshot.Value;

                    username = (string)userData["username"];

                    OnDataReady?.Invoke();
                }
            });
    }
}