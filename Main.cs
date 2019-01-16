using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blackjack
{
    public partial class Main : Form
    {
        Random rand = new Random();

        int pickSuit = 0;
        int pickNumber = 0;
        int cardCounter = 1;
        int dealerCardCounter = 1;
        int handValue = 0;
        int dealerHandValue = 0;
        int aceCount = 0;
        int dealerAceCount = 0;
        int currentBetIndex = 0;
        int currentBet = 5;
        int[] betArray = {5, 10, 25, 50, 100, 250, 500, 1000};
        int accountValue = 100;

        bool doubleDown = false;
        bool runOnce = true;
        bool dealerTurn = false;
        bool doHit = false;
        bool canHit = false;
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
            stayButton.Enabled = false; stayButton.BackColor = SystemColors.ControlDark;
            splitButton.Enabled = false; splitButton.BackColor = SystemColors.ControlDark;
            doubledownButton.Enabled = false; doubledownButton.BackColor = SystemColors.ControlDark;
            updateBet.Enabled = false; updateBet.BackColor = SystemColors.ControlDark;
            betDisplayBox.Text = currentBet.ToString();
            actualBetAmount.Text = "$" + currentBet.ToString();
            accountValueLabel.Text = "$" + accountValue.ToString();
        }

        private void runGame()
        {
            string deckString = System.IO.File.ReadAllText("Deck.txt");
            String[] suits = deckString.Split('/');            // Splits deck into 4 suites
            String[] refillSuites = suits;                     
            String[] diamonds = suits[0].Split('.');           // Diamond - 0
            String[] hearts = suits[1].Split('.');             // Heart   - 1
            String[] spades = suits[2].Split('.');             // Spade   - 2
            String[] clubs = suits[3].Split('.');              // Club    - 3
            
            if (runOnce)
            {
                runOnce = false;
                
                // - - - - - - - -
                // GAME LOOP
                // - - - - - - - -

                while (cardCounter < 3)
                {

                    pickNumber = rand.Next(0, 13);
                    pickSuit = rand.Next(0, 4);

                    if (pickNumber == 12)
                    {
                        aceCount++;
                    }
                    drawCard(diamonds, hearts, spades, clubs, pickSuit, pickNumber, cardCounter, true);
                    if (pickNumber == 0)
                    {
                        handValue += 2;
                    }
                    else if (pickNumber > 8 && pickNumber < 12)
                    {
                        handValue += 10;
                    }
                    else if (pickNumber == 12)
                    {
                        handValue += 11;
                    }
                    else
                    {
                        handValue += pickNumber + 2;
                    }
                    cardCounter++;
                }
                if(handValue == 22)
                {
                    handValue -= 10;
                    aceCount--;
                }

                while (dealerCardCounter < 2)
                {
                    pickSuit = rand.Next(0, 4);
                    pickNumber = rand.Next(0, 13);
                    if (pickNumber == 12)
                    {
                        dealerAceCount++;
                    }

                    drawCard(diamonds, hearts, spades, clubs, pickSuit, pickNumber, cardCounter, false);
                    if (pickNumber == 0)
                    {
                        dealerHandValue += 2;
                    }
                    else if (pickNumber > 8 && pickNumber < 12)
                    {
                        dealerHandValue += 10;
                    }
                    else if (pickNumber == 12)
                    {
                        dealerHandValue += 11;
                    }
                    else
                    {
                        dealerHandValue += pickNumber + 2;
                    }
                    dealerCardCounter++;
                }

                handValueLabel.Text = handValue.ToString();

                if (handValue == 21)
                {
                    handValueLabel.Text = handValue.ToString();
                    canHit = false;
                    hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                    dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                    betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                    betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                    // Blackjack (Win)
                    transaction(true);
                    resultsLabel.Text = "Player Win!";
                    resultsLabel.BackColor = Color.White;
                }
                else {
                    canHit = true;
                    hitButton.Enabled = true; hitButton.BackColor = SystemColors.ButtonFace;
                    stayButton.Enabled = true; stayButton.BackColor = SystemColors.ButtonFace;
                    doubledownButton.Enabled = true; doubledownButton.BackColor = SystemColors.ButtonFace;
                }
                
            }

            if (doHit || doubleDown)
            {
                doHit = false;
                doubleDown = false;

                revealCardBase(cardCounter);
                pickSuit = rand.Next(0, 4);
                pickNumber = rand.Next(0, 13);
                if (pickNumber == 12)
                {
                    aceCount++;
                }
                drawCard(diamonds, hearts, spades, clubs, pickSuit, pickNumber, cardCounter, true);
                if (pickNumber == 0)
                {
                    handValue += 2;
                }
                else if (pickNumber > 8 && pickNumber < 12)
                {
                    handValue += 10;
                }
                else if (pickNumber == 12)
                {
                    handValue += 11;
                }
                else
                {
                    handValue += pickNumber + 2;
                }
                cardCounter++;
                if (handValue > 21)
                {
                    if(aceCount > 0)
                    {
                        handValue -= 10;
                        aceCount--;
                        handValueLabel.Text = handValue.ToString();
                    } else
                    {
                        handValueLabel.Text = "BUST";
                        // Lose
                        resultsLabel.Text = "Dealer Win!";
                        resultsLabel.BackColor = Color.White;
                        transaction(false);
                        canHit = false;
                        hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        stayButton.Enabled = false; stayButton.BackColor = SystemColors.ControlDark;
                        betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                        betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                    }
                }
                else if (handValue == 21)
                {
                    handValueLabel.Text = handValue.ToString();
                    canHit = false;
                    hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                    betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                    betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                } else
                {
                    handValueLabel.Text = handValue.ToString();
                }
            }
            while (dealerTurn)
            {
                revealDealerCardBase(dealerCardCounter);
                pickSuit = rand.Next(0, 4);
                pickNumber = rand.Next(0, 13);
                if (pickNumber == 12)
                {
                    dealerAceCount++;
                }
                drawCard(diamonds, hearts, spades, clubs, pickSuit, pickNumber, cardCounter, false);
                if (pickNumber == 0)
                {
                    dealerHandValue += 2;
                }
                else if (pickNumber > 8 && pickNumber < 12)
                {
                    dealerHandValue += 10;
                }
                else if (pickNumber == 12)
                {
                    dealerHandValue += 11;
                }
                else
                {
                    dealerHandValue += pickNumber + 2;
                }
                dealerCardCounter++;

                if (dealerHandValue > 21)
                {
                    if(dealerAceCount > 0)
                    {
                        dealerAceCount--;
                        dealerHandValue -= 10;
                    } else
                    {
                        // Player Win
                        resultsLabel.Text = "Player Win!";
                        resultsLabel.BackColor = Color.White;
                        hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                        betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                        dealerTurn = false;
                        dealerAmountLabel.Text = "BUST";
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        transaction(true);
                        break;
                    }
                }
                else if (dealerHandValue < 17)
                {
                    dealerAmountLabel.Text = dealerHandValue.ToString();
                }
                else if (dealerHandValue >= 17)
                {
                    if (dealerHandValue > handValue)
                    {
                        // Dealer Win
                        resultsLabel.Text = "Dealer Win!";
                        resultsLabel.BackColor = Color.White;
                        canHit = false;
                        hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                        betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                        dealerTurn = false;
                        dealerAmountLabel.Text = dealerHandValue.ToString();
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        transaction(false);
                        break;
                    } else if(dealerHandValue < handValue)
                    {
                        // Player Win
                        resultsLabel.Text = "Player Win!";
                        resultsLabel.BackColor = Color.White;
                        canHit = false;
                        hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                        betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                        dealerTurn = false;
                        dealerAmountLabel.Text = dealerHandValue.ToString();
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        transaction(true);
                        break;
                    } else
                    {
                        // Push
                        resultsLabel.Text = "Push!";
                        resultsLabel.BackColor = Color.White;
                        canHit = false;
                        hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
                        betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
                        dealerTurn = false;
                        dealerAmountLabel.Text = dealerHandValue.ToString();
                        dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                        break;
                    }
                }
            }

        }

        private void revealDealerCardBase(int dealerCards)
        {
            switch (dealerCards)
            {
                case 2:
                    dealerBlank2.Visible = true;
                    break;
                case 3:
                    dealerBlank3.Visible = true;
                    break;
                case 4:
                    dealerBlank4.Visible = true;
                    break;
                case 5:
                    dealerBlank5.Visible = true;
                    break;
            }
        }

        private void revealCardBase(int cardCounter)
        {
            switch(cardCounter)
            {
                case 3:
                    cardBlank3.Visible = true;
                    break;
                case 4:
                    cardBlank4.Visible = true;
                    break;
                case 5:
                    cardBlank5.Visible = true;
                    break;
                case 6:
                    cardBlank6.Visible = true;
                    break;
                case 7:
                    cardBlank7.Visible = true;
                    break;
                case 8:
                    cardBlank8.Visible = true;
                    break;
            }
        }

        private void drawCard(String[] diamonds, String[] hearts, String[] spades, String[] clubs, int pickSuit, int pickNumber, int cardCounter, bool player)
        {
            if(player)
            {
                switch (pickSuit)
                {
                    case 0:
                        string value1 = diamonds[pickNumber];
                        printSuit(cardCounter, 1, cardCounter, true);
                        printCard(cardCounter, dealerCardCounter, value1, true);
                        //diamonds[pickNumber] = "";
                        break;
                    case 1:
                        string value2 = hearts[pickNumber];
                        printSuit(cardCounter, 2, cardCounter, true);
                        printCard(cardCounter, dealerCardCounter, value2, true);
                        //hearts[pickNumber] = "";
                        break;
                    case 2:
                        string value3 = spades[pickNumber];
                        printSuit(cardCounter, 3, cardCounter, true);
                        printCard(cardCounter, dealerCardCounter, value3, true);
                        //spades[pickNumber] = "";
                        break;
                    case 3:
                        string value4 = clubs[pickNumber];
                        printSuit(cardCounter, 4, cardCounter, true);
                        printCard(cardCounter, dealerCardCounter, value4, true);
                        //clubs[pickNumber] = "";
                        break;

                }
            } else
            {
                switch (pickSuit)
                {
                    case 0:
                        string value1 = diamonds[pickNumber];
                        printSuit(cardCounter, 1, cardCounter, false);
                        printCard(cardCounter, dealerCardCounter, value1, false);
                        //diamonds[pickNumber] = "";
                        break;
                    case 1:
                        string value2 = hearts[pickNumber];
                        printSuit(cardCounter, 2, cardCounter, false);
                        printCard(cardCounter, dealerCardCounter, value2, false);
                        //hearts[pickNumber] = "";
                        break;
                    case 2:
                        string value3 = spades[pickNumber];
                        printSuit(cardCounter, 3, cardCounter, false);
                        printCard(cardCounter, dealerCardCounter, value3, false);
                        //spades[pickNumber] = "";
                        break;
                    case 3:
                        string value4 = clubs[pickNumber];
                        printSuit(cardCounter, 4, cardCounter, false);
                        printCard(cardCounter, dealerCardCounter, value4, false);
                        //clubs[pickNumber] = "";
                        break;

                }
            }
        }

        private void printSuit(int card, int suit, int cardCounter, bool player) {
            if(player)
            {
                switch (suit)
                {
                    case 1:
                        // change picture to diamonds
                        switch (cardCounter)
                        {
                            case 1:
                                suitDiamonds1.Visible = true;
                                break;
                            case 2:
                                suitDiamonds2.Visible = true;
                                break;
                            case 3:
                                suitDiamonds3.Visible = true;
                                break;
                            case 4:
                                suitDiamonds4.Visible = true;
                                break;
                            case 5:
                                suitDiamonds5.Visible = true;
                                break;
                            case 6:
                                suitDiamonds6.Visible = true;
                                break;
                            case 7:
                                suitDiamonds7.Visible = true;
                                break;
                            case 8:
                                suitDiamonds8.Visible = true;
                                break;
                        }
                        break;
                    case 2:
                        // change picture to hearts
                        switch (cardCounter)
                        {
                            case 1:
                                suitHearts1.Visible = true;
                                break;
                            case 2:
                                suitHearts2.Visible = true;
                                break;
                            case 3:
                                suitHearts3.Visible = true;
                                break;
                            case 4:
                                suitHearts4.Visible = true;
                                break;
                            case 5:
                                suitHearts5.Visible = true;
                                break;
                            case 6:
                                suitHearts6.Visible = true;
                                break;
                            case 7:
                                suitHearts7.Visible = true;
                                break;
                            case 8:
                                suitHearts8.Visible = true;
                                break;
                        }
                        break;
                    case 3:
                        // change picture to spades
                        switch (cardCounter)
                        {
                            case 1:
                                suitSpades1.Visible = true;
                                break;
                            case 2:
                                suitSpades2.Visible = true;
                                break;
                            case 3:
                                suitSpades3.Visible = true;
                                break;
                            case 4:
                                suitSpades4.Visible = true;
                                break;
                            case 5:
                                suitSpades5.Visible = true;
                                break;
                            case 6:
                                suitSpades6.Visible = true;
                                break;
                            case 7:
                                suitSpades7.Visible = true;
                                break;
                            case 8:
                                suitSpades8.Visible = true;
                                break;
                        }
                        break;
                    case 4:
                        // change picture to clubs
                        switch (cardCounter)
                        {
                            case 1:
                                suitClubs1.Visible = true;
                                break;
                            case 2:
                                suitClubs2.Visible = true;
                                break;
                            case 3:
                                suitClubs3.Visible = true;
                                break;
                            case 4:
                                suitClubs4.Visible = true;
                                break;
                            case 5:
                                suitClubs5.Visible = true;
                                break;
                            case 6:
                                suitClubs6.Visible = true;
                                break;
                            case 7:
                                suitClubs7.Visible = true;
                                break;
                            case 8:
                                suitClubs8.Visible = true;
                                break;
                        }
                        break;

                }
            } else
            {
                switch(suit)
                {
                    case 1:
                        switch (dealerCardCounter)
                        {
                            case 1:
                                dealerDiamonds1.Visible = true;
                                break;
                            case 2:
                                dealerDiamonds2.Visible = true;
                                break;
                            case 3:
                                dealerDiamonds3.Visible = true;
                                break;
                            case 4:
                                dealerDiamonds4.Visible = true;
                                break;
                            case 5:
                                dealerDiamonds5.Visible = true;
                                break;
                        }
                        break;
                    case 2:
                        switch (dealerCardCounter)
                        {
                            case 1:
                                dealerHearts1.Visible = true;
                                break;
                            case 2:
                                dealerHearts2.Visible = true;
                                break;
                            case 3:
                                dealerHearts3.Visible = true;
                                break;
                            case 4:
                                dealerHearts4.Visible = true;
                                break;
                            case 5:
                                dealerHearts5.Visible = true;
                                break;
                        }
                        break;
                    case 3:
                        switch (dealerCardCounter)
                        {
                            case 1:
                                dealerSpades1.Visible = true;
                                break;
                            case 2:
                                dealerSpades2.Visible = true;
                                break;
                            case 3:
                                dealerSpades3.Visible = true;
                                break;
                            case 4:
                                dealerSpades4.Visible = true;
                                break;
                            case 5:
                                dealerSpades5.Visible = true;
                                break;
                        }
                        break;
                    case 4:
                        switch (dealerCardCounter)
                        {
                            case 1:
                                dealerClubs1.Visible = true;
                                break;
                            case 2:
                                dealerClubs2.Visible = true;
                                break;
                            case 3:
                                dealerClubs3.Visible = true;
                                break;
                            case 4:
                                dealerClubs4.Visible = true;
                                break;
                            case 5:
                                dealerClubs5.Visible = true;
                                break;
                        }
                        break;
                }
            }
        }

        private void printCard(int cardplayer, int cardDealer, String value, bool player)
        {
            if(player)
            {
                switch (cardplayer)
                {
                    case 1:
                        label1.Text = value;
                        break;
                    case 2:
                        label2.Text = value;
                        break;
                    case 3:
                        label3.Visible = true;
                        label3.Text = value;
                        break;
                    case 4:
                        label4.Visible = true;
                        label4.Text = value;
                        break;
                    case 5:
                        label5.Visible = true;
                        label5.Text = value;
                        break;
                    case 6:
                        label6.Visible = true;
                        label6.Text = value;
                        break;
                    case 7:
                        label7.Visible = true;
                        label7.Text = value;
                        break;
                    case 8:
                        label8.Visible = true;
                        label8.Text = value;
                        break;
                }
            } else
            {
                switch (cardDealer)
                {
                    case 1:
                        dealerLabel1.Text = value;
                        break;
                    case 2:
                        dealerLabel2.Visible = true;
                        dealerLabel2.Text = value;
                        break;
                    case 3:
                        dealerLabel3.Visible = true;
                        dealerLabel3.Text = value;
                        break;
                    case 4:
                        dealerLabel4.Visible = true;
                        dealerLabel4.Text = value;
                        break;
                    case 5:
                        dealerLabel5.Visible = true;
                        dealerLabel5.Text = value;
                        break;
                }
            }
        }

        private void transaction(bool playerWin)
        {
            if(playerWin)
            {
                accountValue += currentBet;
            } else
            {
                accountValue -= currentBet;
            }
            accountValueLabel.Text = "$" + accountValue.ToString();
        }

        private void resetScreen()
        {
            currentBet = betArray[currentBetIndex];
            actualBetAmount.Text = "$" + currentBet;
            cardCounter = 1;
            dealerCardCounter = 1;
            handValue = 0;
            dealerHandValue = 0;
            aceCount = 0;
            dealerAceCount = 0;
            runOnce = true;
            doHit = false;
            canHit = false;

            resultsLabel.Text = "";
            resultsLabel.BackColor = Color.FromArgb(0, 64, 0);

            cardBlank3.Visible = false; label3.Visible = false; suitDiamonds3.Visible = false; suitHearts3.Visible = false; suitSpades3.Visible = false; suitClubs3.Visible = false;
            cardBlank4.Visible = false; label4.Visible = false; suitDiamonds4.Visible = false; suitHearts4.Visible = false; suitSpades4.Visible = false; suitClubs4.Visible = false;
            cardBlank5.Visible = false; label5.Visible = false; suitDiamonds5.Visible = false; suitHearts5.Visible = false; suitSpades5.Visible = false; suitClubs5.Visible = false;
            cardBlank6.Visible = false; label6.Visible = false; suitDiamonds6.Visible = false; suitHearts6.Visible = false; suitSpades6.Visible = false; suitClubs6.Visible = false;
            cardBlank7.Visible = false; label7.Visible = false; suitDiamonds7.Visible = false; suitHearts7.Visible = false; suitSpades7.Visible = false; suitClubs7.Visible = false;
            cardBlank8.Visible = false; label8.Visible = false; suitDiamonds8.Visible = false; suitHearts8.Visible = false; suitSpades8.Visible = false; suitClubs8.Visible = false;

            dealerBlank2.Visible = false; dealerLabel2.Visible = false; dealerDiamonds2.Visible = false; dealerHearts2.Visible = false; dealerSpades2.Visible = false; dealerClubs2.Visible = false;
            dealerBlank3.Visible = false; dealerLabel3.Visible = false; dealerDiamonds3.Visible = false; dealerHearts3.Visible = false; dealerSpades3.Visible = false; dealerClubs3.Visible = false;
            dealerBlank4.Visible = false; dealerLabel4.Visible = false; dealerDiamonds4.Visible = false; dealerHearts4.Visible = false; dealerSpades4.Visible = false; dealerClubs4.Visible = false;
            dealerBlank5.Visible = false; dealerLabel5.Visible = false; dealerDiamonds5.Visible = false; dealerHearts5.Visible = false; dealerSpades5.Visible = false; dealerClubs5.Visible = false;

            dealerAmountLabel.Text = "";
            betUp.Enabled = true; betUp.BackColor = SystemColors.ButtonFace;
            betDown.Enabled = true; betDown.BackColor = SystemColors.ButtonFace;
        }

        private void testbutton_Click(object sender, EventArgs e)
        {
            Main form1 = new Main();

            form1.Show();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dealButton_Click(object sender, EventArgs e)
        {

            if(runOnce)
            {
                dealButton.BackColor = SystemColors.ControlDark;
                dealButton.Enabled = false;
                betUp.Enabled = false; betUp.BackColor = SystemColors.ControlDark;
                betDown.Enabled = false; betDown.BackColor = SystemColors.ControlDark;
                runGame();
            } else
            {
                resetScreen();
                dealButton.BackColor = SystemColors.ControlDark;
                dealButton.Enabled = false;
                betUp.Enabled = false; betUp.BackColor = SystemColors.ControlDark;
                betDown.Enabled = false; betDown.BackColor = SystemColors.ControlDark;
                runGame();
            }

            
        }

        private void hitButton_Click(object sender, EventArgs e)
        {
            if(canHit)
            {
                doubledownButton.Enabled = false; doubledownButton.BackColor = SystemColors.ControlDark;
                doHit = true;
                runGame();
            }
        }

        private void stayButton_Click(object sender, EventArgs e)
        {
            stayButton.Enabled = false; stayButton.BackColor = SystemColors.ControlDark;
            hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
            dealerTurn = true;
            runGame();
        }

        private void splitButton_Click(object sender, EventArgs e)
        {

        }

        private void doubledownButton_Click(object sender, EventArgs e)
        {
            actualBetAmount.Text = actualBetAmount.Text + " + $" + currentBet;
            currentBet *= 2;
            doubleDown = true;
            doubledownButton.Enabled = false; doubledownButton.BackColor = SystemColors.ControlDark;
            hitButton.Enabled = false; hitButton.BackColor = SystemColors.ControlDark;
            runGame();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void betUp_Click(object sender, EventArgs e)
        {
            if(currentBetIndex < 7)
            {
                currentBetIndex++;
                currentBet = betArray[currentBetIndex];
                betDisplayBox.Text = currentBet.ToString();
                updateBet.BackColor = SystemColors.ButtonFace;
                updateBet.Enabled = true;
            }
        }

        private void betDown_Click(object sender, EventArgs e)
        {
            if (currentBetIndex > 0)
            {
                currentBetIndex--;
                currentBet = betArray[currentBetIndex];
                betDisplayBox.Text = currentBet.ToString();
                updateBet.BackColor = SystemColors.ButtonFace;
                updateBet.Enabled = true;
            }
        }

        private void updateBet_Click(object sender, EventArgs e)
        {
            if(currentBet > accountValue)
            {
                dealButton.Enabled = false; dealButton.BackColor = SystemColors.ControlDark;
                updateBet.BackColor = SystemColors.ControlDark;
                updateBet.BackColor = SystemColors.ControlDark;
                updateBet.Enabled = false;
            } else
            {
                actualBetAmount.Text = "$" + betArray[currentBetIndex];

                dealButton.Enabled = true; dealButton.BackColor = SystemColors.ButtonFace;
                updateBet.BackColor = SystemColors.ControlDark;
                updateBet.BackColor = SystemColors.ControlDark;
                updateBet.Enabled = false;
            }
        }
    }
}
