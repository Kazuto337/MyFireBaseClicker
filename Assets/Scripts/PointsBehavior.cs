using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

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
    public List<GameObject> scoreboard;



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

        FirebaseDatabase.DefaultInstance.GetReference("users/").OrderByChild("scores").GetValueAsync().ContinueWithOnMainThread(task =>//busca los objetos en el json de tipo user y los organiza por score
        {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)//una vez se complete hace algo
            {
                DataSnapshot snapshot = task.Result;
                int i = 0;
                foreach (DataSnapshot item in snapshot.Children)
                {
                    string username = item.Child("username").Value.ToString();
                    float score = float.Parse(item.Child("score").Value.ToString());

                    print(username + " " + score);
                    scoreboard[i].GetComponent<ScoreElement>().usernameText.text = username;
                    scoreboard[i].GetComponent<ScoreElement>().scoreText.text = score.ToString();

                    if (i >= scoreboard.Count)
                    {
                        break;
                    }
                    i++;
                }
            }
        });
    }
}
