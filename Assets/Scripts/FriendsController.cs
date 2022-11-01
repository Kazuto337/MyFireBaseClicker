using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

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

    public void GetFriendStatus(string id, bool status)
    {
        if (friendsDic.ContainsKey(id))
        {
            if (status)
            {
                string m = "Se ha conectado tu amigo " + friendsDic[id].username;
                NotificationController.instance.AddPopUpNotification(m);

            }
            else
            {
                string m = "Se ha desconectado tu amigo " + friendsDic[id].username;
                NotificationController.instance.AddPopUpNotification(m);

            }
        }
    }
    public void InitRequestController()
    {


        string myId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var userOnlineRef = FirebaseDatabase.DefaultInstance
        .GetReference($"users/{myId}");

        userOnlineRef.ValueChanged += HandleRequests;

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
                    FriendRequest fR = new FriendRequest((string)userOnline["sender"], (bool)userOnline["accepted"], (string)userOnline["requestId"], (string)userOnline["username"]);
                    if (!frRequests.ContainsKey(fR.requestId))
                    {
                        if (!fR.accepted)
                        {
                            frRequests.Add(fR.requestId, fR);
                            NotificationController.instance.AddNotificationFriendRequest(fR);
                        }
                    }
                }
            }
            if (userList.ContainsKey("friends"))
            {
                Dictionary<string, object> friends = (Dictionary<string, object>)userList["friends"];
                foreach (var userDoc in friends)
                {
                    Dictionary<string, object> friend = (Dictionary<string, object>)userDoc.Value;
                    OwnFriend fR = new OwnFriend((string)friend["id"], (string)friend["userName"]);

                    if (!friendsDic.ContainsKey(fR.Id))
                    {
                        friendsDic.Add(fR.Id, fR);
                        NotificationController.instance.AddNotificationFriendAccepted(fR);

                        GameObject _friendSlot = Instantiate(friendLayout, friendListPanel.transform);
                        _friendSlot.gameObject.SetActive(true);
                        _friendSlot.GetComponentInChildren<Text>().text = fR.username;
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
        string username = mDatabase.Child("users/" + myId).Child("username").GetValueAsync().ToString();
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
        string myUsername = mDatabase.Child("users/" + myId).Child("username").GetValueAsync().ToString();

        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabase.Child("users").Child(myId).Child("friendRequests").Child(notificacion.fR.requestId).Child("accepted").SetValueAsync(true);
        OwnFriend user = new OwnFriend(notificacion.fR.sender, notificacion.fR.username);
        OwnFriend ownUsert = new OwnFriend(myId, myUsername);

        string json1 = JsonUtility.ToJson(user);
        string json2 = JsonUtility.ToJson(ownUsert);
        mDatabase.Child("users").Child(user.Id).Child("friends").Child(myId).SetRawJsonValueAsync(json2);
        mDatabase.Child("users").Child(myId).Child("friends").Child(user.Id).SetRawJsonValueAsync(json1);
        requestAccepted.SetActive(true);
        requestAccepted.GetComponentInChildren<Text>().text = "Ahora eres amigo de " + user.username;
        DestroyImmediate(notificacion.gameObject);
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

        public string Id;
        public string username;
        public OwnFriend(string Id, string username)
        {
            this.Id = Id;
            this.username = username;
        }
    }
}
