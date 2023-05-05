using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public float Current_MPH = 45f;

    [SerializeField] private LoopingWorld _loopingWorld;
    
    void Start()
    {
        
    }
    
    private void Update()
    {
        _loopingWorld.UpdateSpeed(Current_MPH);
    }
}
