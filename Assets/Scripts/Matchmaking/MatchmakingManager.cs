using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using APIs;
using Firebase.Database;
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
                                Debug.Log(GameId);
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
    }
}