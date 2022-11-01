using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Serializables;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreHandler : MonoBehaviour
{
    public string playerId;
    // private Points points;
    private bool avaiable;
    private Text _text;

    public void Init()
    {
        // points = new Points(0);
        _text = GetComponent<Text>();
        // MainManager.Instance.gameManager.ListenForPoints(playerId, ExecuteMove, Debug.Log);
        avaiable = true;
    }

    public void AddPoints()
    {
        // points.points++;
        // MainManager.Instance.gameManager.SendPoints(points,
        //     () => MainManager.Instance.gameManager.SendPoints(points, () => ExecuteMove(points),
        //         Debug.Log), error =>
        //     {
        //         Debug.Log(error);
        //     });
    }

    //
    // private void ExecuteMove(Points points)
    // {
    //     this.points = points;
    //     _text.text = this.points.points.ToString();
    // }
}
