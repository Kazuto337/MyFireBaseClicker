using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using Firebase.Database;
using UnityEngine.UI;
using Firebase;

public class UsersOnlineController : MonoBehaviour
{
    // Start is called before the first frame update
    DatabaseReference mDatabase;
    GameState _GameState;
    string UserId;
    [SerializeField] FireBaseManager _ButtonLogout;
    [SerializeField] GameObject userLayout, userListPanel;
    public Dictionary<string, GameObject> userList = new Dictionary<string, GameObject>();

    void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        _GameState = GameObject.Find("UserController").GetComponent<GameState>();
        _GameState.OnDataReady += InitUsersOnlineController;
        UserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void InitUsersOnlineController()
    {
        FirebaseDatabase.DefaultInstance.LogLevel = LogLevel.Verbose;
        Debug.Log("Init users online controller");
        _ButtonLogout.OnLogOut += SetUserOffline;
        var userOnlineRef = FirebaseDatabase.DefaultInstance
        .GetReference("users-online");

        mDatabase.Child("users-online").ChildAdded += HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved += HandleChildRemoved;

        SetUserOnline();
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Dictionary<string, object> userConnected = (Dictionary<string, object>)args.Snapshot.Value;

        if ((string)userConnected["id"] != UserId)
        {
            userLayout.GetComponentInChildren<Text>().text = (string)userConnected["username"];
            userLayout.GetComponent<Data>().username = (string)userConnected["username"];
            userLayout.GetComponent<Data>().userId = (string)userConnected["id"];
            GameObject onlineUser = Instantiate(userLayout, userListPanel.transform);
            onlineUser.SetActive(true);
            userList.Add((string)userConnected["id"], onlineUser);
        }
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Dictionary<string, object> userDisconnected = (Dictionary<string, object>)args.Snapshot.Value;

        if (userList.ContainsKey((string)userDisconnected["id"]))
        {
            GameObject disconnectedUser;
            userList.TryGetValue((string)userDisconnected["id"], out disconnectedUser);
            DestroyImmediate(disconnectedUser);
            userList.Remove((string)userDisconnected["id"]);
        }
    }

    private void OnDestroy()
    {
        mDatabase.Child("users-online").ChildAdded -= HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved -= HandleChildRemoved;
    }

    private void SetUserOnline()
    {
        mDatabase.Child("users-online").Child(UserId).Child("username").SetValueAsync(_GameState.username);
        mDatabase.Child("users-online").Child(UserId).Child("id").SetValueAsync(UserId);
    }

    private void SetUserOffline()
    {
        mDatabase.Child("users-online").Child(UserId).SetValueAsync(null);
    }

    void OnApplicationQuit()
    {
        SetUserOffline();
        mDatabase.Child("users-online").ChildAdded -= HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved -= HandleChildRemoved;
    }

}