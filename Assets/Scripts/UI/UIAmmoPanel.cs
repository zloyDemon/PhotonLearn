using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmoPanel : MonoBehaviour
{
    [SerializeField] private Text currentText;
    [SerializeField] private Text totalText;

    public void UpdateAmmo(int current, int total)
    {
        currentText.text = current.ToString();
        totalText.text = total.ToString();
    }
}
