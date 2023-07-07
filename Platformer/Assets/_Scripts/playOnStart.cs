using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private AudioClip _clip;
    void Start()
    {
        SoundManager.instance.PlayMusic(_clip);

    }

}
