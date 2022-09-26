using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreElement : MonoBehaviour
{
    public Text usernameText, scoreText;

    public void NewScoreElement(string username, float score)
    {
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}
