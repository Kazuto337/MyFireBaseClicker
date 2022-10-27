using System.Collections;
using System.Collections.Generic;
using APIs;
using Firebase.Database;
using Managers;
using Serializables;
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
        StartCoroutine(ProccessQueue(gameId, MainManager.Instance.currentLocalPlayerId));

        searchingPanel.SetActive(false);
        foundPanel.SetActive(true);
    }

    
    public IEnumerator ProccessQueue(string playerId, string localPlayerID)
    {
        var DBTask = FirebaseDatabase.DefaultInstance.RootReference.Child("games").Child(playerId).Child("gameInfo").GetValueAsync();
        yield return new WaitUntil(() => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogError(message:$"Failed to register task with: {DBTask.Exception}");
        }
        else
        {
            GameManager.instance.currentGameInfo = new GameInfo();
            GameManager.instance.currentGameInfo.gameId = DBTask.Result.Child("gameId").ToString();
            GameManager.instance.currentGameInfo.playersIds = new string[2];
            GameManager.instance.currentGameInfo.playersIds[0] =  DBTask.Result.Child("_0").ToString();
            GameManager.instance.currentGameInfo.playersIds[1] =  DBTask.Result.Child("_1").ToString();
            GameManager.instance.currentGameInfo.localPlayerId = localPlayerID;
            MainManager.Instance.matchmakingManager.GameOn = true;
            SceneManager.LoadScene("MatchedGame");
        }
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