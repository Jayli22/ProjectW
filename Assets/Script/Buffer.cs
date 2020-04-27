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
       
    }

    public void Active(float t)
    {
      
    }

    public void Inactive()
    {
        

    }

}
