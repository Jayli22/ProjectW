using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer : MonoBehaviour
{
    private bool finished;
    private Timer durationTimer;
    private float duration;

    public bool Finished { get => finished; set => finished = value; }
    public float Duration { get => duration; set => duration = value; }

    void Start()
    {
        durationTimer = gameObject.AddComponent<Timer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(durationTimer.Finished)
        {
            Inactive();
        }
    }

    public void Active(float t)
    {
        Player.MyInstance.moveSpeed = 1.5f;
        durationTimer.Duration = t;
        durationTimer.Run();
    }

    public void Inactive()
    {
        Player.MyInstance.moveSpeed = 3;

    }

}
