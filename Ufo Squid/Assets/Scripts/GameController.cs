using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    private float gameStartTime;
    private bool gameStarted;

    public float difficultyScale = 1f;

    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath;

    public delegate void GameStartEventHandler();
    public event GameStartEventHandler OnGameStart;

    ///public delegate void AfterDeathAnimHandler();
    ///public event AfterDeathAnimHandler AfterDeathAnim;

    private bool music = true;

    public bool IsMusicActive => music;

    public float patternBoost;

    public Color selectedSkin;
    public Color openedSkin;
    public Color closedSkin;

    private EnemyController enemyController;
    private MenuEvents menuEvents;
    private SoundController soundController;
    private PlayerController player;
    public Transform shop;

    [SerializeField]
    private int money;
    private int highScore;
    private int score;

    private int playerSkin = 1;

    private bool[] isSkinBought = new bool[7] { true, false, false, false, false, false, false };

    private bool firstExec;

    private TextMeshProUGUI[] highScoreTexts;
    private TextMeshProUGUI[] scoreTexts;
    private TextMeshProUGUI[] moneyTexts;

    public float Difficulty
    {
        get => difficultyScale * (Time.time - gameStartTime) * 0.01f + 1f;
    }
    public bool GameStarted => gameStarted;

    private void Awake()
    {
        firstExec = !Load();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        enemyController = FindObjectOfType<EnemyController>();
        menuEvents = FindObjectOfType<MenuEvents>();
        soundController = FindObjectOfType<SoundController>();
        player = FindObjectOfType<PlayerController>(true);

        for (int i = 0; i < 7; i++)
        {
            Image skinBtn = shop.GetChild(i).GetChild(1).gameObject.GetComponent<Image>();

            if (playerSkin - 1 == i)
            {
                skinBtn.color = selectedSkin;
                skinBtn.transform.parent.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            }
            else if (isSkinBought[i])
            {
                skinBtn.color = openedSkin;
                skinBtn.transform.parent.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            }
            else
                skinBtn.color = closedSkin;
        }

        // Fuck, that so looong
        highScoreTexts = FindObjectsOfType<GameObject>(true).Where(obj => obj.tag == "High Score").Select(obj => obj.GetComponent<TextMeshProUGUI>()).ToArray();
        scoreTexts = FindObjectsOfType<GameObject>(true).Where(obj => obj.tag == "Score").Select(obj => obj.GetComponent<TextMeshProUGUI>()).ToArray();
        moneyTexts = FindObjectsOfType<GameObject>(true).Where(obj => obj.tag == "Money").Select(obj => obj.GetComponent<TextMeshProUGUI>()).ToArray();

        soundController.SetMusicMute(!music);
        ChangeStatsText();
    }

    public void SetSkin() =>
        player.GetComponent<Animator>().SetInteger("Skin", playerSkin);

    public void SkinButtonPressed( GameObject skinBtn )
    {
        int index = skinBtn.transform.GetSiblingIndex();

        if (index == playerSkin - 1)
            return;

        if (isSkinBought[index])
            SelectSkin(skinBtn, index);
        else
            TryBuySkin(skinBtn, index);
    }

    private void TryBuySkin( GameObject skinBtn, int index )
    {
        TextMeshProUGUI costField = skinBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        int cost = int.Parse(costField.text);

        if (money < cost)
            return;

        isSkinBought[index] = true;

        money -= cost;

        costField.text = "";
        skinBtn.transform.GetChild(1).GetComponent<Image>().color = openedSkin;

        ChangeStatsText();
        soundController.PlaySound("SkinBuy");
    }

    public void AddTrackToQueue( string name ) => soundController.AddTrackToQueue(name);

    private void SelectSkin( GameObject skinBtn, int index )
    {
        shop.transform.GetChild(playerSkin - 1).GetChild(1).GetComponent<Image>().color = openedSkin;
        playerSkin = index + 1;
        shop.transform.GetChild(index).GetChild(1).GetComponent<Image>().color = selectedSkin;
        soundController.PlaySound("SkinSelect");
    }

    public void SwitchMusic(string name, float repeatTime = 10f) => soundController.SwitchMusic(name, repeatTime);

    public void StartGame()
    {
        gameStartTime = Time.time;
        gameStarted = true;

        OnGameStart.Invoke();
    }

    public void ChangeSoundState()
    {
        soundController.SetMusicMute(music);
        music = !music;
    }

    public void Death()
    {
        gameStarted = false;

        score = Convert.ToInt32((Time.time - gameStartTime) * 10f);

        if (score > highScore)
            highScore = score;

        soundController.SwitchMusic("DeathMenu", 38.4f);

        OnDeath.Invoke();
        ChangeStatsText();
    }

    public void AfterDeath()
    {
        var playerTrans = player.transform;

        playerTrans.localPosition = new Vector3(0, playerTrans.localPosition.y, 1);
        playerTrans.localScale = new Vector3(Mathf.Abs(playerTrans.localScale.x), playerTrans.localScale.y, 1);
    }

    private void ChangeStatsText()
    {
        foreach (TextMeshProUGUI highScoreField in highScoreTexts)
            highScoreField.text = highScore.ToString();

        foreach (TextMeshProUGUI scoreField in scoreTexts)
            scoreField.text = score.ToString();

        foreach (TextMeshProUGUI moneyField in moneyTexts)
            moneyField.text = money.ToString();
    }

    private void SaveOld()
    {
        string saveName = Application.persistentDataPath + "/ded.inside";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = new FileStream(saveName, FileMode.Create);

        formatter.Serialize(file, money);
        formatter.Serialize(file, highScore);
        formatter.Serialize(file, playerSkin);
        formatter.Serialize(file, isSkinBought);
        formatter.Serialize(file, music);

        file.Close();
    }

    private bool LoadOld()
    {
        string saveName = Application.persistentDataPath + "/ded.inside";

        if (!File.Exists(saveName))
        {
            Debug.LogWarning("Save file is not found");
            return false;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = new FileStream(saveName, FileMode.Open);

        money = (int)formatter.Deserialize(file);
        highScore = (int)formatter.Deserialize(file);
        playerSkin = (int)formatter.Deserialize(file);
        isSkinBought = formatter.Deserialize(file) as bool[];
        music = (bool)formatter.Deserialize(file);

        file.Close();

        return true;
    }

    private int GetBoolInInt()
    {
        int i = 0;

        i |= Convert.ToInt32(music);
        i |= Convert.ToInt32(isSkinBought[0]) << 1;
        i |= Convert.ToInt32(isSkinBought[1]) << 2;
        i |= Convert.ToInt32(isSkinBought[2]) << 3;
        i |= Convert.ToInt32(isSkinBought[3]) << 4;
        i |= Convert.ToInt32(isSkinBought[4]) << 5;
        i |= Convert.ToInt32(isSkinBought[5]) << 6;
        i |= Convert.ToInt32(isSkinBought[6]) << 7;

        return i;
    }

    private void SetBoolFromInt( int i )
    {
        music = Convert.ToBoolean(i & 1);
        isSkinBought[0] = Convert.ToBoolean(i & (1 << 1));
        isSkinBought[1] = Convert.ToBoolean(i & (1 << 2));
        isSkinBought[2] = Convert.ToBoolean(i & (1 << 3));
        isSkinBought[3] = Convert.ToBoolean(i & (1 << 4));
        isSkinBought[4] = Convert.ToBoolean(i & (1 << 5));
        isSkinBought[5] = Convert.ToBoolean(i & (1 << 6));
        isSkinBought[6] = Convert.ToBoolean(i & (1 << 7));
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetInt("PlayerSkin", playerSkin);
        PlayerPrefs.SetInt("Booleans", GetBoolInInt());
        PlayerPrefs.Save();
    }

    private bool Load()
    {
        bool isOk = true;

        if (PlayerPrefs.HasKey("Money"))
            money = PlayerPrefs.GetInt("Money");
        else
            isOk = false;

        if (PlayerPrefs.HasKey("HighScore"))
            highScore = PlayerPrefs.GetInt("HighScore");
        else
            isOk = false;

        if (PlayerPrefs.HasKey("PlayerSkin"))
            playerSkin = PlayerPrefs.GetInt("PlayerSkin");
        else
            isOk = false;

        if (PlayerPrefs.HasKey("Booleans"))
            SetBoolFromInt(PlayerPrefs.GetInt("Booleans"));
        else
            isOk = false;

        return isOk;
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    private void FixedUpdate()
    {
        if (patternBoost > 0.5f)
            money++;

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
