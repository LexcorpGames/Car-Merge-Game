using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private MiniatureBoard _miniatureBoard;

    public void TryBuy()
    {
        _miniatureBoard.LoadMiniature(1);
    }
}
