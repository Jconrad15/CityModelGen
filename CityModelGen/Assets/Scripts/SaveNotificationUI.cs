using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveNotificationUI : MonoBehaviour
{
    [SerializeField]
    private GameObject saveNotificationArea;

    private void Start()
    {
        Hide();

        // Register to saving events
        MainController mc = FindObjectOfType<MainController>();
        mc.RegisterSaveStart(Show);
        mc.RegisterSaveEnd(Hide);
    }

    private void Hide()
    {
        saveNotificationArea.SetActive(false);
    }

    private void Show()
    {
        saveNotificationArea.SetActive(true);
    }

}
