using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Scripting;

public class UIController : MonoBehaviour {

    public Slider betSlider;
    public Text betText;
    private static int betValue;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI jackpotValue;
    public TextMeshProUGUI creditValue;
    public Button spinButton;
    ReelsSpinning spin;

	void Awake ()
    {
        spin = GameObject.Find("Reels").GetComponent<ReelsSpinning>();
        betText.text = "100";
        betValue = 100;

    }
	
	void Update ()
    {
        JackpotCounter();
        CheckMessage();
    }

    public void BetCalculatorSlider()
    {

        betText.text = betSlider.value.ToString();
        betValue = (int)betSlider.value;


    }

    public void JackpotCounter()
    {
        int val;
        if (int.TryParse(jackpotValue.text,out val))
        {
            val++;
            jackpotValue.text = val.ToString();
        }
    }
    
    public void SpinButtonSound()
    {
        spinButton.GetComponent<AudioSource>().Play();
    }

    public void MessageText(string text)
    {
        messageText.text = text;
        
    }

    public void UpdateCredit()
    {
       
        int val;
        if (int.TryParse(creditValue.text, out val))
        {
            val = val - betValue;
            if (val <= 0)
            { 
                spinButton.interactable = false;
                spin.DisableSpinning();

            }
            else
            {
                creditValue.text = val.ToString();
            }
        }

    }

    void CheckMessage()
    {

        if (spin.enabled)
        {
            MessageText("GOOD LUCK");
        }
        else
            MessageText("WELCOME");

    }

   
}
