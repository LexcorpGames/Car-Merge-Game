using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CarLoader_Rival : MonoBehaviour
{
    [Header("RIVALS")]
    [SerializeField] private Transform _carPosition;
    [SerializeField] private Transform _positionStart;
    [SerializeField] private Transform _positionEnd;
    [Range(0f, 1f)]
    [SerializeField] private float _overtakeProgress;
    [SerializeField] private List<CarDesciption> _carPrefabs;

    private Transform _poolNode;
    [Header("Debug")]
    [SerializeField] private CarDesciption _loadedCar;
    [Header("Creatives")]
    [SerializeField] private float lastCarDistanceOffset = 160f;
    [SerializeField] private float firstCarDistanceOffset = 160f;
    [SerializeField] private List<float> carSpacing;


    private CarDesciption _editorLoadedCar;

    public float OvertakeProgress { get => _overtakeProgress; set => _overtakeProgress = value; }
    public CarDesciption LoadedCar { get => _loadedCar; set => _loadedCar = value; }

    bool allVisible = false;
    private List<CarDesciption> rivals;

    private void Awake()
    {
        GameObject newNode = new GameObject("CarPool");
        newNode.SetActive(false);
        newNode.transform.SetParent(_carPosition);
        newNode.transform.localPosition = Vector3.zero;
        newNode.transform.localEulerAngles = Vector3.zero;
        _poolNode = newNode.transform;

        foreach (var prefab in _carPrefabs)
        {
            var newCarGO = Instantiate(prefab, _poolNode);
            newCarGO.name = prefab.name;
            newCarGO.transform.localPosition = Vector3.zero;
            newCarGO.transform.localEulerAngles = Vector3.zero;
            CarDesciption desc = newCarGO.GetComponent<CarDesciption>();
            desc.SetPoolParent(_poolNode);
            //newCarGO.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateCarPos();
    }

    public CarDesciption LoadCar(int level)
    {
        //Remove prev car
        if (_loadedCar != null)
        {
            var childGO = _loadedCar.transform;
            childGO.parent = _poolNode;
        }

        for (int i = 0; i < _poolNode.childCount; i++)
        {
            var carGO = _poolNode.GetChild(i);
            CarDesciption desc = carGO.GetComponent<CarDesciption>();
            if(desc.Level == level)
            {
                desc.IsMiniature = false;

                carGO.SetParent(_carPosition);
                _loadedCar = desc;
                desc.ActivateRoadModel();
                return _loadedCar;
            }
        }

        return null;
    }

    public CarDesciption LoadAllCars(int level)
    {
        if (!allVisible)
        {
            allVisible = true;
            rivals = new List<CarDesciption>();
            while(_poolNode.childCount != 0)
            {
                var carGO = _poolNode.GetChild(0);
                CarDesciption desc = carGO.GetComponent<CarDesciption>();
                desc.IsMiniature = false;
                carGO.SetParent(_carPosition);
                desc.ActivateRoadModel();
                if (desc.Level == level)
                {
                    _loadedCar = desc;
                }
                else 
                {
                    rivals.Add(desc);
                }
            }
        }
        return _loadedCar;
    }

    public void UpdateRivalPosition(float progress)
    {
        if (_loadedCar == null) return;

        _overtakeProgress = progress;
    }

    public void UpdateCarPos()
    {
        if(_loadedCar != null)
        {
            if (allVisible)
            {
                var dir = -Vector3.forward;
                _loadedCar.transform.position = Vector3.Lerp(_positionStart.position - lastCarDistanceOffset * dir, _positionEnd.position - firstCarDistanceOffset * dir, _overtakeProgress);
                for (int i = 0; i < rivals.Count; i++)
                {
                    rivals[i].transform.position = _loadedCar.transform.position + carSpacing[i] * (i+1)* dir;
                }
            }
            else
            {
                _loadedCar.transform.position = Vector3.Lerp(_positionStart.position, _positionEnd.position, _overtakeProgress);
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CarLoader_Rival))]
public class CarLoader_Rival_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(Application.isPlaying)
        {
            return;
        }

        CarLoader_Rival myTarget = (CarLoader_Rival)target;

        if (myTarget == null) return;

        myTarget.UpdateCarPos();
        GUILayout.Space(20f);
    }
}

#endif
