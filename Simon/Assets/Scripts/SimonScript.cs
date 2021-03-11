using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonScript : MonoBehaviour
{
    /*
    Simon Game - Griffin Atkinson - 11/03/2021

    Simon game summary
    game consists of the game displaying an increasingly long sequence of buttons(4 possibilities) that the user then attmpts to recreate during their turn
    if the user is successful, the button sequence randomly adds another button to the end of the sequence
    game continues until the user fails to recreate the sequence.


    basic flow -
    1 user clicks start    (game randomly selects 1 button to begin sequence)

    2 game starts playing button-sequence + adds a random button to the end of the sequence 
    2.5 button sequence ends

    3 game starts user turn (user tries to click buttons in the order of the button-sequence)
    4 user clicks button
        if button click is correct
            - if click is the last in sequence -> go to 2
            - if click is not the last in sequence -> go back to 4
        if button click is incorrect
            - game ends -> score is recorded
    */



    //bool tracks if game is currently active
    private bool gameStarted = false;

    //bool tracks highest score achieved
    private int highscore = 0;

    //textmeshes for displaying score on GUI
    public Transform highscoreDisplayText;
    public Transform scoreDisplayText;

    //each button's sprite renderers
    private SpriteRenderer r;
    private SpriteRenderer y;
    private SpriteRenderer b;
    private SpriteRenderer g;

    //manually set each button's default and highlighted colours
    public Color defaultRed; public Color highlightedRed;
    public Color defaultGreen; public Color highlightedGreen;
    public Color defaultYellow; public Color highlightedYellow;
    public Color defaultBlue; public Color highlightedBlue;

    //each button's default and highlighted colours in usable arrays
    private Color[] defaultColoursArr;
    private Color[] highlightedColoursArr;


    //audio source attached to main object
    private AudioSource audioPlayer;

    //manually set each audio clip var
    public AudioClip rSound;
    public AudioClip gSound;
    public AudioClip ySound;
    public AudioClip bSound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private AudioClip[] buttonSoundsArr;

    //score arr keeps track of the sequence of buttons 
    private List<int> buttonSequence = new List<int> (0);
    private int clickedBtn;


    //array of all 4 button objects (doesnt include start button)
    private GameObject[] buttonsArr;

    //manually set the start button var
    public GameObject startBtn;



    // Start is called before the first frame update -
    void Start()
    {
        //setting arrays for accessing each button's colours
        defaultColoursArr = new Color[] { defaultRed, defaultGreen, defaultYellow, defaultBlue };
        highlightedColoursArr = new Color[] { highlightedGreen, highlightedGreen, highlightedGreen, highlightedGreen };

        //setting array for accessing each button's sound clips
        buttonSoundsArr = new AudioClip[] { rSound, gSound, ySound, bSound };

        //gathering sound player and setting it up
        audioPlayer = GetComponent<AudioSource>();
        audioPlayer.time = .2f;

        //gathering all the coloured buttons
        buttonsArr = new GameObject[] {transform.Find("redBtn").gameObject, transform.Find("greenBtn").gameObject, transform.Find("yellowBtn").gameObject, transform.Find("blueBtn").gameObject};

        //setting each button to it's default colour
        for (int i = 0; i < buttonsArr.Length; i++)
        {
            buttonsArr[i].GetComponent<SpriteRenderer>().color = defaultColoursArr[i];
        } 
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

    void StartGame() //called when start button is clicked
    {
        //setting game started var and disabling start button
        gameStarted = true;
        //startBtn.SetActive(false);

        //clearing old sequence
        buttonSequence.Clear();

        //creating new sequence
        AddToSequence();

        //playing the button sequence
        StartCoroutine(PlaySequence());


    }

    void EndGame() //called when user fails to repeat sequence
    {
        StopAllCoroutines();

        //setting game started var and enabling start button
        gameStarted = false;
        startBtn.SetActive(true);

        //clearing old sequence
        buttonSequence.Clear();
    }

    void AddToSequence()
    {
        //adding a random button to the end of the sequence
        
        buttonSequence.Add(Random.Range(1, 5));
        Debug.Log("new sequence button: " + buttonSequence[buttonSequence.Count-1]);

    }

    void WinRound()
    {
        //called when all buttons in sequence were correctly guessed by player

        //update score display
        UpdateScore();

        //add one more to the sequence and start 
        AddToSequence();
        StartCoroutine(PlaySequence());
    }

    void UpdateScore() {
        //displaying current score
        scoreDisplayText.GetComponent<TextMesh>().text = buttonSequence.Count.ToString();

        //checking if current score is higher than highscore
        if (highscore < buttonSequence.Count){
            highscore = buttonSequence.Count;
            highscoreDisplayText.GetComponent<TextMesh>().text = highscore.ToString();
        }
    }

    IEnumerator PlaySequence()
    {
        //enumerator runs through each value in the button sequence and displays it to the player -> lights it up and plays a sound accordingly
        for (int i = 0; i < buttonSequence.Count; i++)
        {//loops through sequence and selects individual button in each iteration "i"

            //setting audio player clip to coresponding button's sound clip
            audioPlayer.clip = buttonSoundsArr[buttonSequence[i]];
            audioPlayer.Play();

            //setting buttons colour to highlighted, waiting a moment, then changing its colour back to default
            buttonsArr[i].GetComponent<SpriteRenderer>().color = highlightedColoursArr[buttonSequence[i]];
            yield return new WaitForSeconds(.5f);
            buttonsArr[i].GetComponent<SpriteRenderer>().color = defaultColoursArr[buttonSequence[i]];
        }

        //when done playing sequence, start player's turn
        StartCoroutine(PlayerTurn());
    }
    IEnumerator PlayerTurn()
    {
        int count = 0; // counts how many successful clicks user has gotten so far this turn
        while (count < buttonSequence.Count) //while there is still more buttons in the sequence
        {
            Debug.Log("count " + count);
            //wait for the user to click
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            //get the button thats closest to mouse click
            int clickedBtn = GetClosestButtonIndex(new Vector2((Input.mousePosition.x -512)/ 1024, (Input.mousePosition.y - 384)/ 768));

            //setting audio player clip to selected button's sound clip
            audioPlayer.clip = buttonSoundsArr[clickedBtn];
            audioPlayer.Play();

            if (clickedBtn == buttonSequence[count])// clicked button = current button in sequence -> correct guess
            {
                //setting selected button's colour to highlighted, waiting a moment, then changing its colour back to default
                buttonsArr[clickedBtn].GetComponent<SpriteRenderer>().color = highlightedColoursArr[clickedBtn];
                yield return new WaitForSeconds(.5f);
                buttonsArr[clickedBtn].GetComponent<SpriteRenderer>().color = defaultColoursArr[clickedBtn];

                if (count + 1 == buttonSequence.Count)// correct guess + no more in sequence -> win round
                {
                    //setting audio player clip to correct guess/win round sound
                    audioPlayer.clip = correctSound;
                    audioPlayer.Play();

                    //in future make center button light up
                    yield return new WaitForSeconds(1);

                    //call win round function
                    WinRound();
                    yield break;
                }
                else// correct guess + more in sequence -> continue round
                {
                    //add one to the successful clicks count
                    count++;
                }
            }
            else// clicked button = NOT current button in sequence -> game failed!
            {
                audioPlayer.clip = incorrectSound;
                audioPlayer.Play();
                yield return new WaitForSeconds(0.5f);
                EndGame();
                yield break;
            }
        }

    }
    private int GetClosestButtonIndex(Vector2 pos)
    {//method that returns the closest button to a position

        //gathers all 
        int closestButtonIndex = 0;
        for (int i = 0; i < buttonsArr.Length; i++)
        {
            //compares distances and switches for the shortest distance
            if (Vector2.Distance(pos, buttonsArr[i].transform.position) <= Vector2.Distance(pos, buttonsArr[closestButtonIndex].transform.position))
            {
                closestButtonIndex = i;
            }

        }
        //return the stored button index
        Debug.Log("user clicked");
        Debug.Log(pos);
        Debug.Log(closestButtonIndex);
        Debug.Log(buttonsArr[closestButtonIndex].transform.position);
        startBtn.transform.position = new Vector3(pos.x,pos.y,10);
        return closestButtonIndex;

    }

}
