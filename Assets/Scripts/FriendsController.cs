using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using Firebase.Extensions;

public class FriendsController : MonoBehaviour
{
    public Text futureFriend;
    public GameObject message;
    public GameObject requestAccepted;
    public GameObject requestPanel;
    private string selectedUser;
    private string selectedId;
    [SerializeField] GameObject friendLayout, friendListPanel;
    private DatabaseReference mDatabase;
    Dictionary<string, FriendRequest> frRequests = new Dictionary<string, FriendRequest>();
    Dictionary<string, OwnFriend> friendsDic = new Dictionary<string, OwnFriend>();

    private void OnEnable()
    {
        GameState.OnMenuEnter += InitRequestController;
        UsersOnlineController.onUserChange += GetFriendStatus;
    }
    private void OnDisable()
    {
        GameState.OnMenuEnter -= InitRequestController;
        UsersOnlineController.onUserChange -= GetFriendStatus;
    }

    public void GetFriendStatus(string id, bool status)
    {
        if (friendsDic.ContainsKey(id))
        {
            if (status)
            {
                string mssg = "Se ha conectado tu amigo " + friendsDic[id].username;
                NotificationController.instance.AddPopUpNotification(mssg);
            }
            else
            {
                string mssg = "Se ha desconectado tu amigo " + friendsDic[id].username;
                NotificationController.instance.AddPopUpNotification(mssg);
            }
        }
    }

    public void InitRequestController()
    {
        string myId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var userOnlineRef = FirebaseDatabase.DefaultInstance
        .GetReference($"users/{myId}");

        userOnlineRef.ValueChanged += HandleRequests;

        GetUsername(myId);
    }

    private void HandleRequests(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            return;
        }

        if (args.Snapshot.HasChildren)
        {
            Dictionary<string, object> userList = (Dictionary<string, object>)args.Snapshot.Value;
            if (userList.ContainsKey("friendRequests"))
            {
                Dictionary<string, object> requests = (Dictionary<string, object>)userList["friendRequests"];
                foreach (var request in requests)
                {
                    Dictionary<string, object> userOnline = (Dictionary<string, object>)request.Value;
                    FriendRequest friendRequest = new FriendRequest((string)userOnline["sender"], (bool)userOnline["accepted"], (string)userOnline["requestId"], (string)userOnline["username"]);
                    if (!frRequests.ContainsKey(friendRequest.requestId))
                    {
                        if (!friendRequest.accepted)
                        {
                            frRequests.Add(friendRequest.requestId, friendRequest);
                            NotificationController.instance.AddNotificationFriendRequest(friendRequest);
                        }
                    }
                }
            }

            if (userList.ContainsKey("friends"))
            {
                Dictionary<string, object> friends = (Dictionary<string, object>)userList["friends"];

                foreach (var item in friends)
                {
                    Dictionary<string, object> friend = (Dictionary<string, object>)item.Value;
                    OwnFriend frienRequest = new OwnFriend((string)friend["id"], (string)friend["userName"]);

                    if (!friendsDic.ContainsKey(frienRequest.id))
                    {
                        friendsDic.Add(frienRequest.id, frienRequest);
                        NotificationController.instance.AddNotificationFriendAccepted(frienRequest);

                        GameObject _friendSlot = Instantiate(friendLayout, friendListPanel.transform);
                        _friendSlot.gameObject.SetActive(true);
                        _friendSlot.GetComponentInChildren<Text>().text = frienRequest.username;
                    }
                }
            }
        }
    }

    public void ShowRequestInfo(Data data)
    {
        selectedUser = data.username;
        selectedId = data.userId;
        string requestId = data + "request";
        if (!friendsDic.ContainsKey(selectedId) && !frRequests.ContainsKey(requestId))
        {
            requestPanel.SetActive(true);
            futureFriend.text = selectedUser;

        }
        else
        {
            string m = "Ya eres amigo de " + selectedUser + " o tienes una solicitud pendiente";
            NotificationController.instance.AddPopUpNotification(m);
        }

    }

    public void SendFriendRequest()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;

        string myId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string username = PlayerPrefs.GetString("Username");
        string requestId = myId + "request";

        FriendRequest friendRequest = new FriendRequest(myId, false, requestId, username);
        Debug.Log("Send" + myId);

        string json = JsonUtility.ToJson(friendRequest);
        mDatabase.Child("users").Child(selectedId).Child("friendRequests").Child(requestId).SetRawJsonValueAsync(json);

        requestPanel.gameObject.SetActive(false);
        message.gameObject.SetActive(true);
    }

    public void AcceptFriendRequest(Notification notificacion)
    {
        string myId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string myUsername = PlayerPrefs.GetString("Username");

        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabase.Child("users").Child(myId).Child("friendRequests").Child(notificacion.fR.requestId).Child("accepted").SetValueAsync(true);
        OwnFriend user = new OwnFriend(notificacion.fR.sender, notificacion.fR.username);
        OwnFriend ownUsert = new OwnFriend(myId, myUsername);

        string json1 = JsonUtility.ToJson(user);
        string json2 = JsonUtility.ToJson(ownUsert);
        mDatabase.Child("users").Child(user.id).Child("friends").Child(myId).SetRawJsonValueAsync(json2);
        mDatabase.Child("users").Child(myId).Child("friends").Child(user.id).SetRawJsonValueAsync(json1);
        requestAccepted.SetActive(true);
        requestAccepted.GetComponentInChildren<Text>().text = "Ahora eres amigo de " + user.username;
        DestroyImmediate(notificacion.gameObject);
    }

    void GetUsername(string userId)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + userId + "/username").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("GetValueAsync encountered an error: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                PlayerPrefs.SetString("Username", snapshot.Value.ToString());
            }
        });
    }

    [System.Serializable]
    public class FriendRequest
    {
        public string sender;
        public bool accepted;
        public string requestId;
        public string username;
        public FriendRequest(string sender, bool accepted, string requestId, string username)
        {
            this.sender = sender;
            this.accepted = accepted;
            this.requestId = requestId;
            this.username = username;
        }
    }

    [System.Serializable]
    public class OwnFriend
    {

        public string id;
        public string username;
        public OwnFriend(string id, string username)
        {
            this.id = id;
            this.username = username;
        }
    }
}
