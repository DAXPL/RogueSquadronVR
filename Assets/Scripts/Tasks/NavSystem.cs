using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavSystem : Serviceable
{
    [System.Serializable]
    private struct NavigationSlider
    {
        public Slider slider;
        public float weight;
        public float targetValue;
    }

    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private NavigationSlider[] sliders;
    private int percentage = 0;
    private float tolerance = 0.15f;

    private void Start()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].slider.value = UnityEngine.Random.Range(0.0f, 1.0f);
            sliders[i].slider.interactable = false;
        }
        percentageText.SetText($"100%");
        percentageText.color = Color.green;
    }

    public void SetDamageState()
    {
        base.Damage();
        for (int i=0; i<sliders.Length;i++)
        {
            sliders[i].weight = UnityEngine.Random.Range(1, 11);
            sliders[i].slider.value = UnityEngine.Random.Range(0.0f, 1.0f);
            sliders[i].targetValue = 0.05f * UnityEngine.Random.Range(2, 19);
            sliders[i].slider.interactable = true;
        }
        percentageText.color = Color.red;
    }

    public void ValidatePercentage(Single s)
    {
        float totalWeight = 0;
        float totalDeviation = 0;

        foreach (NavigationSlider ns in sliders)
        {
            totalWeight += ns.weight;
            float deviation = Mathf.Abs(ns.slider.value - ns.targetValue);  //Odchylenie od docelowej wartoœci
            if (deviation <= tolerance)
            {
                deviation = 0;
            }
            else
            {
                // Odejmujemy tolerancjê od odchylenia, aby zmniejszyæ jego wp³yw
                deviation -= tolerance;
            }

            totalDeviation += deviation * ns.weight;  // Wa¿one odchylenie
        }

        //Procent kalibracji jako 100% minus œrednie wa¿one odchylenie
        percentage = (int)Mathf.Clamp((1 - (totalDeviation / totalWeight)) * 100, 0, 100);

        if (percentageText != null)
        {
            percentageText.SetText($"{percentage}%");
            percentageText.color = percentage == 100 ? Color.green : Color.red;
        }

        if (percentage == 100)
        {
            Fix();
            foreach (NavigationSlider ns in sliders)
            { 
                ns.slider.interactable = false;
            }
        }
    }
}
