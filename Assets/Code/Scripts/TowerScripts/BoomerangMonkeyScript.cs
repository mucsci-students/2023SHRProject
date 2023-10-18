using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangMonkeyScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {
        //Sharp Shots
        layersPoppedPerHit = 2;
    }

    protected override void Upgrade1_2()
    {
       //Sharper Shots
        layersPoppedPerHit = 3;
    }

    protected override void Upgrade2_1()
    {
        //More Range
        radiusSpriteRenderer.transform.localScale *= 1.1f;
        
    }

    protected override void Upgrade2_2()
    {
        //Even more Range
        radiusSpriteRenderer.transform.localScale *= 1.12f;
        
    }
}
