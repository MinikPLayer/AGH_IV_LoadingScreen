using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ImageScript : MonoBehaviour
{
    public Texture2D[] backgrounds;
    public Texture2D[] foregrounds;

    List<Texture2D> bgToUse = new List<Texture2D>();
    List<Texture2D> fgToUse = new List<Texture2D>();

    public SpriteRenderer currBg;
    public SpriteRenderer currFg;

    public SpriteRenderer fadeSprite;

    public MusicScript musicScript;

    public float changeTime = 6.58f;
    public float initialDelay = 1;

    public int fadeTime = 10;

    public float maxAnimMove = 0.35f;
    public float minAnimMove = 0.1f;

    float animMoveX = -1;
    float animMoveY = -1;

    float startBgScaleY = 0;
    // Start is called before the first frame update
    void Start()
    {
        //lastChange = lastChange.AddSeconds(initialDelay);
        Reset(true);

        lastAspect = Camera.main.aspect;
    }

    public void Reset(bool skipFadeIn = false)
    {
        lastChange = initialDelay + musicScript.start;
        animMoveX = UnityEngine.Random.Range(-1000, 1000) / 2137.0f;
        animMoveY = UnityEngine.Random.Range(-1000, 1000) / 2137.0f;
        //ChangeImage(skipFadeIn);
        StartCoroutine(ChangeImage(skipFadeIn, true));
    }

    float MoveTransform(float move)
    {
        move = move * -1337;
        move -= (int)move; // range (0, 1)
        move *= (maxAnimMove - minAnimMove); // range (-maxAnimMove, maxAnimMove)
        move += Math.Sign(move) * minAnimMove;


        return move;
    }

    void RandomAnim()
    {
        //animMoveX = UnityEngine.Random.Range(-maxAnimMove, maxAnimMove);
        //animMoveZ = UnityEngine.Random.Range(-maxAnimMove, maxAnimMove);
        animMoveX = MoveTransform(animMoveX);
        animMoveY = MoveTransform(animMoveY);

        Vector3 p1 = currBg.transform.position;
        p1.x = -animMoveX / changeTime;
        Vector3 p2 = currFg.transform.position;
        p2.x = -animMoveX / changeTime;

        currBg.transform.position = p1;
        currFg.transform.position = p2;
    }

    void ScaleToScreen(SpriteRenderer sr, Vector2 additionalScale, float maxYSize, int animMult = 1)
    {
        sr.transform.localScale = new Vector3(1, 1, 1);

        var multX = (Math.Abs(animMoveX) * changeTime) * 0.5f; // 50 because 0.5 * 100

        var width = sr.sprite.bounds.size.x - multX;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight * Screen.width / (double)Screen.height;

        float multX2 = 0;
        float multY = 0;

        if(animMoveY * animMult < 0)
        {
            multX2 = Math.Abs(animMoveY) * changeTime * 0.2f;
            multY = Math.Abs(animMoveY) * changeTime * 0.2f;
        }

        // Preserve aspect ratio
        float x = (1 + multX2) * (float)(worldScreenWidth / width) * additionalScale.x;
        float y = (1 + multY) * (float)(worldScreenHeight / height) * additionalScale.y;

        if (x > y)
            y = x;
        else
            x = y;
        

        sr.transform.localScale = new Vector3(x, y, 0.1f);

        var scale = sr.transform.localScale;
        var bottom = -worldScreenHeight / 2;
        var hght = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;
        //var posY = bottom + hght / 2;
        //var posY = bottom + hght * scale.y / 2;
        var posY = bottom;
        var p = sr.transform.position;
        p.y = (float)posY;
        sr.transform.position = p;
    }

    void ReScaleImages()
    {
        ScaleToScreen(currBg, new Vector2(1, 1), 1000);
        ScaleToScreen(currFg, new Vector2(0.4f, 0.4f), 2, -1);

        fgScale = currFg.transform.localScale;
        bgScale = currBg.transform.localScale;
    }

    int lastBGIndex = -1;
    int lastFGIndex = -1;
    IEnumerator ChangeImage(bool skipFadeIn = false, bool ignoreLastChange = false)
    {
        const int steps = 100;

        float step = 1.0f / steps;

        if (!skipFadeIn)
        {
            for (int i = 0; i < steps; i++)
            {
                var clr = fadeSprite.color;
                clr.a = step * i;
                fadeSprite.color = clr;
                //await Task.Delay(fadeTime);
                yield return new WaitForSeconds(fadeTime / 1000f);
            }
        }

        if(!ignoreLastChange)
            lastChange += changeTime;

        RandomAnim();
        
        // FG
        if (fgToUse.Count == 0)
            fgToUse.AddRange(foregrounds);

        var random = new System.Random().Next(0, fgToUse.Count);

        lastFGIndex = random;
        var tex = fgToUse[random];
        fgToUse.RemoveAt(random);
        currFg.sprite = Sprite.Create(tex, new Rect(0,0, tex.width, tex.height), new Vector2(0.5f, 0f));

        // BG
        if (bgToUse.Count == 0)
            bgToUse.AddRange(backgrounds);

        random = new System.Random().Next(0, bgToUse.Count); //UnityEngine.Random.Range(0, bgToUse.Count);

        lastBGIndex = random;
        tex = bgToUse[random];
        bgToUse.RemoveAt(random);
        currBg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f));

        ReScaleImages();

        startBgScaleY = currBg.transform.localScale.y;

        for (int i = 0; i < steps; i++)
        {
            var clr = fadeSprite.color;
            clr.a = 1 - step * i;
            fadeSprite.color = clr;
            //await Task.Delay(fadeTime);
            yield return new WaitForSeconds(fadeTime / 1000f);
        }

        changing = false;
    }

    Vector3 bgScale;
    Vector3 fgScale;

    float lastAspect;
    float lastChange = 0;

    bool changing = false;
    // Update is called once per frame
    void Update()
    {
        var now = musicScript.music.time;
        if (now < lastChange + initialDelay)
            Reset();

        Debug.Log(lastChange);
        var diff = (now - lastChange);
        if(diff > changeTime && !changing)//&& !inProgress)
        {
            //ChangeImage();
            changing = true;
            StartCoroutine(ChangeImage());
            
        }



        float aspect = Camera.main.aspect;
        if (lastAspect != aspect)
        {
            ReScaleImages();
            Debug.Log("Rescaling");
            lastAspect = aspect;
        }

        Vector3 p = currBg.transform.position;
        p.x += animMoveX * Time.deltaTime * 0.5f;
        currBg.transform.position = p;

        p = currFg.transform.position;
        p.x += animMoveX * Time.deltaTime;
        currFg.transform.position = p;

        Vector3 s = currBg.transform.localScale;
        s.y = s.x = bgScale.x + animMoveY * startBgScaleY * 0.1f * diff;
        currBg.transform.localScale = s;

        s = currFg.transform.localScale;
        s.y = s.x = fgScale.x - animMoveY * 0.1f * diff;
        currFg.transform.localScale = s;
    }
}
