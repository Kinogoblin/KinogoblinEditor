using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteChanger : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material material;
    [SerializeField] private string propName;

    void Update()
    {
        material.SetTexture(propName, spriteRenderer.sprite.texture);
    }
}
