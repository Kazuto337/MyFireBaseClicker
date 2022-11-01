using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FriendsController;

public class NotificationController : MonoBehaviour
{
    public static NotificationController instance;

    public GameObject notification;
    public GameObject newFriendRequest;
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
        string text = fR.username + " te ha enviado una solicitud de amigo";
        GameObject newNotification = Instantiate(notification, notification.transform.parent);
        newNotification.SetActive(true);
        newNotification.GetComponent<Notification>().text.text = text;
        newNotification.GetComponent<Notification>().fR = fR;
    }
    public void AddNotificationFriendAccepted(OwnFriend friend)
    {
        panel.SetActive(true);

        Debug.Log(friend.username);
        string text = friend.username + " ha comenzado a ser tu amigo";
        GameObject newNotification = Instantiate(newFriendRequest, notification.transform.parent);
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
