using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;

public class FriendsController : MonoBehaviour
{
    [SerializeField] Button addButton;
    [SerializeField] Text friendText;

    private DatabaseReference friendDatabase;
    private FirebaseUser user;
    public string friendId;

    private void Start()
    {
        friendDatabase = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void AddFriend()
    {
        friendDatabase.Child("users").Child(user.UserId).Child("friends").Push().SetValueAsync(friendId);
        addButton.gameObject.SetActive(false);
        friendText.gameObject.SetActive(true);
    }
}
