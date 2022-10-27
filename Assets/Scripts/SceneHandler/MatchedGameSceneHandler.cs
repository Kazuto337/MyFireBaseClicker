using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchedGameSceneHandler : MonoBehaviour
{
    public PlayerScoreHandler[] playerScores;
    private void Start()
    {
        var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == MainManager.Instance.currentLocalPlayerId)
            {
                playerScores[0].playerId =  players[i];
                playerScores[0].Init();
            }
            else
            {
                playerScores[1].playerId =  players[i];
                playerScores[1].Init(); 
            }
        }
    }

    public void Leave()
    {
        var players = MainManager.Instance.gameManager.currentGameInfo.playersIds;
        foreach (var player in players.Where(p => p != MainManager.Instance.currentLocalPlayerId))
            MainManager.Instance.gameManager.StopListeningForPoints(player);
        SceneManager.LoadScene("Main");
    }
}
