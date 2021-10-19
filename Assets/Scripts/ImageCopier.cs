using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageCopier : MonoBehaviour
{
    public SpriteRenderer target;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sr.sprite = target.sprite;
    }
}
