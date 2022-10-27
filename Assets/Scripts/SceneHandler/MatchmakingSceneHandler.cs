using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingSceneHandler : MonoBehaviour
{
    public GameObject searchingPanel;
    public GameObject foundPanel;

    private bool readyingUp;
    private string gameId;

    private void Start() => JoinQueue();

    private void JoinQueue()
    {
        MainManager.Instance.matchmakingManager.GameOn = false;
        MainManager.Instance.matchmakingManager.JoinQueue(MainManager.Instance.currentLocalPlayerId, gameId =>
            {
                MainManager.Instance.matchmakingManager.GameOn = true;
                this.gameId = gameId;
            },
            Debug.Log);
    }

    private void Update()
    {
        if (!MainManager.Instance.matchmakingManager.GameOn || readyingUp) return;
        readyingUp = true;
        GameFound();
    }

    private void GameFound()
    {
        Debug.Log("GameFound");
        MainManager.Instance.gameManager.GetCurrentGameInfo(gameId, MainManager.Instance.currentLocalPlayerId,
            gameInfo =>
            {
            }, Debug.Log);

        MainManager.Instance.matchmakingManager.GameOn = true;
        searchingPanel.SetActive(false);
        foundPanel.SetActive(true);
        SceneManager.LoadScene("MatchedGame");
    }

    public void LeaveQueue()
    {
        if (MainManager.Instance.matchmakingManager.GameOn) MainManager.Instance.gameManager.StopListeningForAllPlayersReady();
        else
            MainManager.Instance.matchmakingManager.LeaveQueue(MainManager.Instance.currentLocalPlayerId,
                () => Debug.Log("Left queue successfully"), Debug.Log);
        SceneManager.LoadScene("Main");
    }

    public void Ready() =>
        MainManager.Instance.gameManager.SetLocalPlayerReady(() => Debug.Log("You are now ready!"), Debug.Log);
}