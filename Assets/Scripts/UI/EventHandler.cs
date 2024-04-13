using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Image image;
    public bool isLoad = false;
    public bool isBlackout = false;
    public bool isFaded = false;
    public bool isbutton1Click = false;
    public bool isbutton2Click = false;
    public int cycleCount = 1;
    private int cycleCounter = 0;
    private void Start()
    {
        
    }
    public void button1Click()
    {
        isBlackout = true;
        isbutton2Click = false;
        isbutton1Click = true;
    }
    public void button2Click()
    {
        isBlackout = true;
        isbutton2Click = true;
        isbutton1Click = false;
    }
    private void Update()
    {
        if (isBlackout&&cycleCounter<cycleCount)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + 1);
            isFaded = false;
            if (image.color.a >= 255)
            {
                isBlackout = false;
                isFaded = true;
                if(isbutton1Click)
                {
                    SceneManager.LoadScene("Main");
                }
                if(isbutton2Click)
                {
                    SceneManager.LoadScene("");
                }
            }
        }

        if (isFaded)
        {
            isBlackout = false;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a -1);
            if (image.color.a<=0)
            {
                if(isBlackout==false)
                {
                    cycleCounter++;
                }
                isBlackout = true;
                isFaded = false;
            }
        }
    }

}
