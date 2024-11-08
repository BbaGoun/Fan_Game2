using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOver = false;
    private Image back;
    //public Sprite normal;
    //public Sprite hover;

    void Awake()
    {
        back = transform.GetChild(0).GetComponent<Image>();
    }

    void OnDisable()
    {
        mouseOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseOver)
        {
            back.color = new Color(0f, 0f, 0f, 0.9f);
            //back.sprite = hover;
        }
        else
        {
            back.color = new Color(0f, 0f, 0f, 1f);
            //back.sprite = normal;
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
