using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    [HideInInspector]
    public AudioSource music;
    public float start = 0;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
        music.time = start;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
