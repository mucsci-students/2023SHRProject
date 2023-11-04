using UnityEngine;

/// <summary>
/// Handles the movement and collision of projectiles.
/// </summary>
public class ProjectileScript : MonoBehaviour
{
    public float speed = 1f;
    protected float MaxDistance = 10f;
    protected uint LayersPerHit = 1;

    // TODO - Implement this 
    protected uint PierceAmount = 1;

    protected float DistanceTraveled;
    private Vector3 _lastTargetDirection = Vector3.zero;

    protected GameObject Target;
    public MonkeyScript parentMonkeyScript;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (DistanceTraveled > MaxDistance)
        {
            Destroy(gameObject);
        }
        else if (Target != null)
        {
            var position = Target.transform.position;
            LookAt(position);
            MoveTowardsTarget(position, speed * Time.deltaTime);
        }
        else
        {
            MoveInDirection(_lastTargetDirection);
        }
    }

    public void SetAllAttributes(float newSpeed, float maxDistance, uint layersPerHit, uint pierceAmount, GameObject target, MonkeyScript parent)
    {
        speed = newSpeed;
        MaxDistance = maxDistance;
        LayersPerHit = layersPerHit;
        PierceAmount = pierceAmount;
        Target = target;
        parentMonkeyScript = parent;
    }
    
    public void SetAllAttributes(float newSpeed, float maxDistance, uint layersPerHit, uint pierceAmount, Vector3 direction, MonkeyScript parent)
    {
        speed = newSpeed;
        MaxDistance = maxDistance;
        LayersPerHit = layersPerHit;
        PierceAmount = pierceAmount;
        _lastTargetDirection = direction;
        parentMonkeyScript = parent;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();
            
            // Objects get destroyed at the end of the frame. This prevents bloon from being destroyed twice by two bloons.
            if (bloonScript.GetHealth() <= 0)
            {
                return;
            }
            
            var layersPopped = bloonScript.ReceiveDamage((int)LayersPerHit);
            parentMonkeyScript.IncrementLayersPopped(layersPopped);
            
            Destroy(gameObject);
            Destroy(this);
            LayersPerHit = 0;
        }
    }

    /// <summary>
    /// Moves the projectile towards a targetPosition based on a speed.
    /// </summary>
    /// <param name="targetPosition">The position to move this projectile towards</param>
    /// <param name="currentSpeed">Speed of the projectile. Should be calculated based on delta time.</param>
    protected void MoveTowardsTarget(Vector3 targetPosition, float currentSpeed)
    {
        Vector3 newPosition = GetNewPosition(targetPosition, currentSpeed);
        _lastTargetDirection = newPosition - transform.position;
        SetPosition(newPosition);
    }

    protected void LookAt(Vector3 targetPosition)
    {
        Vector3 myLocation = transform.position;
        Vector3 vectorToTarget = targetPosition - myLocation;

        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

        // Apply the rotation
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // Subtract 90 to align with the sprite's orientation
    }

    protected void MoveInDirection(Vector3 direction)
    {
        Vector3 newPosition = GetNewPosition(transform.position + direction, speed * Time.deltaTime);
        SetPosition(newPosition);
    }

    /// <summary>
    /// Sets the position of this transform and updates distance traveled.
    /// </summary>
    /// <param name="newPosition">The new position to move the projectile to during this frame</param>
    protected void SetPosition(Vector3 newPosition)
    {
        DistanceTraveled += Vector3.Distance(newPosition , transform.position);
        transform.position = newPosition;
    }

    /// <summary>
    /// Returns a new vector that will be the new position of this based on the targetPosition position and speed which limits
    /// how far the projectile will move per frame.
    /// </summary>
    /// <param name="targetPosition">The targetPosition position for this projectile to move towards</param>
    /// <param name="currentSpeed">How fast to move towards the targetPosition position</param>
    /// <returns>The new position of this projectile</returns>
    private Vector3 GetNewPosition(Vector3 targetPosition, float currentSpeed)
    {
        // Force projectile to move in 2D space
        var position = transform.position;
        targetPosition.z = position.z;
        return Vector3.MoveTowards(position, targetPosition, currentSpeed);
    }
}
