using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ReelsSpinning : MonoBehaviour
{

    Button spinButton;
    AudioSource reelSound;
    SymbolsData symbolsData;    
    AudioSource buttonSound;
    Animation animate;
    GameObject coins;

    /*json loader for stop pictures*/
    DataLoader loader;
    DataLoader.StopData[] stopData;

    #region Parameters

    public float spinTime;                          //time to reach the target position
    float currentTime;                              //accumulate time
    float  oneSpin;                                 //how much units for one full spin        
    public int numberOfSpins;                       //how much spins we want
    float[] distance = { 0, 0, 0, 0, 0 };           //distance for every reel
    int counter;                                    //counter for tracking spins

    public TextMeshProUGUI creditValue;
    #endregion

    void Awake()
    {
        spinButton = GameObject.FindGameObjectWithTag("spin").GetComponent<Button>();
        reelSound = GetComponent<AudioSource>();
             
    }

    void Start()
    {
        symbolsData = SymbolsData.instance;         //getting data about each symbol
        oneSpin = 6 * symbolsData.distance;         //calculating one full spin
        counter = 0;                                //setting counter to zero
        loader = new DataLoader();                  //loading data from json->DataStop in Resources
        stopData=loader.LoadDataStop("DataStop");
    
    }

    void Update()
    {
        /*UpdateTime*/
        currentTime += Time.deltaTime;

        /* beggin of spinning*/
        if (currentTime == Time.deltaTime)
        {

            for (int i = 1; i <= 5; i++)
            {
                UpdatePositions(i);
                FindSymbolDistance(i, stopData[counter].names[i-1]);
            }
           
        }
      
        /* end of spinning */
        if (currentTime > spinTime )
        {
            for (int i = 1; i <= 5; i++)
            {
                FixOverstep(i);
                StartCoroutine (StoppingReelsAnimation(i));
                
            }
            
            DisableSpinning();
        }

        /* spinning*/    
        else 
        {
            for (int i = 1; i <= 5; i++)
            {
                SpinReel(i);
            }

        }

    }

    void OnEnable()
    {
        /* disabling button*/
        spinButton.interactable = false;
        /*reel sound */
        reelSound.enabled = true;
        /* reseting timer */
        currentTime = 0f;
        counter++;


    }

    /*called from spin button*/
    public void EnableSpinning()
    {
        this.enabled = true;
    }

    public void DisableSpinning()
    {
        reelSound.enabled = false;
        this.enabled = false;
    }

    void SpinReel(int reelNumber)
    {
        float speed = SpinSpeed(reelNumber);    //updating speed
        int i = reelNumber - 1;
        RectTransform position;
        float bottom = symbolsData.startPosition - 6 * symbolsData.distance;
        float epsilon = 0f;
        for (int j = 0; j < 6; j++)
        {
            /* get position of picture */
            position=symbolsData.symbols[i, j].SymbolObject.GetComponent<RectTransform>();

            /* move picture with lerp */
            position.anchoredPosition = new Vector2(position.anchoredPosition.x, position.anchoredPosition.y - speed);

            /* check if it is bottom => move on top */
            if (position.anchoredPosition.y <= bottom)
            {
                epsilon = position.anchoredPosition.y + 240f;
                position.anchoredPosition = new Vector2(position.anchoredPosition.x, symbolsData.startPosition + epsilon);
            }

        }

    }

    void FixOverstep(int reelNumber)
    {
        float speed = CalculateOverstep(reelNumber);
        int i = reelNumber - 1;
        RectTransform position;
        for (int j = 0; j < 6; j++)
        {
            /* get position of picture */
            position = symbolsData.symbols[i, j].SymbolObject.GetComponent<RectTransform>();
            Vector2 target = new Vector2(position.anchoredPosition.x, position.anchoredPosition.y - speed);
            /* move picture */
            //position.anchoredPosition = Vector2.MoveTowards(position.anchoredPosition, target, Time.deltaTime);
            position.anchoredPosition = target;
        }

    }

    float CalculateOverstep(int reelNumber)
    {
        int position = 0;
        float overstep = 0;

        position = FindSymbolPosition(reelNumber, stopData[counter].names[reelNumber-1]);
        SymbolsData.Symbol symbol = FindOnPosition(reelNumber, position);
        overstep = symbol.SymbolObject.GetComponent<RectTransform>().anchoredPosition.y-20f; //20 is our target position
            
        return overstep;
    }

    float SpinSpeed(int reelNumber)
    {
        float speed = distance[reelNumber - 1] * Time.deltaTime/spinTime;
        return speed;
    }

    /* distance from target position*/
    void FindSymbolDistance(int reelNumber, string symbolName)
    {
        int position = -1;
        position = FindSymbolPosition(reelNumber, symbolName);
        float dist = oneSpin*numberOfSpins + (2 - position) * 65f;  //2 is target position
        distance[reelNumber-1] = dist;

    }

    /* position of target symbol->symbolName*/
    int FindSymbolPosition(int reelNumber, string symbolName)
    {
        int i = reelNumber - 1;
        int position = -1;

        for (int j = 0; j < 6; j++)
        {
            if (symbolsData.symbols[i, j].SymbolName.Equals(symbolName))
            {
                position = symbolsData.symbols[i, j].Position;
                break;
            }
            
        }
        return position;

    }
    
    /*find symbol data on position*/
    SymbolsData.Symbol FindOnPosition(int reelNumber,int position)
    {
        int i = reelNumber - 1;
        SymbolsData.Symbol symbol=new SymbolsData.Symbol();
        for (int j = 0; j < 6; j++)
        {
            if (symbolsData.symbols[i, j].Position == position)
                symbol = symbolsData.symbols[i, j];

        }
        return symbol;

    }

    /* update positions after spinning */
    void UpdatePositions(int reelNumber)
    {
        int moved = (int)(distance[reelNumber - 1] / symbolsData.distance) % 6;
        for (int i = 0; i < 6; i++)
        {
            symbolsData.symbols[reelNumber - 1, i].Position = (symbolsData.symbols[reelNumber - 1, i].Position + moved) % 6;
        }
    }
 
    /* animate stoping*/
    IEnumerator StoppingReelsAnimation(int reelNumber)
    {
        /*finding symbols to animate*/
        int position = FindSymbolPosition(reelNumber, stopData[counter].names[reelNumber-1]);
        SymbolsData.Symbol symbol = FindOnPosition(reelNumber, position);
        /*starting animation*/
        Animate(symbol, reelNumber);
        /*playing animations for 4 seconds*/
        yield return new WaitForSeconds(4f);
        /* stop after 4 seconds*/
        StopAnimating();
        /* enabling button */
        spinButton.interactable = true;
    }

    void Animate(SymbolsData.Symbol symbol, int reelNumber)
    {
        int won = GetWonValue();

        if (won >= 1000)
        {
            AnimatePulsing(symbol);
            if (reelNumber == 1)
                AnimateCoins();
        }
        else if (won > 0)
        {
            int val;
            if (int.TryParse(creditValue.text, out val))
            {
                val = val + won ;
                creditValue.text = val.ToString();
            }

            AnimatePulsing(symbol);
            if (reelNumber == 1)
                PlaySound();

          
        }
        else
        {   //MESSAGEES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AnimatePulsing(symbol);
        }
    }

    void AnimatePulsing(SymbolsData.Symbol symbol)
    {
        animate = symbol.SymbolObject.GetComponent<Animation>();
        animate.Play("pop");
    }

    void AnimateCoins()
    {

      coins = (GameObject)Instantiate(Resources.Load("coins"));

    }

    void PlaySound()
    {
        AudioSource sound = GameObject.FindGameObjectWithTag("sound").GetComponent<AudioSource>();
        AudioClip clip = sound.clip;
        sound.PlayOneShot(clip);

    }

    void StopAnimating()
    {
        Destroy(coins);
    }

    /* called from UIController */
    public int GetWonValue()
    {
        return stopData[counter].won;
    }

}
