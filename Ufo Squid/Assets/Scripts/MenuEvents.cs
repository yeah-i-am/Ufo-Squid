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

        //gameController.SwitchMusic("MenuToGame");
        //gameController.AddTrackToQueue("MenuToGame");
        //gameController.AddTrackToQueue("Game");
        gameController.SwitchMusic("Game", 38.4f);
    }

    public void RestartButtonPressed()
    {
        UILayerAnimator.SetInteger("DeathChoise", 1);
        StartButtonPressed();
    }

    public void MenuButtonPressed()
    {
        UILayerAnimator.SetInteger("DeathChoise", 2);
        gameController.SwitchMusic("Menu");
    }

    public void ShopButtonPressed()
    {
        gameController.SwitchMusic("Shop");
        UILayerAnimator.SetBool("Shop", true);
    }

    public void SkinButtonPressed( GameObject skinBtn )
    {
        gameController.SkinButtonPressed(skinBtn);
    }

    public void FromShopToMenuButtonPressed()
    {
        gameController.SwitchMusic("Menu");
        UILayerAnimator.SetBool("Shop", false);
    }

    public void OnDeath()
    {
        UILayerAnimator.SetInteger("DeathChoise", 0);
        UILayerAnimator.SetBool("GameStarted", false);
        gameLayerAnimator.SetBool("GameStarted", false);
        BordersAnimator.SetBool("GameStarted", false);
    }

    public void SoundButtonPressed()
    {
        gameController.ChangeSoundState();

        soundButtonState = 1 - soundButtonState;

        soundButton.sprite = soundButtonSprites[soundButtonState];
    }

    public void SetSoundButtonState()
    {
        if (gameController.IsMusicActive)
            soundButtonState = 0;
        else
            soundButtonState = 1;

        soundButton.sprite = soundButtonSprites[soundButtonState];
    }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        gameController.OnDeath += OnDeath;
        SetSoundButtonState();
    }

    public void Update()
    {
        
    }
}
