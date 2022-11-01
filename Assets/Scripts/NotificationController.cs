using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FriendsController;

public class NotificationController : MonoBehaviour
{
    public static NotificationController instance;

    public GameObject notification;
    public GameObject newFriend;
    public GameObject popUp;
    public GameObject panel;

    private void Awake()
    {
        instance = this;
    }

    public void AddNotificationFriendRequest(FriendRequest fR)
    {
        panel.SetActive(true);

        Debug.Log(fR.username);
        string text = fR.username + " has sent you a friend request.";
        GameObject newNotification = Instantiate(notification, notification.transform.parent);
        newNotification.SetActive(true);
        newNotification.GetComponent<Notification>().text.text = text;
        newNotification.GetComponent<Notification>().fR = fR;
    }

    public void AddNotificationFriendAccepted(OwnFriend friend)
    {
        panel.SetActive(true);

        Debug.Log(friend.username);
        string text = friend.username + " and you are friend now.";
        GameObject newNotification = Instantiate(newFriend, notification.transform.parent);
        newNotification.SetActive(true);
        newNotification.GetComponent<Notification>().text.text = text;
    }

    public void AddPopUpNotification(string message)
    {
        StartCoroutine(ShowNotification(message));
    }

    IEnumerator ShowNotification(string message)
    {
        popUp.SetActive(true);
        popUp.GetComponentInChildren<Text>().text = message;
        yield return new WaitForSeconds(5);
        popUp.SetActive(false);

    }
}
