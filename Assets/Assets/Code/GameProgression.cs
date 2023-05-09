using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public float Current_MPH = 45f;
    public float Current_Distance = 0f;

    [Header("Setup")]
    [SerializeField] private LoopingWorld _loopingWorld;
    [SerializeField] private MiniatureBoard _miniatureBoard;
    [SerializeField] private CarLoader_Player _carLoaderPlayer;
    [SerializeField] private TextMeshProUGUI _textMesh;

    [Header("Boost")]
    [SerializeField] private bool _manageFOV;
    [SerializeField] private Vector2 _boostCamFOV;
    [SerializeField] private float _boostTransition = 5f;

    [Header("Config")]
    [SerializeField] private float[] _startSpeed;

    private float _currentSpeed;
    private float _desiredSpeed;
    private float _desiredFOV;

    private void Start()
    {
        _carLoaderPlayer.OnNewCarLoaded += OnStartDrivingEvent;

        _desiredFOV = _boostCamFOV.x;
    }
    
    private void LateUpdate()
    {
        Current_MPH = Mathf.Lerp(Current_MPH, _desiredSpeed, Time.deltaTime);

        Current_Distance += (Current_MPH / 3600.0f) * Time.deltaTime;

        if (_manageFOV)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _desiredFOV, Time.deltaTime * _boostTransition);
        }

        _loopingWorld.UpdateSpeed(Current_MPH);

        _textMesh.text = Current_MPH.ToString("F0");

        //BOOST
        if (_carLoaderPlayer.LoadedCar != null)
        {
            if (_miniatureBoard.DraggedCar == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartDriving(true);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                StartDriving(false);
            }
        }
    }

    public void StartDriving(bool activeBoost = false)
    {
        int carLevel = _carLoaderPlayer.LoadedCar.Level;
        if (activeBoost)
        {
            float currentLevelSpeed = _startSpeed[carLevel];
            float nextLevelSpeed = ((_startSpeed.Length - 1) == carLevel) ? _startSpeed[carLevel] * 1.5f : _startSpeed[carLevel + 1];
            float speedDifference = nextLevelSpeed - currentLevelSpeed;
            _desiredSpeed = currentLevelSpeed + speedDifference * 0.5f;
            _desiredFOV = _boostCamFOV.y;
        }
        else
        {
            _desiredSpeed = _startSpeed[carLevel];
            _desiredFOV = _boostCamFOV.x;
        }
    }

    public void StopDriving()
    {
        _desiredSpeed = 0f;
    }

    private void OnStartDrivingEvent()
    {
        StartDriving(false);
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

        if (GUILayout.Button("Apply speed", GUILayout.Height(35f)))
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
