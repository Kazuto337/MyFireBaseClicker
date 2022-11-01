using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

public class FriendsController : MonoBehaviour
{
    public Text username;
    public GameObject message;
    public GameObject newFriendMessage;
    public GameObject requestPanel;
    //private User selectedUser;
    public GameObject friendSlot;
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


        string myId = PlayerPrefs.GetString("userID");

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
                Dictionary<string, object> request = (Dictionary<string, object>)userList["friendRequests"];
                foreach (var userDoc in request)
                {
                    Dictionary<string, object> userOnline = (Dictionary<string, object>)userDoc.Value;
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

                        GameObject friendSlot_ = Instantiate(friendSlot, friendSlot.transform.parent);
                        friendSlot_.gameObject.SetActive(true);
                        friendSlot_.GetComponentInChildren<Text>().text = fR.username;
                    }



                }
            }
        }
    }

    //public void ShowRequestInfo(Id id)
    //{
    //    selectedUser = UsersOnlineController.instance.userList[id.userId];
    //    string requestId = id + "r";
    //    if (!friendsDic.ContainsKey(selectedUser.userId) && !frRequests.ContainsKey(requestId))
    //    {
    //        requestPanel.SetActive(true);
    //        username.text = selectedUser.username;

    //    }
    //    else
    //    {
    //        string m = "Ya eres amigo de " + selectedUser.username + " o tienes una solicitud pendiente";
    //        NotificationController.instance.AddPopUpNotification(m);
    //    }

    //}

    //public void SendFriendRequest()
    //{

    //    mDatabase = FirebaseDatabase.DefaultInstance.RootReference;

    //    string myId = PlayerPrefs.GetString("userID");
    //    string username = PlayerPrefs.GetString("username");
    //    string requestId = myId + "r";

    //    FriendRequest friendRequest = new FriendRequest(myId, false, requestId, username);
    //    Debug.Log("Send" + myId);

    //    string json = JsonUtility.ToJson(friendRequest);
    //    mDatabase.Child("users").Child(selectedUser.userId).Child("friendRequests").Child(requestId).SetRawJsonValueAsync(json);

    //    requestPanel.gameObject.SetActive(false);
    //    message.gameObject.SetActive(true);
    //}

    //public void AcceptFriendRequest(Notification notificacion)
    //{
    //    string myId = PlayerPrefs.GetString("userID");
    //    string myUsername = PlayerPrefs.GetString("username");

    //    mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
    //    mDatabase.Child("users").Child(myId).Child("friendRequests").Child(notificacion.fR.requestId).Child("accepted").SetValueAsync(true);
    //    User user = new User(notificacion.fR.username, notificacion.fR.sender);
    //    User ownUsert = new User(myUsername, myId);

    //    string json1 = JsonUtility.ToJson(user);
    //    string json2 = JsonUtility.ToJson(ownUsert);
    //    mDatabase.Child("users").Child(user.userId).Child("friends").Child(myId).SetRawJsonValueAsync(json2);
    //    mDatabase.Child("users").Child(myId).Child("friends").Child(user.userId).SetRawJsonValueAsync(json1);
    //    newFriendMessage.SetActive(true);
    //    newFriendMessage.GetComponentInChildren<Text>().text = "Ahora eres amigo de " + user.username;
    //    DestroyImmediate(notificacion.gameObject);
    //}


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
