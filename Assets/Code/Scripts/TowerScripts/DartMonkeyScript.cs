using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartMonkeyScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {   
        //Long Range Darts
        radiusSpriteRenderer.transform.localScale *= 1.1f;
        GetComponent<SpriteRenderer>().sprite = upgradePath1[0].GetSprite();
    }
    
    protected override void Upgrade1_2()
    {
        //Enhanced Eyesight
        radiusSpriteRenderer.transform.localScale *= 1.11f;
        //TODO: ADD CAMO
        GetComponent<SpriteRenderer>().sprite = upgradePath1[1].GetSprite();
    }
    
    protected override void Upgrade2_1()
    {
        //Sharp Shots
        layersPoppedPerHit = 2;
        GetComponent<SpriteRenderer>().sprite = upgradePath2[0].GetSprite();
    }

    protected override void Upgrade2_2()
    {
        //Razor Sharp Shots
        layersPoppedPerHit = 4;
        GetComponent<SpriteRenderer>().sprite = upgradePath2[1].GetSprite();
    }
}
