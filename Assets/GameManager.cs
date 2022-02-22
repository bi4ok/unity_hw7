using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ImageTimer HarvestTimer;
    public ImageTimer EatingTimer;
    public Image RaidTimerImg;
    public Image PeasantTimerImg;
    public Image WarriorTimerImg;

    public Button peasantButton;
    public Button warriorButton;

    public Text resourcesText;
    public Text raidWarriorsCountText;
    public Text beforeRaidCountText;
    public Text gameOverText;
    public Text gameWinText;

    public int peasantCount;
    public int warriorCount;
    public int wheatCount;

    public int wheatPerPeasant;
    public int wheatToWarrior;

    public int warriorCost;
    public int peasantCost;

    public float peasantCreateTime;
    public float warriorCreateTime;
    public float raidMaxTime;

    public int raidIncrease;
    public int nextRaid;
    public int ticksBeforeRaid;
    public int raidCountToWin;

    private float _peasantTimer = -2;
    private float _warriorTimer = -2;
    private float _raidTimer;
    private int _raidCount;

    private int _allWarriorsCount = 0;
    private int _allPeasantsCount = 0;
    private int _allRaidsCount = 0;
    private int _allWheatCount = 0;

    public AudioSource clickSound;
    public AudioSource music;
    public AudioSource eatingSound;
    public AudioSource newWarrirorSound;
    public AudioSource newPeasantSound;
    public AudioSource raidSound;
    public AudioSource wheatAddSound;

    public GameObject GameOverScreen;
    public GameObject GameWinScreen;
    public GameObject GamePauseScreen;
    public GameObject GameScreen;

    void Start()
    {
        GameOverScreen.SetActive(false);
        GameWinScreen.SetActive(false);
        GamePauseScreen.SetActive(false);
        GameScreen.SetActive(true);
        UpdateText();
        _raidTimer = raidMaxTime;

        music.Play();
    }

    void Update()
    {
        // Логика набегов
        _raidTimer -= Time.deltaTime;
        RaidTimerImg.fillAmount = _raidTimer / raidMaxTime;

        if (_raidTimer <= 0)
        {
            _raidCount += 1;
            if (_raidCount > ticksBeforeRaid)
            {
                raidSound.Play();
                _allRaidsCount++;
                warriorCount -= nextRaid;
                nextRaid += raidIncrease;
            }
            _raidTimer = raidMaxTime;
        }


        // Логика добывания пшеницы
        if (HarvestTimer.Tick)
        {
            wheatAddSound.Play();
            int newWheat = peasantCount * wheatPerPeasant;
            wheatCount += newWheat;
            _allWheatCount += newWheat;

        }

        // Логика поедания пшеницы
        if (EatingTimer.Tick)
        {
            eatingSound.Play();
            wheatCount -= warriorCount * wheatToWarrior;
        }

        // Работа с таймером найма крестьян
        if(_peasantTimer > 0)
        {
            _peasantTimer -= Time.deltaTime;
            PeasantTimerImg.fillAmount = _peasantTimer / peasantCreateTime;
        }
        else if (_peasantTimer > -1)
        {
            newPeasantSound.Play();
            PeasantTimerImg.fillAmount = 1;
            peasantButton.interactable = true;
            peasantCount += 1;
            _allPeasantsCount++;
            _peasantTimer = -2;
        }
        else
        {
            peasantButton.interactable = wheatCount >= peasantCost ? true : false;
        }

        // Работа с таймером найма воинов
        if (_warriorTimer > 0)
        {
            _warriorTimer -= Time.deltaTime;
            WarriorTimerImg.fillAmount = _warriorTimer / warriorCreateTime;
        }
        else if (_warriorTimer > -1)
        {
            newWarrirorSound.Play();
            WarriorTimerImg.fillAmount = 1;
            warriorButton.interactable = true;
            warriorCount += 1;
            _allWarriorsCount++;
            _warriorTimer = -2;
        }
        else
        {
            warriorButton.interactable = wheatCount >= warriorCost ? true : false;
        }


        // Проверка условий победы/поражения
        if(warriorCount < 0)
        {
            Time.timeScale = 0;
            GameOverScreen.SetActive(true);
            gameOverText.text = $"Пережито набегов:{_allRaidsCount}." +
                $"\nНанято воинов:{_allWarriorsCount}." +
                $"\nНанято крестьян:{_allPeasantsCount}." +
                $"\nПроизведено пшеницы:{_allWheatCount}.";
        }
        else if (_raidCount >= raidCountToWin)
        {
            Time.timeScale = 0;
            GameWinScreen.SetActive(true);
            gameWinText.text = $"Пережито набегов:{_allRaidsCount}." +
                $"\nНанято воинов:{_allWarriorsCount}." +
                $"\nНанято крестьян:{_allPeasantsCount}." +
                $"\nПроизведено пшеницы:{_allWheatCount}.";
        }



        UpdateText();
    }

    public void CreateWarrior()
    {
        clickSound.Play();  // Очень не хотелось везде дублировать эту строку, но пока не придумал решения лучше
        wheatCount -= warriorCost;
        _warriorTimer = warriorCreateTime;
        warriorButton.interactable = false;

    }

    public void CreatePeasant()
    {
        clickSound.Play();
        wheatCount -= peasantCost;
        _peasantTimer = peasantCreateTime;
        peasantButton.interactable = false;

    }

    public void MusicPauseButton()
    {
        clickSound.Play();
        if (music.isPlaying)
        {
            music.Pause();
        }
        else
        {
            music.Play();
        }
    }

    public void PauseButtonClick()
    {
        clickSound.Play();
        Debug.Log(Time.timeScale);
        if(Time.timeScale != 0)
        {
            Time.timeScale = 0;
            GamePauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            GamePauseScreen.SetActive(false);
        }
        Debug.Log(GamePauseScreen);


    }

    private void UpdateText()
    {
        resourcesText.text = peasantCount + "\n" + warriorCount + "\n\n" + wheatCount;
        raidWarriorsCountText.text = $"{nextRaid}";
        int ticksToRaid = (ticksBeforeRaid - _raidCount) > 0 ? (ticksBeforeRaid - _raidCount) : 0;
        beforeRaidCountText.text = $"{ticksToRaid}";
    }
}
