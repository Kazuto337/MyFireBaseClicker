using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;

public class PointsBehavior : MonoBehaviour
{
    [Header("Score")]
    public float points = 0f;
    [SerializeField] Text scoreText;
    [Space(5f)]

    [Header("ScoreBoard")]
    public DatabaseReference dbReference;
    string userId;
    [SerializeField] GameObject gameScreen, saveScreen, statsScreen;
    public GameObject scoreElement;
    public Transform scoreboardContent;



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

    public void StatsButton()
    {
        statsScreen.SetActive(true);
        gameScreen.SetActive(false);

        var dbTask = dbReference.Child("users").OrderByChild("score").GetValueAsync();
        DataSnapshot snapshot = dbTask.Result;

        foreach (Transform child in scoreboardContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (DataSnapshot item in snapshot.Children.Reverse<DataSnapshot>())
        {
            string username = item.Child("username").Value.ToString();
            float score = float.Parse(item.Child("score").Value.ToString());

            GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
            scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username , score);
        }        
    }

}
