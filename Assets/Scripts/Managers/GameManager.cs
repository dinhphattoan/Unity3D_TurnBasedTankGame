using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
public enum PlayerTurn { PLAYER1,PLAYER2}
public class GameManager : MonoBehaviour
{   
    [Header("Game Rounds")]
    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    [Header("Game time speed")]
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    public float m_TurnDelay = 1f;              // The delay time of alterate turn.
    public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
    [Header ("UI text")]
    public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public Text m_MessageTextTurn;
    public Text m_MessageTurnPoint;
    public Text m_MessageBuff;
    [Header("Sound Clip")]
    public AudioSource pickupClip;
    //-------------Screen Instances----------------
    //Screen UI
    public GameObject screenControlUI;
    public bool isUIOn;
    //---------

    //--------------Instances-----------------
    public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
    public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.
    public Rigidbody m_Shell;
    //----------------------------------------
    private int m_RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.
    //----------------------------------------
    public bool isGameStart = false;
    public int TankTurn = 0;
    private int oldTurn;

    [Header("Buff values")]
    public int shieldEarnValue=3;
    public int ATKEarnValue=3;
    public bool ATKBuffOn;
    //Shield, ATK
    Dictionary<int, int[]> tanksBuff = new Dictionary<int, int[]>();
    private void FixedUpdate()
    {
        screenControlUI.SetActive(isUIOn);
        if(TankTurn!=oldTurn)
        {
            EnableTankControl();
            oldTurn = TankTurn;
        }
        m_MessageTurnPoint.text = "Turn Point: " + ((int)m_Tanks[TankTurn].m_Movement.valueTurn).ToString();
        m_MessageBuff.text = currentTanksBuff();
        
         m_Shell.GetComponent<ShellExplosion>().isBuffed = ATKBuffOn || tanksBuff[TankTurn][1]>0;
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    [Obsolete]
    private void Start()
    {
        // Create the delays so they only have to be made once.
        oldTurn = TankTurn;
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);
        SpawnAllTanks();
        SetCameraTargets();
        isUIOn = DeviceType.Handheld == SystemInfo.deviceType;
       
        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine(GameLoop());
    }
    private void SpawnAllTanks()
    {

        // For all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... create them, set their player number and references needed for control.
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
            m_Tanks[i].EnableControl();
            if (DeviceType.Handheld == SystemInfo.deviceType)
            {
                m_Tanks[i].m_Movement.isMobileControl = true;
            }
            tanksBuff.Add(i,new int[2]{ 0,0});
            
        }
    }
    public void earnATKPlayer()
    {
        pickupClip.Play();
        tanksBuff[TankTurn][1] += ATKEarnValue;

    }
    public void earnShieldPlayer()
    {
        pickupClip.Play();
        tanksBuff[TankTurn][0] += shieldEarnValue;
    }
    
    private void nextTurnChange()
    {
        for(int i=0;i<tanksBuff.Count;i++)
        {
            for (int j = 0; j < 2; j++) 
                if(tanksBuff[i][j]>0)
                tanksBuff[i][j]--;
        }
    }
    private string currentTanksBuff()
    {
        string result="";
        for (int i = 0; i < tanksBuff.Count; i++)
        {
            result += "Player " + (i+1) + ": ";
            if (tanksBuff[i][0] > 0)
            {
                m_Tanks[i].TankHealth.isDefBuff = true;
                result += "\n\t- Protection (" + tanksBuff[i][0].ToString() + " turn left!)";
            }
            else m_Tanks[i].TankHealth.isDefBuff = false;
            if (tanksBuff[i][1] > 0)
            {
                m_Tanks[i].TankHealth.isATKBuff = true;
                result += "\n\t- ATK Buff (" + tanksBuff[i][1].ToString() + " turn left!)";
            }
            else m_Tanks[i].TankHealth.isATKBuff = false;
            if(i!=tanksBuff.Count-1)
            {
                result += "\n";
            }

        }return result;
    }
    
    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of tanks.
        Transform[] targets = new Transform[m_Tanks.Length];

        // For each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate tank transform.
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // These are the targets the camera should follow.
        m_CameraControl.m_Targets = targets;
    }


    // This is called from start and will run each phase of the game one after another.
    [Obsolete]
    private IEnumerator GameLoop ()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundStarting ());
        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine (RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (m_GameWinner != null)
        {
            
            // If there is a game winner, restart the level.
            Application.LoadLevel (Application.loadedLevel);
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine (GameLoop ());
        }
    }

    
    private IEnumerator RoundStarting ()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllTanks ();
        //DisableTankControl ();
        EnableAllTank();
        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetStartPositionAndSize ();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;
        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }
    
    


    private IEnumerator RoundPlaying()
    {
        // As soon as the round begins playing let the players control the tanks.

        EnableTankControl();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;
        
        // While there is not one tank left...
        while (!OneTankLeft())
        {
            if (m_Tanks[TankTurn].m_Movement.valueTurn<=0)
            {
                TankTurn++;
                EnableTankControl();
            }
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding ()
    {
        // Stop tanks from moving.
        DisableTankControl ();

        // Clear the winner from the previous round.
        m_RoundWinner = null;

        // See if there is a winner now the round is over.
        m_RoundWinner = GetRoundWinner ();

        // If there is a winner, increment their score.
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage ();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }


    // This is used to check if there is one or fewer tanks remaining and thus the round should end.
    private bool OneTankLeft()
    {
        // Start the count of tanks left at zero.
        int numTanksLeft = 0;

        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if they are active, increment the counter.
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        // If there are one or fewer tanks remaining return true, otherwise return false.
        return numTanksLeft <=1;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private TankManager GetRoundWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }


    // This function is to find out if there is a winner of the game.
    private TankManager GetGameWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        // If no tanks have enough rounds to win, return null.
        return null;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a winner then change the message to reflect that.
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Go through all the tanks and add each of their scores to the message.
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // If there is a game winner, change the entire message to reflect that.
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    public bool Buttondown;
    // This function is used to turn all the tanks back on and reset their positions and properties.
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }
    
    public void horizontalTank(bool flag)
    {
        m_Tanks[TankTurn].m_Movement.m_TurnInputValue = (flag) ? 1 : -1;
    }
    public void VerticalTank(bool flag)
    {
        m_Tanks[TankTurn].m_Movement.m_MovementInputValue = (flag) ? 1 : -1;
    }
    public void resetHorizontalValue()
    {
        m_Tanks[TankTurn].m_Movement.m_TurnInputValue = 0;
    }
    public void resetVerticalValue()
    {
        m_Tanks[TankTurn].m_Movement.m_MovementInputValue = 0;
    }
    //----------------------------------EVENT HANDLER (For Windows Cursor)----------------------------------------------
    //-----Control Panel-------
    //left
    public void downPanelLeft()
    {
        horizontalTank(false);
    }
    public void upPanelLeft()
    {
        resetHorizontalValue();
    }
    //Up
    public void upPanelUp()
    {
        resetVerticalValue();
    }
    public void downPanelUp()
    {
        VerticalTank(true);
    }
    //right
    public void upPanelRight()
    {
        resetHorizontalValue();
    }
    public void downPanelRight()
    {
        horizontalTank(true);
    }
    //down
    public void downPanelDown()
    {
        VerticalTank(false);
    }
    public void upPanelDown()
    {
        resetVerticalValue();
    }
    //---Shoot Button
    public void PointerButtonDown()
    {
        m_Tanks[TankTurn].m_Shooting.isButtonDown = true;
        
        Buttondown = true;
    }
    public void PointerButtonUp()
    {
        Buttondown = false; m_Tanks[TankTurn].m_Movement.valueTurn -= 80;
        m_Tanks[TankTurn].m_Shooting.isButtonDown = false;
        
    }
    //-----------------------------------------------------------
    private void EnableTankControl()
    {
        if (TankTurn >= m_Tanks.Length) TankTurn = 0;
        if (TankTurn < 0) TankTurn = m_Tanks.Length-1;
        m_Tanks[oldTurn].m_Movement.valueTurn = 0;
        m_Tanks[TankTurn].m_Movement.valueTurn = 150;
        oldTurn = TankTurn;
       m_MessageTextTurn.text = "Tank " + (TankTurn + 1).ToString() + " Turn!";
        nextTurnChange();
    }
    private void EnableAllTank()
    {
        foreach(var im in m_Tanks)
        {
            im.EnableControl();
        }
    }

    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}