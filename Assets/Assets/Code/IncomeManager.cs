using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    [System.Serializable]
    private class DistReward
    {
        public float TimeDelay;
        public float Reward;
    }

    [SerializeField] private float _currentMoney;
    [Header("Setup")]
    [SerializeField] private MiniatureBoard _miniatureBoard;
    [SerializeField] private GameProgression _gameProgression;
    [SerializeField] private NumberAnimator _numberAnimator;
    [SerializeField] private TextMeshProUGUI _costText;

    [Header("Income")]
    [SerializeField] private float _startMoney = 0;
    [SerializeField] private float _gainInterval = 0.33f;
    [SerializeField] private float[] _incomePerMile;
    [SerializeField] private float[] _incomeMultiplier;

    [Header("Miniature Cost")]
    [SerializeField] private float[] _costPerBuy;

    [Header("Checkpoints")]
    [SerializeField] private DistReward _rewardPerCheckpoint;

    private float _updateIntervalLeft;
    private int _multiplierIndex;
    private int _costButtonIndex;
    private float _currentCheckPointTime;

    public void TryBuy()
    {
        float buyCost = _costPerBuy[_costButtonIndex];

        if (buyCost <= _currentMoney)
        {
            _currentMoney -= buyCost;

            _miniatureBoard.LoadMiniature(1);

            _costButtonIndex++;
            _costButtonIndex = Mathf.Clamp(_costButtonIndex, 0, _costPerBuy.Length);

            _costText.text = _costPerBuy[_costButtonIndex].ToString();
        }
    }

    public void Update()
    {
        //CASH TICK
        if(_updateIntervalLeft >= 0)
        {
            _updateIntervalLeft -= Time.deltaTime;

            //UPDATE TICK
            if(_updateIntervalLeft < 0f)
            {
                _updateIntervalLeft = _gainInterval;

                OnUpdateTick();
            }
        }

        //CHECKPOINT TICK
        if(_gameProgression.Current_MPH > 10f)
        {
            _currentCheckPointTime += Time.deltaTime;

            if(_currentCheckPointTime >= _rewardPerCheckpoint.TimeDelay)
            {
                _currentCheckPointTime = 0f;

                CheckpointReached();
            }
        }
    }

    private void OnUpdateTick()
    {
        int carLevel = _miniatureBoard.CurrentCarLevel;

        _currentMoney += _gameProgression.Current_MPH * (_incomePerMile[carLevel] / 60f) * _incomeMultiplier[_multiplierIndex];

        _numberAnimator.SetTargetValue(_currentMoney);
    }

    private void CheckpointReached()
    {
        Debug.Log("REWARD: +" + _rewardPerCheckpoint.Reward + " Cash!");
    }
}
