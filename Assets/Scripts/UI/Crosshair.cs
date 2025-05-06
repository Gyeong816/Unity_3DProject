using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Elements")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Crosshair Settings")]
    public float defaultSpread = 20.0f;
    public float maxSpread = 50.0f;
    public float spreadSpeed = 5.0f;

    private float currentSpread;
    

    private void Start()
    {
        currentSpread = defaultSpread;
        UpdateCrosshairPosition();
    }


    private void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            currentSpread = maxSpread;
        }
        
        currentSpread = Mathf.Lerp(currentSpread, defaultSpread, spreadSpeed * Time.deltaTime);
        UpdateCrosshairPosition();
    }

    private void UpdateCrosshairPosition()
    {
        top.anchoredPosition = new Vector2(0, currentSpread);
        bottom.anchoredPosition = new Vector2(0, -currentSpread);
        
        left.anchoredPosition = new Vector2(-currentSpread, 0);
        right.anchoredPosition = new Vector2(currentSpread, 0);
    }
}
