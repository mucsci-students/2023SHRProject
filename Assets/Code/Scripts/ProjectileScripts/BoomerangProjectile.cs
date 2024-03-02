using UnityEngine;

/// <summary>
/// Handles the movement and collision of projectiles.
/// </summary>
public class BoomerangProjectile : ProjectileScript
{
    private Vector3 _midpoint = Vector3.zero;
    public bool isReturningToTower;

    public bool invincible;
    public float noDamageTimeLimit = 0.00000001f;
    public float timer;

    protected override void FixedUpdate()
    {
        if (DistanceTraveled > MaxDistance)
        {
            Destroy(gameObject);
        }
        else if (Target != null)
        {
            var position = Target.transform.position;
            LookAt(position);
            MoveTowardsTarget(position, speed * Time.fixedDeltaTime);
        }
        else
        {
            Circle();
            
            if (invincible)
            {
                timer += Time.fixedDeltaTime;
                if (timer >= noDamageTimeLimit)
                {
                    invincible = false;
                    timer = 0f;
                }
            }
        }
    }

    private void Circle ()
    {
        if (!isReturningToTower)
        {
            var position = transform.position;
            var position1 = parentMonkeyScript.gameObject.transform.position;
            _midpoint = new Vector3((position.x + position1.x) / 2f, (position.y + position1.y) / 2f);
            isReturningToTower = true;
        } 
        const float degreesPerSecond = 2500f; // Desired rotation speed in degrees per second
        float radius = Vector3.Distance(transform.position, _midpoint);
        float circumference = 2 * Mathf.PI * radius;
        float anglePerFrame = (degreesPerSecond / 360f) * 360f * (Time.fixedDeltaTime / circumference);
        //transform.position = new Vector2(midpoint.x + (1.5f * Mathf.Sin(Mathf.Deg2Rad * alpha)), midpoint.y + ( 2f * Mathf.Cos(Mathf.Deg2Rad * alpha)));
        transform.RotateAround(_midpoint, Vector3.forward, anglePerFrame);
        //alpha += 0.5f;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && !invincible && other.gameObject.CompareTag("Bloon"))
        {
            var bloonScript = other.gameObject.GetComponent<BloonScript>();
            // Objects get destroyed at the end of the frame. This prevents bloon from being destroyed twice by two bloons.
            if (bloonScript.GetHealth() <= 0)
            {
                return;
            }

            var layersPopped = bloonScript.ReceiveDamage((int)LayersPerHit);
            parentMonkeyScript.IncrementLayersPopped(layersPopped);

            invincible = true;
            Target = null;

        }
        else if (isReturningToTower && other.gameObject == parentMonkeyScript.gameObject)
        {
            Destroy(gameObject);
            Destroy(this);
            LayersPerHit = 0;
        }
    }
}


