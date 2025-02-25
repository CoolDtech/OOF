using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        Gamplay,
        Paused,
        GameOver,
        LevelUp,
        OOF

    }
    
    //存取當前遊戲狀態
    public GameState currentState;
    //存取前一個遊戲狀態
    public GameState previousState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;


    [Header("視窗")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;

    [Header("當前屬性顯示")]
    //當前屬性顯示
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;
    public TMP_Text currentDefenceDisplay;

    [Header("結算畫面顯示物件")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("計時器")]
    public float timeLimit; //時限
    float stopwatchTime; //計時器時間
    public TMP_Text stopwatchDisplay;

    //檢查如果遊戲結束
    public bool isGameOver = false;
    //檢查玩家是否在選擇升級
    public bool choosingUpgrade;
    //玩家物件參照物
    public GameObject playerObject;

    void Awake()
    {
        //檢查是否有多餘的該類別實例(instance)
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("多餘的" + this + "已經被吃掉了");
            Destroy(gameObject);
        }
        DisableScreens();
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.Gamplay:
                //遊戲中的情況
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            
            case GameState.Paused:
                //暫停的情況
                CheckForPauseAndResume();
                break;
            
            case GameState.GameOver:
                //你輸了的情況
                if(!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f; //停止遊戲時間
                    Debug.Log("你輸了，嫩");
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if(!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f; //暫停遊戲來選擇你的升級
                    Debug.Log("選擇你的大GG");
                    levelUpScreen.SetActive(true);
                }
                break;

            default:
            Debug.LogWarning("沒有狀態啦");
            break;
        }
    }

    
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        //生成浮動數字
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        if(textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);
        
        //在持續時間結束後才消失
        Destroy(textObj, duration);
        //把生成文字物件添加至畫布的parent屬性
        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while(t < duration)
        {              
            //如果rect object不存在就終止
            if(!rect) break;

            //文字淡出，調整文字Alph值
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            //如果目標存在，存取目標位置
            if(target)
                lastKnownPosition = target.position;

            //讓傷害文字向上漂浮
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));

            //等待
            yield return w;
            t += Time.deltaTime;
        }


    }


    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        //如果沒有設定畫布就不會生成任何浮動數字
        if(!instance.damageTextCanvas) return;
        //找畫面上的位置
        if(!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
            ));

    }


    //用來定義變換遊戲狀態的方法
    public void ChangeState(GameState newstate)
    {
        currentState = newstate;
    }

    public void PasueGame()
    {
        if(currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // THE WORLD！！！
            pauseScreen.SetActive(true);
            Debug.Log("時間暫停了");
        }
    }
    public void ResumeGame()
    {
        if(currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            Debug.Log("時間再度流動");
        }
    }

    //定義變換恢復與暫停遊戲的方法
    void CheckForPauseAndResume()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PasueGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<PlayerInventory.Slot> chosenWeaponsData, List<PlayerInventory.Slot> chosenPassiveItemsData)
    {
        if(chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("已選武器以及道具的長度不符");
            return;
        }

        //用來把已選武器加入結算畫面
        for(int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            //檢查已選武器對應的圖片元素是否為null
            if(chosenWeaponsData[i].image.sprite)
            {
                //一個蘿蔔一個坑
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].image.sprite;

            }
            else
            {
                //沒有蘿蔔沒有坑
                chosenWeaponsUI[i].enabled = false;
            }
        }
        //用來把已選道具加入結算畫面
        for(int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            //檢查已選道具對應的圖片元素是否為null
            if(chosenPassiveItemsData[i].image.sprite)
            {
                //一個蘿蔔一個坑
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].image.sprite;

            }
            else
            {
                //沒有蘿蔔沒有坑
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchDisplay();

        if(stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateStopwatchDisplay()
    {
        //反正就是算分秒
        int minutes = Mathf.FloorToInt(stopwatchTime/60);
        int seconds = Mathf.FloorToInt(stopwatchTime%60);

        //顯示已經過時間
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gamplay);

    }
}
