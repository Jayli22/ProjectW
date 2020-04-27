using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    LevelChanger levelChanger;
    private static UIManager instance;
    public GameObject skillInventory;
    public static UIManager MyInstance {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    public GameObject deadHint;
    private GameManager gameManagement;
    public void OnClick(string name)
    {
        switch (name)
            {
            case "StartButton1":
                //levelChanger.FadeToLevel(Random.Range(1,5));
                gameManagement.EnterNewLevel("forest1");
                gameManagement.brachTag = 1;
                break;
            case "StartButton2":
                //levelChanger.FadeToLevel(Random.Range(1,5));
                gameManagement.EnterNewLevel("forest1");
                gameManagement.brachTag = 2;

                break;
            case "RestartButton":
                Destroy(Player.MyInstance.gameObject);
                levelChanger.FadeToLevel(0);
                break; 

    }
}

    // Start is called before the first frame update
    void Start()
    {
        levelChanger = FindObjectOfType<LevelChanger>();
        gameManagement = GameManager.MyInstance;
       // LogOutput.console.LogStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            skillInventory.SetActive(false);

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
           if(skillInventory.activeSelf)
            {
                skillInventory.SetActive(false);
            }
           else
            {
                skillInventory.SetActive(true);
            }
        }
    }
}
