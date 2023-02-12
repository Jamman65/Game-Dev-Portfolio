﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JO
{
    public class Staminabar : MonoBehaviour
    {
        public Slider slider;

        public void SetMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }
        public void SetCurrentStamina(float currentStamina)
        {
            slider.value = currentStamina;
        }
    }
}
