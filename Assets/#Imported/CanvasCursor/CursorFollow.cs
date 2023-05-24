using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CursorFollow : MonoBehaviour
{
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Vector2 size;
    [SerializeField] private bool isActive = true;
    [SerializeField] private bool hasOutline = true;
    [SerializeField] private bool animateClick = true;

    [SerializeField] private RectTransform cursor;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector3 clickDown = Vector3.one * 0.5f;
    [SerializeField] private Vector3 clickUp = Vector3.one;

    private UnityAction AnimateClick;

    private void Start()
    {
        cursor.sizeDelta = size;
        cursor.GetComponent<Outline>().enabled = hasOutline;
        cursor.GetComponent<Image>().sprite = cursorSprite;
        if (animateClick) 
        {
            AnimateClick += OnClickFX;
        }
    }


    private void Update()
    {
        if (isActive) 
        {
            cursor.position = Input.mousePosition;
            AnimateClick?.Invoke();
        }
    }

    private void OnClickFX() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Tween.LocalScale(cursor, clickDown, 0.1f, 0f, curve);
            Tween.LocalScale(cursor, clickUp, 0.1f, 0.1f, curve);
        }
    }

    private void OnDestroy()
    {
        AnimateClick = null;
    }
}
