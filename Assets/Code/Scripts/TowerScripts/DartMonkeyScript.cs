using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartMonkeyScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {
        firingRate /= 2;
        GetComponent<SpriteRenderer>().sprite = upgradePath1[0].GetSprite();
    }

    protected override void Upgrade1_2()
    {
        firingRate /= 2;
        GetComponent<SpriteRenderer>().sprite = upgradePath1[1].GetSprite();
    }
    
    protected override void Upgrade2_1()
    {
        radiusSpriteRenderer.transform.localScale *= 1.5f;
        
        GetComponent<SpriteRenderer>().sprite = upgradePath2[0].GetSprite();
    }

    protected override void Upgrade2_2()
    {
        radiusSpriteRenderer.transform.localScale *= 1.5f;
        
        GetComponent<SpriteRenderer>().sprite = upgradePath2[1].GetSprite();
    }
}
