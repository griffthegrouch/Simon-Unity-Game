using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonScript : MonoBehaviour
{

    //bool tracks if game is currently active
    private bool gameStarted = false;

    //each button's sprite renderers
    private SpriteRenderer r;
    private SpriteRenderer y;
    private SpriteRenderer b;
    private SpriteRenderer g;

    //audio source attached to main object
    private AudioSource audioPlayer;

    //score arr keeps track of the sequence of buttons 
    private int[] scoreArr;
    private int clickedBtn;
    
    //array of all 4 button objects (doesnt include start button)
    private GameObject[] buttonsArr;


    //manually set the start button var
    public GameObject beeper; //remove
    public GameObject startBtn;

    //manually set each audio clip var
    public AudioClip rSound;
    public AudioClip gSound;
    public AudioClip ySound;
    public AudioClip bSound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;


    // Start is called before the first frame update -
    void Start()
    {
        // automatically gathering sprite and audio renderers
        r = transform.Find("red").GetComponent<SpriteRenderer>();
        y = transform.Find("yellow").GetComponent<SpriteRenderer>();
        b = transform.Find("blue").GetComponent<SpriteRenderer>();
        g = transform.Find("green").GetComponent<SpriteRenderer>();

        audioPlayer = GetComponent<AudioSource>();

        
    }

    // Update is called once per frame
    void Update()
    {
        //if start button is clicked
            if (Input.GetMouseButtonDown(0))
        {
            if (!gameStarted)
            {
                StartGame();
            }
            else
            {

            }
        }
    }
    private void StartGame()
    {
        gameStarted = true;
        startBtn.SetActive(false);

        intArr = new int[0];
        AddToSequenceAndStart();
    }
    private void EndGame()
    {
        StopAllCoroutines();
        gameStarted = false;
        startBtn.SetActive(true);

        intArr = new int[0];
        AddToSequenceAndStart();
    }
    private void AddToSequenceAndStart()
    {
        StopCoroutine(PlaySequence());
        int[] tempArr = new int[intArr.Length + 1];
        for (int i = 0; i < intArr.Length; i++)
        {
            tempArr[i] = intArr[i];
        }
        tempArr[intArr.Length ] = Random.Range(1, 5);
        intArr = tempArr;
        StartCoroutine(PlaySequence());
    }
    IEnumerator PlaySequence()
    {
        for (int i = 0; i < intArr.Length; i++)
        {
            switch (intArr[i])
            {
                case 1:
                    beeper.transform.position = r.transform.position;
                    audioPlayer.clip = rSound;
                    audioPlayer.time = 2f;
                    audioPlayer.Play();
                    break;
                case 2:
                    beeper.transform.position = g.transform.position;
                    audioPlayer.clip = gSound;
                    audioPlayer.time = 2f;
                    audioPlayer.Play();
                    break;
                case 3:
                    beeper.transform.position = y.transform.position;
                    audioPlayer.clip = ySound;
                    audioPlayer.time = 2f;
                    audioPlayer.Play();
                    break;
                case 4:
                    beeper.transform.position = b.transform.position;
                    audioPlayer.clip = bSound;
                    audioPlayer.time = 2f;
                    audioPlayer.Play();
                    break;
            }
            beeper.SetActive(true);
            yield return new WaitForSeconds(1);
            beeper.SetActive(false);
        }
        StartCoroutine(PlayerTest());
        yield break;
    }
    IEnumerator PlayerTest()
    {
        int count = 0;
        while (count < intArr.Length)

        {
            Debug.Log("count " + count);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            SpriteRenderer btn = GetClosestObject("btn", new Vector2(Input.mousePosition.x / 100 - 5, Input.mousePosition.y / 100 - 5)).GetComponent<SpriteRenderer>();
            if (btn == r)
            {
                clickedBtn = 1;
                beeper.transform.position = r.transform.position;
                audioPlayer.clip = rSound;
                audioPlayer.time = 2f;
            }
            else  if (btn == g)
            {
                clickedBtn = 2;
                beeper.transform.position = g.transform.position;
                audioPlayer.clip = gSound;
                audioPlayer.time = 2f;
            }
            else if (btn == y)
            {
                clickedBtn = 3;
                beeper.transform.position = y.transform.position;
                audioPlayer.clip = ySound;
                audioPlayer.time = 2f;
            }
            else  if (btn == b)
            {
                clickedBtn = 4;
                beeper.transform.position = b.transform.position;
                audioPlayer.clip = bSound;
                audioPlayer.time = 2f;
            }

            if (clickedBtn == intArr[count])//correct guess
            {
                if(count+1 == intArr.Length)//win round
                {
                    audioPlayer.Play();
                    beeper.SetActive(true);
                    yield return new WaitForSeconds(1);
                    beeper.SetActive(false);
                    audioPlayer.clip = correctSound;
                    audioPlayer.time = 0f;
                    audioPlayer.Play();
                    yield return new WaitForSeconds(0.5f);
                    AddToSequenceAndStart();
                    yield break;
                }
                else//correct guess but not round
                {
                    audioPlayer.Play();
                    beeper.SetActive(true);
                    yield return new WaitForSeconds(1);
                    beeper.SetActive(false);
                    count++;
                }
            }
            else//fail game
            {
                audioPlayer.clip = incorrectSound;
                audioPlayer.time = 0f;
                audioPlayer.Play();
                yield return new WaitForSeconds(0.5f);
                EndGame();
                yield break;
            }
        }
       
    }     
    private GameObject GetClosestObject(string tag, Vector2 pos)
    {//no calls but is a useful function
     //method that returns the closest gameobject to a position

        //gathers all 
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestObject = objectsWithTag[0];
        for (int i = 0; i < objectsWithTag.Length; i++)
        {
            //compares distances and switches for the shortest distance
            if (Vector2.Distance(pos, objectsWithTag[i].transform.position) <= Vector2.Distance(pos, closestObject.transform.position))
            {
                closestObject = objectsWithTag[i];
            }
        }
        return closestObject;
    }

}
