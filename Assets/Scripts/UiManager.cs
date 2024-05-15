using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthBar;

    public void updateHealthText(float newHealth)
    {
        healthText.text = "Health: " + newHealth;
        healthBar.value = newHealth / 100 * 1;
    }
}
