using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchedGameSceneHandler : MonoBehaviour
{
    public Text[] playerScores;
    private int points = 0;
    private FirebaseDatabase _database;
    private DatabaseReference _refNew;
    private DatabaseReference _refCurrentGame;
    private string opponent;
    private string playerID;
    private void Start()
    {
        playerScores[0].text = "0";
        playerScores[1].text = "0";
        points = 0;
        _database = FirebaseDatabase.DefaultInstance;
        playerID = GameManager.instance.currentGameInfo.localPlayerId;
        opponent = GameManager.instance.currentGameInfo.opponentPlayerId;
        _refCurrentGame = _database.GetReference($"matchmaking/games/{opponent }");
        _refCurrentGame.ValueChanged += RefCurrentGameOnValueChanged;
    }

    
    private void RefCurrentGameOnValueChanged(object sender, ValueChangedEventArgs e)
    {
        var json = e.Snapshot.GetRawJsonValue();
        if (string.IsNullOrEmpty(json)) return;
        Game game = JsonUtility.FromJson<Game>(json);
        playerScores[1].text = game.playerPoints.ToString();
    }
    
    public void AddPoints()
    {
        points++;
        playerScores[0].text = points.ToString();
        Game game = new Game(opponent)
        {
            playerReady = true,
            playerPoints = points
        };
        _database.GetReference($"matchmaking").Child("games").Child(playerID).SetRawJsonValueAsync(JsonUtility.ToJson(game));
    }

    private void OnDestroy()
    {
        _refCurrentGame.ValueChanged -= RefCurrentGameOnValueChanged;
    }

    public void Leave()
    {
        FirebaseDatabase.DefaultInstance.GetReference($"matchmaking").Child("games").Child(GameManager.instance.currentGameInfo.localPlayerId).RemoveValueAsync();
        QueueStatus queue = new QueueStatus
        {
            value = "Leave"
        };
        FirebaseDatabase.DefaultInstance.GetReference($"matchmaking").Child("queue").Child(GameManager.instance.currentGameInfo.localPlayerId).SetRawJsonValueAsync(JsonUtility.ToJson(queue));
        SceneManager.LoadScene("Main");
    }
}
