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
    public PlayerScoreHandler[] playerScores;

    private void Start()
    {
        playerScores[0].playerId = GameManager.instance.currentGameInfo.localPlayerId;
        playerScores[0].Init();
        playerScores[1].playerId = GameManager.instance.currentGameInfo.opponentPlayerId;
        playerScores[1].Init();
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
