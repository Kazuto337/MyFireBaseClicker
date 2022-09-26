using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase;

public class PointsBehavior : MonoBehaviour
{
    public float points = 0f;
    [SerializeField] Text scoreText;

    public DatabaseReference dbReference;
    string userId;
    [SerializeField] GameObject gameScreen, saveScreen;

    private void Start()
    {
        scoreText.text = (points.ToString());
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = FireBaseManager.instance.user.UserId;
    }
    private void Update()
    {
        scoreText.text = (points.ToString());
    }

    public void AddPoints()
    {
        points++;
    }

    public void SaveButton()
    {
        UserData data = new UserData();
        data.username = FireBaseManager.instance.user.DisplayName;
        data.score = float.Parse(scoreText.text);

        string json = JsonUtility.ToJson(data);
        dbReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
        gameScreen.SetActive(true);
        saveScreen.SetActive(false);
        points = 0;
    }

}
