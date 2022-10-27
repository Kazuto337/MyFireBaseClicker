using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using Firebase.Database;
using UnityEngine.UI;

public class UsersOnlineController : MonoBehaviour
{
    [Header("Current Status")]
    [SerializeField] Text currentStatus;
    [SerializeField] Dropdown statusDropdown;

    [Header("Controller")]    
    [SerializeField] GameState _GameState;
    [SerializeField] string UserId;
    DatabaseReference dbReference;

    static UsersOnlineController _instance;
    public static UsersOnlineController instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(instance.gameObject);
        }
        else _instance = this;
    }

    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState.OnDataReady += InitUsersOnlineController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        SetUserOnline();
    }

    public void InitUsersOnlineController()
    {
        Debug.Log("Init users online controller");
        dbReference.Child("users").Child(UserId).Child("status");

        SetUserOnline();
    }
    public void SetUserOnline()
    {
        //dbReference.Child("users-online").Child(UserId).Child("username").SetValueAsync(_GameState.username);
        print(_GameState.username + "Is Now ONLINE");
        currentStatus.text = "ONLINE";
        currentStatus.color = Color.green;
        dbReference.Child("users").Child(UserId).Child("status").SetValueAsync(true);
    }
    public void SetUserOffline()
    {
        //dbReference.Child("users-online").Child(UserId).SetValueAsync(null);
        print(_GameState.username + "Is Now OFFLINE");
        currentStatus.text = "OFFLINE";
        currentStatus.color = Color.red;
        dbReference.Child("users").Child(UserId).Child("status").SetValueAsync(false);
    }

    public void DropDownStatusChanged()
    {
        if (statusDropdown.value == 0)
        {
            SetUserOnline();
        }
        else if (statusDropdown.value == 1)
        {
            SetUserOffline();
        }
    }

    void OnApplicationQuit()
    {
        SetUserOffline();
    }
}