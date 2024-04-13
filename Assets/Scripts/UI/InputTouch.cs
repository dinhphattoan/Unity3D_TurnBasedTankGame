using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTouch : MonoBehaviour
{
   public int tankNumber;
    public TankMovement Tank;
    public float moveSpeed;
    public float tankMoveCounter;  //Axis for moving (Vertical)
    public float tankTurnCounter;   // Axis for turning (Horizontal)
    public TankShooting TankShooting;
    private void Start()
    {
        
        tankMoveCounter=Tank.GetComponent<TankMovement>().m_MovementInputValue;
        tankTurnCounter = Tank.GetComponent<TankMovement>().m_TurnInputValue;
    } 
    // Start is called before the first frame update
    public void upBtn_Click()
    {
        if (Tank.m_MovementInputValue <= 1)
            Tank.m_MovementInputValue =1;
    }
    public void downBtn_Click()
    {
        if (Tank.m_MovementInputValue >= -1)
            Tank.m_MovementInputValue =1;
    }
    public void leftBtn_Click()
    {
        if (Tank.m_TurnInputValue >= -1)
            Tank.m_TurnInputValue =-1;
    }
    public void rightBtn_Click()
    {
        if (Tank.m_TurnInputValue <= 1)
            Tank.m_TurnInputValue = 1;
    }
    // Update is called once per frame
    void Update()
    {
        //if(tankMoveCounter>0)
        //{
        //    tankMoveCounter-=0.1f;
        //}
        //else if(tankMoveCounter<0)
        //{
        //    tankMoveCounter+=0.1f;
        //}
        //if (Tank.m_TurnInputValue > 0)
        //{
        //    Tank.m_TurnInputValue -= 0.1f;
        //}
        //else if (Tank.m_TurnInputValue < 0)
        //{
        //    tankTurnCounter += 0.1f;
        //}
    }
}
