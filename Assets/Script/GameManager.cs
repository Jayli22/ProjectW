using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private DDASystem ddaSystem;
    private EnemyGenerator enemyGenerator;
    private LevelController levelController;
    private Player player;
    public GameObject playerPrefeb;
    public int brachTag;

    private static GameManager instance;
    public static GameManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

 



    private void Awake()
    {      
        ddaSystem = GetComponent<DDASystem>();
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        player = Player.MyInstance;
        levelController = GetComponent<LevelController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(GameObject.Find("CameraBound") == null)
        {
            if (player != null)
            {
                player.gameObject.SetActive(false);
                levelController.enabled = false;
            }
        }
        else
        {
            if (player == null)
            {
                GameObject g = Instantiate(playerPrefeb);
                g.SetActive(true);
                player = Player.MyInstance;
                levelController.enabled = true;
                levelController.player = player;
                levelController.EnterNewLevel();
            }
            else if(player.isActiveAndEnabled == false )
            {
                player.gameObject.SetActive(true);
                levelController.EnterNewLevel();
                levelController.enabled = true;

            }
        }
       
        
    }



    /// <summary> 
    /// 进入新关卡，调用levelcontroller
    /// 
    /// </summary>
    public void EnterNewLevel(string sceneName)
    {
        if(player == null)
        {
            LevelSwitch(1);
            LoadingSceneScript.gSceneName = sceneName;
        }
        else
        {
           
            LevelSwitch(1);
            LoadingSceneScript.gSceneName = sceneName;
        }

       //levelController.GenerateNewLevel();
    }


    /// <summary>
    /// 切换关卡场景
    /// </summary>
    public void LevelSwitch(int sceneID)
    {
        LevelChanger levelChanger = FindObjectOfType<LevelChanger>();
        levelChanger.FadeToLevel(1);
        LoadingSceneScript.gSceneNumber = sceneID;
    }

    public void LevelSwitch(string sceneName)
    {
        LevelChanger levelChanger = FindObjectOfType<LevelChanger>();
        levelChanger.FadeToLevel(1);
        //LoadingSceneScript.gSceneName = "Wuyan_guochang1";
    }
}
