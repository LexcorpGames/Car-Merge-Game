using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public float Current_MPH = 45f;

    [SerializeField] private LoopingWorld _loopingWorld;
    [SerializeField] private CarLoader_Player _carLoaderPlayer;

    private float _currentSpeed;
    private float _desiredSpeed;

    private void Start()
    {
        _carLoaderPlayer.OnNewCarLoaded += StartDriving;
    }
    
    private void Update()
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, _desiredSpeed, Time.deltaTime);

        _loopingWorld.UpdateSpeed(_currentSpeed);
    }

    public void StartDriving()
    {
        _desiredSpeed = Current_MPH
            ;
    }

    public void StopDriving()
    {
        _desiredSpeed = 0f;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(GameProgression))]
public class GameProgression_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameProgression myTarget = (GameProgression)target;

        if (myTarget == null) return;

        GUILayout.Space(20f);

        if (GUILayout.Button("Start Moving", GUILayout.Height(35f)))
        {
            myTarget.StartDriving();
        }

        GUILayout.Space(20f);

        if (GUILayout.Button("Stop Moving", GUILayout.Height(35f)))
        {
            myTarget.StopDriving();
        }
    }
}

#endif
