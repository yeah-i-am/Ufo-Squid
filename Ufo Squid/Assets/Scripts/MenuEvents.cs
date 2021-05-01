using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEvents : MonoBehaviour
{
    private GameController gameController;
    private int soundButtonState = 0;

    public Animator gameLayerAnimator;
    public Animator UILayerAnimator;
    public Animator BordersAnimator;

    public Image soundButton;
    public Sprite[] soundButtonSprites;

    public void StartButtonPressed()
    {
        gameLayerAnimator.SetBool("GameStarted", true);
        UILayerAnimator.SetBool("GameStarted", true);
        BordersAnimator.SetBool("GameStarted", true);
    }

    public void SoundButtonPressed()
    {
        gameController.ChangeSoundState();

        soundButtonState = 1 - soundButtonState;

        soundButton.sprite = soundButtonSprites[soundButtonState];
    }

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void Update()
    {
        
    }
}
