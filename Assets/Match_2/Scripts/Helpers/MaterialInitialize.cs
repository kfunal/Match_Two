using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInitialize : MonoBehaviour
{
    [SerializeField] private Texture texture;

    private ParticleSystem particle;
    private ParticleSystemRenderer particleRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        InitMaterial();
    }

    private void InitMaterial()
    {
        particle = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        particleRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", texture);
        particleRenderer.SetPropertyBlock(propertyBlock, 0);
    }
}
