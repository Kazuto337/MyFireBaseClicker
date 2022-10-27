using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIs;
using Firebase.Database;
using Serializables;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else instance = this;
    }

    public void ChangeScene(int _sceneIndex)
    {
        SceneManager.LoadScene(_sceneIndex);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public GameInfo currentGameInfo;

    private Dictionary<string, bool> readyPlayers;
    private KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>> readyListener;
    private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> localPlayerTurnListener;
    private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> currentGameInfoListener;

    private readonly Dictionary<string, KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>>
        pointsListeners =
            new Dictionary<string, KeyValuePair<DatabaseReference, EventHandler<ChildChangedEventArgs>>>();

    public void GetCurrentGameInfo(string gameId, string localPlayerId, Action<GameInfo> callback,
        Action<AggregateException> fallback)
    {
        currentGameInfoListener =
            DatabaseAPI.ListenForValueChanged($"games/{gameId}/gameInfo", args =>
            {
                if (!args.Snapshot.Exists) return;

                var gameInfo =
                    JsonUtility.FromJson<GameInfo>(args.Snapshot.GetRawJsonValue());
                currentGameInfo = gameInfo;
                currentGameInfo.localPlayerId = localPlayerId;
                DatabaseAPI.StopListeningForValueChanged(currentGameInfoListener);
                callback(currentGameInfo);
            }, fallback);
    }

    public void SetLocalPlayerReady(Action callback, Action<AggregateException> fallback)
    {
        DatabaseAPI.PostObject($"games/{currentGameInfo.gameId}/ready/{currentGameInfo.localPlayerId}", true,
            callback,
            fallback);
    }

    public void ListenForAllPlayersReady(IEnumerable<string> playersId, Action<string> onNewPlayerReady,
        Action onAllPlayersReady,
        Action<AggregateException> fallback)
    {
        readyPlayers = playersId.ToDictionary(playerId => playerId, playerId => false);
        readyListener = DatabaseAPI.ListenForChildAdded($"games/{currentGameInfo.gameId}/ready/", args =>
        {
            readyPlayers[args.Snapshot.Key] = true;
            onNewPlayerReady(args.Snapshot.Key);
            if (!readyPlayers.All(readyPlayer => readyPlayer.Value)) return;
            StopListeningForAllPlayersReady();
            onAllPlayersReady();
        }, fallback);
    }

    public void StopListeningForAllPlayersReady() => DatabaseAPI.StopListeningForChildAdded(readyListener);

    public void SendPoints(Points points, Action callback, Action<AggregateException> fallback)
    {
        DatabaseAPI.PushObject($"games/{currentGameInfo.gameId}/{currentGameInfo.localPlayerId}/points/", points,
            () =>
            {
                Debug.Log("Points sent successfully!");
                callback();
            }, fallback);
    }

    public void ListenForPoints(string playerId, Action<Points> onNewPoints, Action<AggregateException> fallback)
    {
        pointsListeners.Add(playerId, DatabaseAPI.ListenForChildAdded(
            $"games/{currentGameInfo.gameId}/{playerId}/points/",
            args => onNewPoints(
                JsonUtility.FromJson<Points>(args.Snapshot.GetRawJsonValue())),
            fallback));
    }

    public void StopListeningForPoints(string playerId)
    {
        DatabaseAPI.StopListeningForChildAdded(pointsListeners[playerId]);
        pointsListeners.Remove(playerId);
    }
}
