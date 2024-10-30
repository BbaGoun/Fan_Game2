using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DropdownMain : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOver = false;
    private TMP_Dropdown dropdown;
    private Image dropdownImage;
    private TMP_Text text;
    private TMP_Text DLText;
    private Image arrow;
    public Sprite[] arrowImages = new Sprite[2];
    

    public void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdownImage = GetComponent<Image>();
        text = transform.Find("Label").GetComponent<TMP_Text>();
        arrow = transform.Find("Arrow").GetComponent<Image>();
    }

    void Update()
    {
        if (mouseOver)
        {
            dropdownImage.color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            dropdownImage.color = new Color(1, 1, 1, 1f);
        }

        if(transform.childCount == 3) // isClosed
        {
            text.color = new Color(1, 1, 1, 200f / 255);
            arrow.sprite = arrowImages[0];
        }
        else // isOpened
        {
            text.color = new Color(5f / 255, 199f / 255, 242f / 255, 200f / 255);
            arrow.sprite = arrowImages[1];

            DLText = transform.GetChild(3).GetChild(0).GetChild(0).GetChild(1 + dropdown.value).GetChild(1).GetComponent<TMP_Text>();
            DLText.color = new Color(5f / 255, 199f / 255, 242f / 255, 200f / 255);
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
