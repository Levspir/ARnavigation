using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateDisplayController : MonoBehaviour
{
    /* change app bar, app text, toast and sound */

	private GameObject actionBar;
    private Image iconState;
    private ToastMessageScript toastMessageScript;
    private SoundManager soundManager;
	private Image actionBarImg;
	private Text appNameText;
    public SoundManager.SoundType[] soundqueue;
    public Sprite IconIdleSprite, IconNavigateSprite;
    private int currentSoundIndex = 0;

    // Use this for initialization
    void Start()
    {
		InitStateDisplay();
    }

	void Awake()
	{
		InitStateDisplay();
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void InitStateDisplay()
    {
		actionBar = gameObject.GetComponent<CanvasButtonScript>().actionBar;
        iconState = actionBar.transform.Find("HambergerButton").gameObject.GetComponent<Image>();
        toastMessageScript = gameObject.GetComponent<ToastMessageScript>();
        soundManager = GameObject.Find("ARCamera").GetComponent<SoundManager>();
        actionBarImg = actionBar.GetComponent<Image>();
        appNameText = actionBar.gameObject.transform.Find("AppName").gameObject.GetComponent<Text>();
        soundqueue = new SoundManager.SoundType[3];
    }

    public void ChangeActionBarColor(MainController.AppState color)
    {
        switch (color)
        {
            case MainController.AppState.Idle:
                actionBarImg.color = new Color32(60, 126, 255, 255);
                iconState.sprite = IconIdleSprite;
                break;
            case MainController.AppState.Navigate:
                actionBarImg.color = new Color32(126, 60, 255, 255);
                iconState.sprite = IconNavigateSprite;
                break;
            default: 
                actionBarImg.color = new Color32(60, 126, 255, 255); 
                iconState.sprite = IconIdleSprite;
                break;
        }
    }

	public void ShowToastMessage(string toastmsg)
	{
		toastMessageScript.showToastOnUiThread(toastmsg, true);
	}

	public void ShowToastMessage(string toastmsg, bool isshort)
	{
		toastMessageScript.showToastOnUiThread(toastmsg, isshort);
	}

    public void ChangeActionText(string actext)
    {
        appNameText.text = actext;
    }

    public void PlaySound(SoundManager.SoundType soundtype)
    {
        soundManager.Play(soundtype);
    }

//    public void PlaySoundQueue()
//    {
//        List<int> soundlist = new List<int>();
//        foreach (SoundManager.SoundType sound in soundqueue)
//        {
//            if(sound != 0)
//            {
//                soundlist.Add((int)sound);
//            }
//        } 
//        soundManager.PlayQueue(soundlist);
//        Array.Clear(soundqueue, 0, soundqueue.Length);
//       
//        currentSoundIndex = 0;
//    }

    public void AddSound(SoundManager.SoundType soundtype, int index)
    {
        soundqueue[index] = soundtype;
    }

    #region Add Sound by List unused

    // /* replace sound into current index  And shift one index for next sound */
    // public void AddPrioritySoundQueue(SoundManager.SoundType soundtype)
    // {
    //     Debug.Log("AddPriority "+soundtype + "  to " + currentSoundIndex);
    //     if(soundqueue.Count-1 >= currentSoundIndex)
    //     {
    //         soundqueue[currentSoundIndex] = (int)soundtype;
    //     }
    //     else
    //     {
    //         soundqueue.Add((int)soundtype);
    //     }
        
    //     currentSoundIndex += 1;
    // }

    // /* replace current sound into current index */
    // public void AddSoundQueue(SoundManager.SoundType soundtype)
    // {
    //     Debug.Log(soundqueue.Count + "Add sound "+soundtype + "  to " + currentSoundIndex);
    //     if(soundqueue.Count-1 >= currentSoundIndex)
    //     {
    //         soundqueue[currentSoundIndex] = (int)soundtype;
    //     }
    //     else
    //     {
    //         soundqueue.Add((int)soundtype);
    //     }
        
    // }
    
    // /* skip current sound and put sound to next or last index */
    // public void AddNextSoundQueue(SoundManager.SoundType soundtype) //unused
    // {
    //     Debug.Log("Add next "+soundtype + "  to " + currentSoundIndex);
    //     if(currentSoundIndex != 0)
    //     {
            
    //     }
    //     soundqueue.Add((int)soundtype);
    //     currentSoundIndex += 1;
    // }

    #endregion


    /* get roataion and defind sound type */
    public SoundManager.SoundType GetSoundDirection(float zRotation)
    {
        if(zRotation > 180){ zRotation = zRotation - 360;}
        float abszRotation = Mathf.Abs(zRotation % 360f);
        if(abszRotation < 30)
        {
            return SoundManager.SoundType.GoBack;
        }
        else if(abszRotation > 150)
        {
            return SoundManager.SoundType.GoAhead;
        }
        else
        {
            if(zRotation < 0)
            {
                return SoundManager.SoundType.TurnRight;
            }
            else if(zRotation > 0)
            {
                return SoundManager.SoundType.TurnLeft;
            }
        }
        return SoundManager.SoundType.GoAhead;
    }
}
