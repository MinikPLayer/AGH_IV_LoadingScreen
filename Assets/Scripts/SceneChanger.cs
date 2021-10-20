using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SceneChanger : MonoBehaviour
{
    public string sceneName = "";
    public Image fadeOutImage;
    public int fadeDelay = 10;

    public AudioClip clickedSoundClip;

    AudioSource aSource;

    IEnumerator _ChangeScene()
    {
        const int steps = 100;

        if (fadeOutImage != null)
        {
            float step = 1f / steps;
            for(int i = 0;i<steps;i++)
            {
                Color c = fadeOutImage.color;
                c.a = step * i;
                fadeOutImage.color = c;
                //await Task.Delay(fadeDelay);
                yield return new WaitForSeconds(fadeDelay / 1000f);
            }
        }

        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    static bool changing = false;
    public void ChangeScene()
    {
        if (changing)
            return;

        if(clickedSoundClip != null)
        {
            aSource.clip = clickedSoundClip;
            aSource.Play();
        }


        changing = true;
        StartCoroutine("_ChangeScene");
    }
}
