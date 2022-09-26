using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsBehavior : MonoBehaviour
{
    public float points = 0f;
    [SerializeField] Text pointText;

    private void Start()
    {
        pointText.text = (points.ToString() + " Points");
    }

    public void AddPoints()
    {
        points++;
        pointText.text = (points.ToString() + " Points");
    }
}
