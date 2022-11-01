using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Serializables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MatchmakingManager : MonoBehaviour
    {
        private FirebaseDatabase _database;
        private DatabaseReference _refNew;
        private DatabaseReference _refCurrentGame;
        public bool gameFound;
        private bool gameReady, opponentReady;
        private string opponent;

        public static MatchmakingManager instance;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else instance = this;
        }

        private void Start()
        {
            _database = FirebaseDatabase.DefaultInstance;
            gameFound = false;
            _refNew = _database.GetReference("matchmaking/queue");
        }

        private void RefNewOnValueChanged(object sender, ValueChangedEventArgs e)
        {
            if(gameFound) return;
            var json = e.Snapshot.GetRawJsonValue();
            if (string.IsNullOrEmpty(MainManager.Instance.currentLocalPlayerId))
            {
                return;
            }

            if (string.IsNullOrEmpty(json)) return;
            foreach (DataSnapshot child in e.Snapshot.Children)
            {
                if (String.CompareOrdinal(child.Key, MainManager.Instance.currentLocalPlayerId) == 0) continue;
                if (child.Child("value").Value.ToString() != "Waiting") return;

                QueueStatus queue = new QueueStatus
                {
                    value = "OnGame"
                };
                _refNew.ValueChanged -= RefNewOnValueChanged;
                
                _database.GetReference($"matchmaking").Child("queue").Child(MainManager.Instance.currentLocalPlayerId)
                    .SetRawJsonValueAsync(JsonUtility.ToJson(queue));
                
                gameFound = true;
                Game game = new Game(child.Key);
                opponent = child.Key;
                _refCurrentGame = _database.GetReference($"matchmaking/games/{opponent}");
                _refCurrentGame.ValueChanged += RefCurrentGameOnValueChanged;
                _database.GetReference($"matchmaking").Child("games").Child(MainManager.Instance.currentLocalPlayerId).SetRawJsonValueAsync(JsonUtility.ToJson(game));
                gameReady = false;
                opponentReady = false;
            }
        }

        private void RefCurrentGameOnValueChanged(object sender, ValueChangedEventArgs e)
        {
            var json = e.Snapshot.GetRawJsonValue();
            Debug.LogError(json);
            if (string.IsNullOrEmpty(json)) return;
            Game game = JsonUtility.FromJson<Game>(json);
            if (!game.playerReady) return;
            opponentReady = true;
            _refCurrentGame.ValueChanged -= RefCurrentGameOnValueChanged;
            StartGame();
        }

        public void JoinQueue()
        {
            if (gameFound) return;
            gameFound = false;
            QueueStatus queue = new QueueStatus
            {
                value = "Waiting"
            };
            _database.GetReference($"matchmaking").Child("queue").Child(MainManager.Instance.currentLocalPlayerId).SetRawJsonValueAsync(JsonUtility.ToJson(queue));
            _refNew.ValueChanged += RefNewOnValueChanged;
        }
        public void SetReady()
        {
            Game game = new Game(opponent)
            {
                playerReady = true
            };
            gameReady = true;
            _database.GetReference($"matchmaking").Child("games").Child(MainManager.Instance.currentLocalPlayerId).SetRawJsonValueAsync(JsonUtility.ToJson(game));
            StartGame();
        }
        public void LeaveQueue()
        {
            if (gameFound) return;
            gameFound = false;
            QueueStatus queue = new QueueStatus
            {
                value = "Leave"
            };
            _database.GetReference($"matchmaking").Child("queue").Child(MainManager.Instance.currentLocalPlayerId).SetRawJsonValueAsync(JsonUtility.ToJson(queue));
            _refNew.ValueChanged -= RefNewOnValueChanged;
        }
        private void OnDestroy()
        {
            if (_refNew != null)
            {
                _refNew.ValueChanged -= RefNewOnValueChanged;
            }
            if (_refCurrentGame != null)
            {
                _refCurrentGame.ValueChanged -= RefCurrentGameOnValueChanged;
            }
            _refNew = null;
            _database = null;
        }

        private void StartGame()
        {
            if (!gameReady || !opponentReady) return;

            GameManager.instance.currentGameInfo = new GameInfo();
            SceneManager.LoadScene("MatchedGame");
        }
    }
}

[Serializable]
public class QueueStatus
{
    public string value;
}

[Serializable]
public class Game
{
    public string opponent;
    public int playerPoints;
    public bool playerReady;

    public Game(string opponent)
    {
        this.opponent = opponent;
        playerPoints = 0;
        playerReady = false;
    }
}

