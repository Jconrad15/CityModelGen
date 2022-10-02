using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private MainController mainController;
    private void Start()
    {
        mainController = FindObjectOfType<MainController>();
    }

    public void SeedChanged(float seed)
    {
        mainController.SeedChanged((int)seed);
    }

    public void XResolutionChanged(float xResolution)
    {
        mainController.XResolutionChanged((int)xResolution);
    }

    public void ZResolutionChanged(float zResolution)
    {
        mainController.ZResolutionChanged((int)zResolution);
    }

    public void WaterHeightChanged(float waterHeight)
    {
        mainController.WaterHeightChanged(waterHeight);
    }

    public void MaxHeightChanged(float maxHeight)
    {
        mainController.MaxHeightChanged(maxHeight);
    }

    public void NoiseScaleChanged(float noiseScale)
    {
        mainController.NoiseScaleChanged(noiseScale);
    }

    public void HeightRandomizationFactorChanged(
        float heightRandomizationFactor)
    {
        mainController.HeightRandomizationFactorChanged(
            heightRandomizationFactor);
    }

}
