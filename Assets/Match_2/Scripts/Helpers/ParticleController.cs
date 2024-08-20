using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private bool clearListenersAfterCall;
    [Space(10), SerializeField] private UnityEvent onParticleSystemStopped;

    private ParticleSystem particle;
    private ParticleSystem.MainModule mainModule;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        mainModule = particle.main;
    }

    private void OnParticleSystemStopped()
    {
        onParticleSystemStopped?.Invoke();
    }

    public void ParticleSystemStoppedAction(Action _action) => onParticleSystemStopped.AddListener(() =>
    {
        _action?.Invoke();

        if (clearListenersAfterCall)
            onParticleSystemStopped.RemoveAllListeners();
    });

    public void PlayParticle() => particle.Play();
    public void ChangeStartSize(float _size) => mainModule.startSize = _size;
}
