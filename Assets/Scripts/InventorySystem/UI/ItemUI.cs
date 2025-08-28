using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private int quantity;

    public void SetData(Sprite sprite, int quantity)
    {
        itemImage.sprite = sprite;
        this.quantity = quantity;
    }

    public void ResetData()
    {
        itemImage.sprite = null;
        quantity = 0;
    }
}
