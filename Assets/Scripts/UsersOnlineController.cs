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
    [SerializeField] List<GameObject> usersList;

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
        string a = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
        print("el usuario actual es" + a);
        Dictionary<string, object> userConnected = (Dictionary<string, object>)args.Snapshot.Value;
        foreach (var item in userConnected)
        {
            if (item.Value.ToString() != a)
            {
                userLayout.GetComponentInChildren<Text>().text = userConnected["username"].ToString();
                GameObject friend = Instantiate(userLayout, userListPanel.transform);
                usersList.Add(friend);
                print("Usuarios Online son " + item.Value.ToString());
            }
        }
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        foreach (GameObject item in usersList)
        {
            Destroy(item);
        }

        Dictionary<string, object> userDisconnected = (Dictionary<string, object>)args.Snapshot.Value;
        Debug.Log(userDisconnected["username"] + " is offline");

    }

    private void OnDestroy()
    {
        mDatabase.Child("users-online").ChildAdded -= HandleChildAdded;
        mDatabase.Child("users-online").ChildRemoved -= HandleChildRemoved;
    }

    private void SetUserOnline()
    {
        mDatabase.Child("users-online").Child(UserId).Child("username").SetValueAsync(_GameState.username);
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