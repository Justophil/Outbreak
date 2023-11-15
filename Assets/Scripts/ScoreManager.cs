using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    Text Score;

    public void Start(){
        Score = GetComponent<Text>();
    }

    public void Update(){
       // Score.text = "Score: " + GameManager.Score; 
    }
}
