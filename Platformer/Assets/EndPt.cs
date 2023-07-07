using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPt : MonoBehaviour
{
    public ParticleSystem expo;
    private bool spawn = false;
    private void Start()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() && spawn == false)
        {
            collision.GetComponent<PlayerController>().SpawnPT();
            expo.Play();
            spawn = true;
            GameManager.instance.HighScore();
        }
    }
}
