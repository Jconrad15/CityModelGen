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

    public void ResolutionChanged(float resolution)
    {
        mainController.ResolutionChanged((int)resolution);
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

    public void VoronoiPerlinInfluenceChanged(
        float voronoiPerlinInfluence)
    {
        mainController.VoronoiPerlinInfluenceChanged(
            voronoiPerlinInfluence);
    }

    public void VoronoiRegionCountChanged(
        float voronoiRegionCount)
    {
        mainController.VoronoiRegionCountChanged(
            (int)voronoiRegionCount);
    }

    public void BuildingWidthChanged(
        float buildingWidth)
    {
        mainController.BuildingWidthChanged(
            buildingWidth);
    }

    public void EmptyLotPercentChanged(
        float emptyLotPercent)
    {
        mainController.EmptyLotPercentChanged(
            emptyLotPercent);
    }

    public void LoneIslandThresholdChanged(
        float loneIslandThreshold)
    {
        loneIslandThreshold = Mathf.Abs(loneIslandThreshold - 4);
        mainController.LoneIslandThresholdChanged(
            loneIslandThreshold);
    }

    public void UseRoadColorChanged(
        bool useRoadColor)
    {
        mainController.UseRoadColorChanged(
            useRoadColor);
    }
}
