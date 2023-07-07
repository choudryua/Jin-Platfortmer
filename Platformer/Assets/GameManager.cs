using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake()
    {
        instance = this; }


    public float _currentTime;
    private float highScore;
    public TextMeshProUGUI _deathScore;
    private float deathScore;

    void Start()
    {
        _currentTime = 0;
        highScore = _currentTime;
        deathScore = 0;
    }
    #region TIMER

    public TextMeshProUGUI timer;
    public TextMeshProUGUI _highScore;

    public bool countUp;

    void FixedUpdate()
    {
        _currentTime = countUp ? _currentTime += Time.deltaTime : _currentTime -= Time.deltaTime;
        float minutes = Mathf.FloorToInt(_currentTime / 60);
        float seconds = Mathf.FloorToInt(_currentTime % 60);

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    #endregion

    #region HIGHSCORE
    public void HighScore()
    {
        PlayerPrefs.SetFloat("HighScore", _currentTime);
        highScore = PlayerPrefs.GetFloat("HighScore");
        _highScore.text = highScore.ToString();
    }
    #endregion

    public void DeathScore()
    {
        deathScore++;
        _deathScore.text = deathScore.ToString();
    }
}
