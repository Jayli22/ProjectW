using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private bool levelClear;
    private List<Collider2D> checkList;
    public Player player;
    private GameManager gameManagement;
    public GameObject[] enemyPrefabs;

    private string[] wuyanCutScenes = { "Wuyan_guochang1","Wuyan_guochang2","Wuyan_guochang3","Wuyan_guochang4","Wuyan_guochang5","Wuyan_guochang6","Wuyan_guochang7"};
    private string[] LanqueCutScenes = { "lanque_guochang1", "lanque_guochang2", "lanque_guochang3", "lanque_guochang4", "lanque_guochang5", "lanque_guochang6" };
    private int cutSceneTag;

    private List<GameObject> enemyList;
    private DDASystem dDASystem;
    private bool ddaEnable;
    private GameObject guideArrow;
    private float tangle;
    //单体高强度怪物
    public Dictionary<string, int> eliteEnemy;
    //群体出没怪物
    public Dictionary<string, int> gregariousEnemy;
    //普通填充怪物
    public Dictionary<string, int> normalEnemy;

    /// <summary>
    /// 当前生成怪物列表
    /// </summary>
    public List<GameObject> EnemyList { get => enemyList; set => enemyList = value; }
    public bool DDAEnable { get => ddaEnable; set => ddaEnable = value; }

    private void Awake()
    {

        GenerateEnemyDictionary();

    }
    // Start is called before the first frame update
    void Start()
    {
        checkList = new List<Collider2D>();
        gameManagement = GameManager.MyInstance;
        cutSceneTag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            player = Player.MyInstance;
        }

        if (guideArrow == null)
        {
            guideArrow = player.guideArrow;
        }

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            levelClear = true;
        }
        else
        {
            levelClear = false;
        }

        if (levelClear)
        {
            CheckExit();
            ArrowGuide();
        }
        else
        {
             guideArrow.SetActive(false);
        }
        //if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    foreach(GameObject g in GameObject.FindGameObjectsWithTag("Enemy"))
        //    {
        //        Destroy(g);
        //    }
        //}
        //if( Input.GetKeyDown(KeyCode.Delete))
        //{
        //    player.currentHp = player.maxHp;
        //}
    }

    private void CheckExit()
    {
        ContactFilter2D c = new ContactFilter2D();
        c.useTriggers = true;
        Physics2D.OverlapCollider(GameObject.Find("ExitPoint").GetComponent<Collider2D>(), c, checkList);
        foreach (Collider2D obj in checkList)
        {
            if (obj.tag == "Player")
            {
                SwitchLevel();
                GetComponent<LevelController>().enabled = false;
            }
        }
    } 

    private void ArrowGuide()
    {
        float angle = ToolsHub.GetAngleBetweenVectors(new Vector2(0, 1), ((Vector3)GameObject.Find("ExitPoint").transform.position - player.transform.position).normalized);
        //guideArrow.transform.position = player.transform.position;
        guideArrow.transform.RotateAround(player.transform.position, Vector3.forward, angle - tangle);
        tangle = angle;
        guideArrow.SetActive(true);

    }
    /// <summary>
    /// Testshiyong
    /// </summary>
    public void SwitchLevel()
    {
        if(SceneManager.GetActiveScene().name == "LanqueBoss")
        {
            gameManagement.EnterNewLevel(LanqueCutScenes[cutSceneTag]);
            DDAEnable = false;
            cutSceneTag++;

        }

        else if(player.levelTag % 3 == 0)
        {
            if(gameManagement.brachTag == 1)
            {
                if (cutSceneTag == 2)
                    cutSceneTag = 3;
                gameManagement.EnterNewLevel(wuyanCutScenes[cutSceneTag]);
                DDAEnable = true;
                

            }
            else
            {
                gameManagement.EnterNewLevel(LanqueCutScenes[cutSceneTag]);
                DDAEnable = false;

            }
            cutSceneTag++;
        }
        else
        {
            int i = Random.Range(1, 5);
            gameManagement.EnterNewLevel("forest" + i);
        }
    }


    /// <summary>
    /// 生成新关卡
    /// </summary>
    public void EnterNewLevel()
    {
        if(levelClear)
        {
            player.levelTag++;
        }

        levelClear = false;
        GenerateLevel();
        ResetPlayerLocation();
    }


    private void ResetPlayerLocation()
    {
        Vector2 pos = new Vector2();
        if (SceneManager.GetActiveScene().name != "StartScene" && SceneManager.GetActiveScene().name != "LoadingScene")
            pos = GameObject.Find("EnterPoint").transform.position;
        if(pos!=null && player != null)
           player.transform.position = pos;
    }
    /// <summary>
    /// 为关卡生成怪物（随机）
    /// </summary>
    public void GenerateLevel()
    {
        if(dDASystem == null)
        {
            dDASystem = gameObject.AddComponent<DDASystem>();
        }
        GenerateEnemyList();
        DDAFunction();

        foreach(GameObject g in EnemyList)
        {
            GenerateEnemy(g);
        }

        
    }


    /// <summary>
    /// 生成当前关卡怪物列表，后续进行DDA判断
    /// </summary>
    public void GenerateEnemyList()
    {
        enemyList = new List<GameObject>();
        //if(player.levelTag < 10)
        //{
        Debug.Log(player.levelTag);
            for(int i = 0; i < 1 + player.levelTag/4; i++)
            {
                int r = Random.Range(0, normalEnemy.Count);
                enemyList.Add(enemyPrefabs[r]);
            }
            EnemyList.Sort(delegate (GameObject g1, GameObject g2) // 按照怪物强度对怪物进行排序
            {
                return g1.GetComponent<Enemy>().strengthValue.CompareTo(g2.GetComponent<Enemy>().strengthValue);
            });
       // }
    }

    /// <summary>
    /// 进行DDA判断,
    /// </summary>
    public void DDAFunction()
    {
        if (DDAEnable)
        {
            for (int i = 0; i < EnemyList.Count; i++)
            {
                Debug.Log(EnemyList[i].name);

            }
            int f = dDASystem.FactorCompare(EnemyList);  //LF和PF对比
            Debug.Log("差值为" + f);

            int levelTag = player.levelTag;

            //if (levelTag < 10)   
            //{
            if (f > 15)// 对比差值判断部分，根据差值进行后续操作
            {
                EnemyList.RemoveAt(0);
                DDAFunction();
            }
            else if (f < -15)
            {
                EnemyList.Add(enemyPrefabs[Random.Range(1, 3)]);
                DDAFunction();

            }
        }
    }

    /// <summary>
    /// 修正怪物列表
    /// </summary>
    public void RegenerateEnemyList()
    {

    }

    public void EliteEnemyProb()
    {

    }

    /// <summary>
    //在地图随机范围内随机生成一个敌人对象
    /// </summary>
    public void GenerateEnemy(GameObject g)
    {
        Vector2 pos;
        pos.x = Random.Range(-4.0f, 4.0f);
        pos.y = Random.Range(-4.0f, 4.0f);
        GameObject newEnemy = Instantiate(g, pos, Quaternion.identity);

    }
    /// <summary>
    //在指定位置生成一个敌人对象(-4.5,4.5)
    /// </summary>
    public void GenerateEnemy(GameObject g, Vector2 pos)
    {
        GameObject newEnemy = Instantiate(g, pos, Quaternion.identity);
    }




    bool GenerateProbability(float p)
    {
        bool b = false;
        if (Random.Range(0f, 1f) > p)
        {
            b = false;
        }
        else
        {
            b = true;
        }
        return b;
    }


    /// <summary>
    /// 将所有敌人怪物信息读取入字典方便之后生成使用
    /// </summary>
    private void GenerateEnemyDictionary()
    {
        normalEnemy = new Dictionary<string, int>();
        gregariousEnemy = new Dictionary<string, int>();
        eliteEnemy = new Dictionary<string, int>();

        foreach (GameObject g in enemyPrefabs)
        {
            Enemy gs = g.GetComponent<Enemy>();
            switch(gs.type)
            {
                case 1:
                    normalEnemy.Add(gs.name, gs.strengthValue);
                    break;
                case 2:
                    gregariousEnemy.Add(gs.name, gs.strengthValue);
                    break;
                case 3:
                    eliteEnemy.Add(gs.name, gs.strengthValue);
                    break;
                default:
                    normalEnemy.Add(gs.name, gs.strengthValue);
                    break;
            }


        }
    }
}
