using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color mousePressedColor;
    public Color mouseOnColor;
    public Color mouseOffColor;
    public bool mouseOver = false;
    public bool mousePressed = false;
    private TMP_Text text;
    private Button button;

    void Awake()
    {
        text = transform.GetChild(0).GetComponent<TMP_Text>();
        button = GetComponent<Button>();
    }

    private void OnDisable()
    {
        mouseOver = false;
        mousePressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (button.interactable)
        {
            if (mousePressed)
            {
                text.color = mousePressedColor;
            }
            else if (mouseOver)
            {
                text.color = mouseOnColor;
            }
            else
            {
                text.color = mouseOffColor;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mousePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mousePressed = false;
    }
}
