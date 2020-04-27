using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartialSkillActiveGroup : MonoBehaviour
{
    public int id;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClick(string name)
    {
        switch (name)
        {
            case "1":
                Player.MyInstance.SkillSwitch(1, id);
                break;
            case "2":
                Player.MyInstance.SkillSwitch(2, id);
                break;
            case "3":
                Player.MyInstance.SkillSwitch(3, id);

                break;
            case "4":
                Player.MyInstance.SkillSwitch(0, id);

                break;
        }
    }
}
