using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private float _speed = 1f;
    private float _maxDistance = 10f;
    private uint _layersPerHit = 1;
    private uint _pierceAmount = 1;

    private GameObject _target;
    private MonkeyScript _parentMonkeyScript;

    // Update is called once per frame
    private void Update()
    {
        if (_target != null)
        {
            transform.position = MoveTowards(_target.transform.position, _speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAllAttributes(float speed, float maxDistance, uint layersPerHit, uint pierceAmount, GameObject target, MonkeyScript parent)
    {
        _speed = speed;
        _maxDistance = maxDistance;
        _layersPerHit = layersPerHit;
        _pierceAmount = pierceAmount;
        _target = target;
        _parentMonkeyScript = parent;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();
            var layersPopped = bloonScript.ReceiveDamage((int)_layersPerHit);
            _parentMonkeyScript.IncrementLayersPopped(layersPopped);
            
            // fix bug
            Destroy(gameObject);
            Destroy(this);
            _layersPerHit = 0;
        }
    }

    private Vector3 MoveTowards(Vector3 targetPosition, float speed)
    {
        return Vector3.MoveTowards(transform.position, targetPosition, speed);
    }
}
