using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOver = false;
    private TMP_Text text;
    private Image arrowLeft;
    private Image arrowRight;
    public Sprite[] arrowImages = new Sprite[2];

    void Awake()
    {
        text = transform.GetChild(0).GetComponent<TMP_Text>();
        arrowLeft = transform.GetChild(1).GetComponent<Image>();
        arrowRight = transform.GetChild(2).GetComponent<Image>();
    }

    private void OnDisable()
    {
        mouseOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseOver)
        {
            text.color = new Color(5f / 255, 199f / 255, 242f / 255, 200f / 255);
            arrowLeft.sprite = arrowImages[1];
            arrowRight.sprite = arrowImages[1];
        }
        else
        {
            text.color = new Color(1, 1, 1, 200f / 255);
            arrowLeft.sprite = arrowImages[0];
            arrowRight.sprite = arrowImages[0];
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
}
