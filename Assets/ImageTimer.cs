using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTimer : MonoBehaviour
{
    public float MaxTime;

    private Image img;
    private float currentTime;

    public bool Tick;

    void Start()
    {
        img = GetComponent<Image>();
        currentTime = MaxTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        Tick = false;
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            Tick = true;
            currentTime = MaxTime;
        }

        img.fillAmount = currentTime / MaxTime;
    }
}
