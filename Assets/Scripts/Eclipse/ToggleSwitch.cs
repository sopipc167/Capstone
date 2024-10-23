using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ToggleSwitch : MonoBehaviour
{
    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)] private float sliderValue;
    public bool CurrentValue { get; private set; }

    private Slider _slider;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
    [SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0,0,1,1);

    private Coroutine _animateSliderCoroutine;

    [Header("Events")]
    [SerializeField] private UnityEvent onToggleOn;
    [SerializeField] private UnityEvent onToggleOff;

    private ToggleSwitch _toggleSwitch;

    protected void OnValidate()
    {
        SetupToggleComponents();
        _slider.value = sliderValue;
    }

    private void SetupToggleComponents()
    {
        if(_slider != null)
            return;
        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if(_slider == null)
        {
            Debug.Log("No slider found!", this);
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    public void SetupForManager(ToggleGroup toggleGroup)
    {
        
    }
    
}
