using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public bool rotateIcon = false;
    public int rotationSpeed = 180;
    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            rotateIcon = true;

        }
        catch
        {
            throw new System.NotImplementedException();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            button.image.transform.rotation = Quaternion.identity;
            rotateIcon = false;
        }
        catch
        {
            throw new System.NotImplementedException();
        }
    }

    public void Update()
    {
        if (rotateIcon)
        {
            button.image.transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
        }
    }
}
