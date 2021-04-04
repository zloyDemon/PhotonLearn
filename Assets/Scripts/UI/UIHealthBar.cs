using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
     [SerializeField] private Gradient gradient;
     [SerializeField] private Image bg;
     [SerializeField] private Image bar;
     [SerializeField] private Text text;

     public void UpdateLife(int hp, int totalHp)
     {
          float f = (float) hp / (float) totalHp;
          bar.fillAmount = f;
          Color c = gradient.Evaluate(f);
          bg.color = new Color(c.r, c.g, c.b, bg.color.a);
          bar.color = c;
          text.text = hp.ToString();
     }
}
