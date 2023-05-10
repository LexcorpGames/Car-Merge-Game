using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    [System.Serializable]
    public class BoardMemeber
    {
        public string Name;
        public CarDesciption CarPrefab;
        public float OvertakeTime;
        public bool IsBehind;
    }

    [SerializeField] private List<BoardMemeber> _boardMemebers;

    public List<BoardMemeber> BoardMemebers
    {
        get => _boardMemebers;
        set => _boardMemebers = value;
    }
}
