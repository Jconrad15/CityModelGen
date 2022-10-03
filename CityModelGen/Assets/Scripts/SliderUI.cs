using ModularMotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderUI : MonoBehaviour
{
    [SerializeField]
    private GameObject sliderArea;

    private UIMotion uiMotion;

    private void Start()
    {
        uiMotion = sliderArea.GetComponent<UIMotion>();
    }

    public void Toggle()
    {
        uiMotion.Play();
    }

}
