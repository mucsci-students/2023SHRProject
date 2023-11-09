
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {
        //Increase range+
        radiusSpriteRenderer.transform.localScale *= 1.1f;
    }

    protected override void Upgrade1_2()
    {
        //Increase range++
        radiusSpriteRenderer.transform.localScale *= 1.11f;
        //TODO: ADD CAMO
    }

    protected override void Upgrade2_1()
    {
        //Fast Speed
        projectileSpeed = 2;
    }

    protected override void Upgrade2_2()
    {
        //Fastest Speed
        projectileSpeed = 3;
    }
}