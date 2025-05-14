using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Game Buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;

    private int standClicks = 0;

    // Access the player and dealer's script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    // public Text to access and update - hud
    public TMP_Text scoreText;
    public TMP_Text dealerScoreText;
    public TMP_Text betsText;
    public TMP_Text cashText;
    public TMP_Text mainText;
    public TMP_Text standBtnText;

    // Card hiding dealer's 2nd card
    public GameObject hideCard;
    // How much is bet
    int pot = 0;

    void Start()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        betBtn.onClick.AddListener(() => BetClicked());
    }

    private void DealClicked()
    {
        // Reset round, hide text, prep for new hand
        playerScript.ResetHand();
        dealerScript.ResetHand();
        // Hide deal hand score at start of deal
        dealerScoreText.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();
        // Update the scores displayed
        scoreText.text = "Hand: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
        // Place card back on dealer card, hide card
        hideCard.GetComponent<Renderer>().enabled = true;
        // Adjust buttons visibility
        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "-";
        // Set standard pot size
        pot = 0;
        betsText.text = "Bets: $" + pot.ToString();
        //playerScript.AdjustMoney(-20);
        cashText.text = "$" + playerScript.GetMoney().ToString();

    }

    private void HitClicked()
    {
        // Check that there is still room on the table
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) RoundOver();
        }
    }

    private void StandClicked()
    {
        standClicks++;

        // Oyuncu "stand" dediðinde artýk kart çekemez
        hitBtn.gameObject.SetActive(false);

        if (standClicks == 1)
        {
            hideCard.GetComponent<Renderer>().enabled = false;
            HitDealer(); // Dealer oynamaya baþlar
        }
    }



    private void HitDealer()
    {
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
        }
        RoundOver();
    }

    // Check for winnner and loser, hand is over
    void RoundOver()
    {
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;



        bool roundOver = true;




        if (playerBust)
        {
            mainText.text = "Dealer wins!";
        }

        else if (!playerBust && !dealerBust)
        {
            if (dealerScript.handValue >= 17 && dealerScript.handValue <= 21 &&
                dealerScript.handValue > playerScript.handValue)
            {
                mainText.text = "Dealer wins!";
            }
            else if (dealerScript.handValue >= 17 && dealerScript.handValue <= 21 &&
                dealerScript.handValue < playerScript.handValue)
            {
                mainText.text = "You win!";
                playerScript.AdjustMoney(pot * 2);
            }
            else if (dealerScript.handValue >= 17 && dealerScript.handValue <= 21 && dealerScript.handValue == playerScript.handValue)
            {
                mainText.text = "Push: Bets returned";
                playerScript.AdjustMoney(pot);
            }
        }


        else if (dealerBust)
        {
            mainText.text = "You win!";
            playerScript.AdjustMoney(pot * 2);
        }

        else
        {
            roundOver = false;
        }

        // Round bittiðinde UI güncelleniyor
        if (roundOver)
        {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(true);

            // Burada görünürlüðü garanti ediyoruz:
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "$" + playerScript.GetMoney().ToString();
            standClicks = 0;
        }
    }


    // Add money to pot if bet clicked
    void BetClicked()
    {
        TMP_Text newBet = betBtn.GetComponentInChildren<TMP_Text>();

        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-intBet);
        cashText.text = "$" + playerScript.GetMoney().ToString();
        pot += intBet;
        betsText.text = "Bets: $" + pot.ToString();
    }


}
