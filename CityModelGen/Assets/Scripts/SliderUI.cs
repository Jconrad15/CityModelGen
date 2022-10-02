using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderUI : MonoBehaviour
{
    [SerializeField]
    private GameObject sliderArea;

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        sliderArea.SetActive(false);
    }

    private void Show()
    {
        sliderArea.SetActive(true);
    }

    public void Toggle()
    {
        if (sliderArea.activeSelf == true)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

}
