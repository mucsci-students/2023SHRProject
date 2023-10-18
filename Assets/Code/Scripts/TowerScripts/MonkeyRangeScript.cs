using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MonkeyRangeScript : MonoBehaviour
{
    [Tooltip("Object Link(s)")]
    
    [SerializeField]
    private MonkeyScript parentMonkeyScript;
    
    //Start is called before the first frame update
    private void Start()
    {
        if (parentMonkeyScript == null)
        {
            parentMonkeyScript = transform.parent.GetComponent<MonkeyScript>();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            Debug.Log(other.gameObject.name);
            parentMonkeyScript.AddBloonToRange(other.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject != null && other.gameObject.CompareTag("Bloon"))
        {
            parentMonkeyScript.RemoveBloonFromRange(other.gameObject);
        }
    }
}
