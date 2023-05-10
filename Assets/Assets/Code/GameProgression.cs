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
    [SerializeField] private LeaderBoard _leaderBoard;
    [SerializeField] private MiniatureBoard _miniatureBoard;
    [SerializeField] private CarLoader_Player _carLoaderPlayer;
    [SerializeField] private CarLoader_Rival _carLoaderRival;
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

    private bool _boostActive;

    private bool _rivalActive;
    private float _rivalOvertakeTime_Current;
    private float _rivalOvertakeTime_Total;
    private int _rivalIndex;

    private void Start()
    {
        _carLoaderPlayer.OnNewCarLoaded += OnStartDrivingEvent;

        _desiredFOV = _boostCamFOV.x;
    }
    
    private void LateUpdate()
    {
        //FINAL MPH
        Current_MPH = Mathf.Lerp(Current_MPH, _desiredSpeed, Time.deltaTime);
        //FINAL DISTANCE
        Current_Distance += (Current_MPH / 3600.0f) * Time.deltaTime;

        //BOOST FOV
        if (_manageFOV)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _desiredFOV, Time.deltaTime * _boostTransition);
        }

        if(_rivalActive)
        {
            float boostMod = 1f;
            if (_boostActive) boostMod = 1.5f;
            _rivalOvertakeTime_Current += Time.deltaTime * boostMod;
            float rivalProgress = _rivalOvertakeTime_Current / _leaderBoard.BoardMemebers[_rivalIndex].OvertakeTime;
            _carLoaderRival.UpdateRivalPosition(rivalProgress);

            if (rivalProgress >= 0.125f)
            {
                _leaderBoard.BoardMemebers[_rivalIndex].IsBehind = true;
            }

            if (rivalProgress >= 1f)
            {
                _rivalActive = false;
                _carLoaderRival.LoadedCar.PoolBack();
                _rivalIndex++;
            }
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
        _boostActive = activeBoost;
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
        InitiateNextRival();
    }

    private void InitiateNextRival()
    {
        int carLevel = _carLoaderPlayer.LoadedCar.Level;
        var rivalDesc = _carLoaderRival.LoadCar(_leaderBoard.BoardMemebers[_rivalIndex].CarPrefab.Level);
        if(rivalDesc != null)
        {
            _rivalActive = true;
            _rivalOvertakeTime_Current = 0f;
            //_rivalOvertakeTime_Total = 
        }
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
