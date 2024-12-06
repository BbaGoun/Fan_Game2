using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTextImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color mousePressedColor;
    public Color mouseOnColor;
    public Color mouseOffColor;
    private bool mouseOver = false;
    private bool mousePressed = false;
    private Image image;
    private Button button;

    void Awake()
    {
        image = GetComponent<Image>();
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
                image.color = mousePressedColor;
            }
            else if (mouseOver)
            {
                image.color = mouseOnColor;
            }
            else
            {
                image.color = mouseOffColor;
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
