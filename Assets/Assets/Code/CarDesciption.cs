using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarDesciption : MonoBehaviour
{
    [Header("General")]
    public int Level = 1;

    [Header("Miniatures")]
    [SerializeField] private bool _isMiniature;
    [SerializeField] private GameObject _levelNumber;
    [SerializeField] private TextMeshPro _numberText;

    private Transform _poolNode;

    public bool IsMiniature
    {
        get { return _isMiniature; }
        set
        {
            _numberText.text = Level.ToString();
            _levelNumber.SetActive(value);
            _isMiniature = value;
        }
    }

    public void SetPoolParent(Transform poolNode)
    {
        _poolNode = poolNode;
    }

    public void PoolBack()
    {
        transform.SetParent(_poolNode);
    }
}
