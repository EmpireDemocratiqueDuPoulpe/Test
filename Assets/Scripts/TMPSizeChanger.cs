using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMPSizeChanger : MonoBehaviour
{
    [Range(1f, 10f)] public float scaleMultiplier = 1f;
    [Range(0f, 100f)] public float scalingSpeed = 0.2f;

    private TMP_Text _text;
    private float _defaultTextSize;
    private float _minScale = 1f;
    private float _currentScale;
    private bool _isScalingUp = true;
    
    private void Start()
    {
        _text = GetComponent<TMP_Text>();

        _defaultTextSize = _text.fontSize;
        _currentScale = _minScale;
    }
    
    private void Update()
    {
        // Check if the text will grow or shrink
        if (_currentScale < _minScale) _isScalingUp = true;
        else if (_currentScale > scaleMultiplier) _isScalingUp = false;

        // Get the next scale
        if (_isScalingUp)
        {
            _currentScale += scalingSpeed * Time.deltaTime;
        }
        else
        {
            _currentScale -= scalingSpeed * Time.deltaTime;
        }

        // Scale the text
        _text.fontSize = _defaultTextSize * _currentScale;
    }
}
