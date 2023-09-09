using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores bloonPath for a bloon and handles logic related to the bloon following the path. Will self delete and subtract
/// lives when it reaches its' final path.
/// </summary>
[RequireComponent(typeof(BloonScript))]
public class PathFollowingScript : MonoBehaviour
{
    /// <summary>
    /// BloonPath is implemented using a List of transforms where the next target along the path is at the lowest index
    /// of the List and the last target position/(where you lose lives) is at the bottom of the stack.
    /// </summary>
    [SerializeField] private List<Transform> bloonPath;

    private uint _currentTargetIndex = 0;
    private uint _speed;
    private float _distanceTraveled = 0;

    // Start is called before the first frame update
    private void Start()
    {
        
        if (!gameObject.TryGetComponent(out BloonScript bloonScript))
        {
            Debug.LogWarning("Bloon is missing BloonScript on " + gameObject.name);
        }

        _speed = bloonScript.GetSpeed();
    }

    // Update is called once per frame
    private void Update()
    {
        // Takes a frame for bloon path to be set when spawning child(this) bloon.
        if (bloonPath.Count == 0)
            return;

        var targetPosition = bloonPath[(int)_currentTargetIndex].position;
        

        if (ReachedTarget(targetPosition))
        {
            // Update target
            ++_currentTargetIndex;
            if (_currentTargetIndex == bloonPath.Count)
            {
                SubtractLives();
                WaveManager.enemiesRemaining -= 1;
                Destroy(gameObject);
            }
        }

        MoveToCurrentTarget(targetPosition);
    }

    /// <summary>
    /// Checks if the current gameObject/bloon is overlapping with a target position on the X and Y axis
    /// </summary>
    /// <param name="targetPosition">The target vector to compare the position with</param>
    /// <returns>True if this position and the target position are equal based on x and y components, otherwise false</returns>
    private bool ReachedTarget(Vector3 targetPosition)
    {
        const double tolerance = 0.01;
        var currentPosition = transform.position;
        return Math.Abs(currentPosition.x - targetPosition.x) < tolerance &&
               Math.Abs(currentPosition.y - targetPosition.y) < tolerance;
    }

    private void SubtractLives()
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Moves the attached gameObject to targetPosition at a max speed of the _speed field.
    /// </summary>
    /// <param name="targetPosition">The position of where the attached gameObject should move to</param>
    private void MoveToCurrentTarget(Vector3 targetPosition)
    {
        //throw new NotImplementedException();
        var translateVector = new Vector3(0f, 0f, 0f);
        var movementDistance = _speed * Time.deltaTime;
        
        if (transform.position.x < targetPosition.x)
        {
            translateVector.x = Math.Min(Math.Abs(transform.position.x - targetPosition.x), movementDistance);
        } else if (transform.position.x > targetPosition.x)
        {
            translateVector.x = -Math.Min(Math.Abs(transform.position.x - targetPosition.x), movementDistance);
        }
        
        if (transform.position.y < targetPosition.y)
        {
            translateVector.y = Math.Min(Math.Abs(transform.position.y - targetPosition.y), movementDistance);
        } else if (transform.position.y > targetPosition.y)
        {
            translateVector.y = -Math.Min(Math.Abs(transform.position.y - targetPosition.y), movementDistance);
        }

        _distanceTraveled += Vector3.Distance(Vector3.zero, translateVector);
        transform.Translate(translateVector);
    }
    
    // Setters and Getters
    
    public List<Transform> GetBloonPath()
    {
        return bloonPath;
    }

    public void SetBloonPath(List<Transform> newBloonPath)
    {
        bloonPath = newBloonPath;
    }

    public uint GetCurrentTargetIndex()
    {
        return _currentTargetIndex;
    }

    public void SetCurrentTargetIndex(uint targetIndex)
    {
        _currentTargetIndex = targetIndex;
    }

    public void SetDistanceTravelled(float distanceTraveled)
    {
        _distanceTraveled = distanceTraveled;
    }

    public float GetDistanceTravelled()
    {
        return _distanceTraveled;
    }
    
}
