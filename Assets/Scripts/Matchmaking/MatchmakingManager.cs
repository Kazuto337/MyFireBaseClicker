using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIs;
using Firebase.Database;
using Serializables;
using UnityEngine;

namespace Managers
{
    public class MatchmakingManager : MonoBehaviour
    {
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> queueListener;

        public void JoinQueue(string playerId, Action<string> onGameFound, Action<AggregateException> fallback)
        {
            StartCoroutine(ProccessQueue(playerId, onGameFound, fallback));
        }

        public IEnumerator ProccessQueue(string playerId, Action<string> onGameFound,
            Action<AggregateException> fallback)
        {
            var DBTask = FirebaseDatabase.DefaultInstance.RootReference.Child("matchmaking").GetValueAsync();
            yield return new WaitUntil(() => DBTask.IsCompleted);
            if (DBTask.Exception != null)
            {
                Debug.LogError(message:$"Failed to register task with: {DBTask.Exception}");
            }
            else
            {
                if (DBTask.Result.ChildrenCount <= 0)
                {
                    AddToQueue(playerId, onGameFound, fallback);
                }
                else
                {
                    bool gameFound = false;
                    foreach (DataSnapshot child in DBTask.Result.Children)
                    {
                        if (child.Child("placeholder").Value.ToString() == "True")
                        {
                            if (child.Value.ToString() != MainManager.Instance.currentLocalPlayerId)
                            {
                                var GameId = child.Key;
                                CreateGame(GameId, GameId, MainManager.Instance.currentLocalPlayerId);
                                DatabaseAPI.PostJSON($"matchmaking/{GameId}/placeholder", "False", () => onGameFound(
                                    GameId), fallback);
                            }
                        }
                    }
                }
            }
        }

        public void AddToQueue(string playerId, Action<string> onGameFound, Action<AggregateException> fallback)
        {
            DatabaseAPI.PostObject($"matchmaking/{playerId}/placeholder", "True",
                () => queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{playerId}/placeholder",
                    args =>
                    {
                        var gameId =
                            StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as
                                string;
                        if (gameId == "True") return;
                        LeaveQueue(playerId, () => onGameFound(
                            gameId), fallback);
                    }, fallback), fallback);
        }

        public void LeaveQueue(string playerId, Action callback, Action<AggregateException> fallback)
        {
            DatabaseAPI.StopListeningForValueChanged(queueListener);
            DatabaseAPI.PostJSON($"matchmaking/{playerId}/placeholder", "False", callback, fallback);
        }

        public bool GameOn = false;

        public void CreateGame(string gameId, string ID1, string ID2)
        {
            Games G = new Games();
            G.gameId = gameId;
            gameInfo GF = new gameInfo();
            GF.gameId = gameId;
            GF.playerIds = new PlayerIds();
            GF.playerIds._0 = ID1;
            GF.playerIds._1 = ID2;
            G.gameInfo = GF;
            FirebaseDatabase.DefaultInstance.GetReference($"games/{gameId}").SetRawJsonValueAsync(JsonUtility.ToJson(G));
        }
    }
}
[Serializable]
public class gameInfo
{
    public string gameId;
    public PlayerIds playerIds;
}

[Serializable]
public class PlayerIds
{
    public string _0;
    public string _1;
}

public class Games
{
    public string gameId;
    public gameInfo gameInfo;
}