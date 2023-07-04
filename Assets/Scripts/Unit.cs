using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int _health = 0;
    [SerializeField] private int _healingAmount = 5; 
    [SerializeField] private int _maxHealth = 100; 
    [SerializeField] private float _healingDelayValue = .5f;
    [SerializeField] private float _healingDuration = 3f;
    
    private Coroutine _receiveHealingCoroutine;
    private WaitForSeconds _healingDelay;

    private void Start()
    {
        _healingDelay = new WaitForSeconds(_healingDelayValue);
        ReceiveHealing();
    }

    public void ReceiveHealing()
    {
        StopHealing();
        _receiveHealingCoroutine = StartCoroutine(ReceiveHealingCoroutine());
    }

    private void StopHealing()
    {
        if (_receiveHealingCoroutine != null)
        {
            StopCoroutine(_receiveHealingCoroutine);
            _receiveHealingCoroutine = null;
        }
    }

    private IEnumerator ReceiveHealingCoroutine()
    {
        float duration = 0f;
        
        while (_health < _maxHealth && duration < _healingDuration)
        {
            _health += _healingAmount;
            print(_health);

            duration += _healingDelayValue;
            yield return _healingDelay;
        }

        _receiveHealingCoroutine = null;
    }
}
