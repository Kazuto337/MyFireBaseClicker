using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.EventSystems;
using Firebase.Database;
using UnityEngine.UI;

public class UsersBehav : MonoBehaviour
{
    public DatabaseReference dbReference;
    public FirebaseUser user;
    [SerializeField] GameObject userLayout , userListPanel;
    [SerializeField] List<GameObject> usersList;

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }
    private void OnEnable()
    {
        //StartCoroutine(SetOnlineUserList());
    }

    public void RefreshList()
    {
        foreach (GameObject item in usersList)
        {
            Destroy(item);
        }
        usersList.Clear();
        //StartCoroutine(SetOnlineUserList());
    }

    //public IEnumerator SetOnlineUserList()
    //{
    //    var usersInDB = dbReference.Child("users").GetValueAsync();
        
    //    yield return new WaitUntil(() => usersInDB.IsCompleted);

    //    foreach (DataSnapshot item in usersInDB.Result.Children)
    //    {
    //        if (bool.Parse(item.Child("status").Value.ToString()) != false && item.Key != user.UserId && item.Child("username").Value != null)
    //        {
    //            print(item.Child("username").Value.ToString());
    //            userLayout.GetComponentInChildren<Text>().text = item.Child("username").Value.ToString();
    //            GameObject j = Instantiate(userLayout, userListPanel.transform);
    //            j.GetComponent<FriendsController>().friendId = item.Key;
    //            usersList.Add(j);
    //        }            
    //    }
    //}
}
