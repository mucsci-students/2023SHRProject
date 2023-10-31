using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MonkeyRangeScript : MonoBehaviour
{
    [Tooltip("Object Link(s)")]
    
    [SerializeField]
    private MonkeyScript parentMonkeyScript;

    private bool isSet = false;
    
    //Start is called before the first frame update
    private void Start()
    {
        if (parentMonkeyScript == null)
        {
            parentMonkeyScript = transform.parent.GetComponent<MonkeyScript>();
        }

        StartCoroutine(runOnce());
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (parentMonkeyScript == null)
        {
            return;
        }
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            parentMonkeyScript.AddBloonToRange(other.gameObject);
        }
    }

    private IEnumerator runOnce()
    {
        yield return new WaitForSeconds(0.1f);
        isSet = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isSet || parentMonkeyScript == null)
        {
            return;
        }
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            parentMonkeyScript.TryAddBloonToRange(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (parentMonkeyScript == null)
        {
            return;
        }
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            parentMonkeyScript.RemoveBloonFromRange(other.gameObject);
        }
    }
}
