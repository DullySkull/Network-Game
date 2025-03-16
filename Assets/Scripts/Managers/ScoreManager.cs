using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")] 
    public int score = 0;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if(scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void AddScore()
    {
        score ++;
        UpdateScoreText();
    }

}
