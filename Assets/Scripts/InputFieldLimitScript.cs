using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Script to limit the input boxes integer value, add to object that contains the TMP Input Field and set the minimum and maximum value
public class InputScript : MonoBehaviour
{
    private TMP_InputField _inputField;

    public int minValue = 1;
    public int maxValue = 500;

    private void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _inputField.onValueChanged.AddListener(OnInputValueChanged);
    }

    private void OnInputValueChanged(string value)
    {
        if (int.TryParse(value, out int intValue))
        {
            intValue = Mathf.Clamp(intValue, minValue, maxValue);
            _inputField.text = intValue.ToString();
        }
        else
        {
            _inputField.text = minValue.ToString();
        }
    }
}