using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperMonkeyScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {
        //Full Metal Jacket
        //Description: Shots can pop through 4 layers of bloon - and can pop lead and frozen bloons.
        layersPoppedPerHit = 4;
    }

    protected override void Upgrade1_2()
    {
       //Point Five Oh 
       //Description: Shots can pop through 7 layers of bloon!
       layersPoppedPerHit = 7;
    }

    protected override void Upgrade2_1()
    {
        //Faster Firing 
        //Description: Allows Sniper to shoot faster(40%).
        projectileSpeed = 70;
    }

    protected override void Upgrade2_2()
    {
        //Night Vision Goggles 
        //Description: Allows Sniper to detect and shoot Camo bloons.
        //TODO: Add Camo
    }
}
