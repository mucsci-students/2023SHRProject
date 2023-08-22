using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Stores bloonPath for a bloon and handles logic related to the bloon following the path. Will self delete and subtract
/// lives when it reaches its' final path.
/// </summary>
[RequireComponent(typeof(BloonScript))]
public class PathFollowingScript : MonoBehaviour
{
    /// <summary>
    /// BloonPath is implemented using a stack of transform where the next target along the path is at the top/peek
    /// of the stack and the last target position/(where you lose lives) is at the bottom of the stack.
    /// </summary>
    [SerializeField] private List<Transform> bloonPath;

    public uint CurrentTargetIndex = 0;
    private uint _speed;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (bloonPath == null || bloonPath.Count <= 0)
        {
            Debug.LogWarning("Path is null or empty on " + gameObject.name);
        }

        if (!gameObject.TryGetComponent(out BloonScript bloonScript))
        {
            Debug.LogWarning("Bloon is missing BloonScript on " + gameObject.name);
        }

        _speed = bloonScript.GetSpeed();
    }

    // Update is called once per frame
    private void Update()
    {
        var targetPosition = bloonPath[(int)CurrentTargetIndex].position;
        
        if (ReachedTarget(targetPosition))
        {
            // Update target
            ++CurrentTargetIndex;
            if (CurrentTargetIndex == bloonPath.Count)
            {
                SubtractLives();
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
        return (transform.position.x == targetPosition.x &&
                transform.position.y == targetPosition.y);
    }

    private void SubtractLives()
    {
        //throw new NotImplementedException();
    }
    
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
        
        transform.Translate(translateVector);
    }
}
