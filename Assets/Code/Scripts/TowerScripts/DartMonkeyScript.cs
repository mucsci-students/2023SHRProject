
public class DartMonkeyScript : MonkeyScript
{
    protected override void Upgrade1_1()
    {   
        //Long Range Darts
        radiusSpriteRenderer.transform.localScale *= 1.1f;
    }

    protected override void Upgrade1_2()
    {
        //Enhanced Eyesight
        radiusSpriteRenderer.transform.localScale *= 1.11f;
        //TODO: ADD CAMO
    }
    
    protected override void Upgrade2_1()
    {
        //Sharp Shots
        layersPoppedPerHit = 2;
    }

    protected override void Upgrade2_2()
    {
        //Razor Sharp Shots
        layersPoppedPerHit = 4;
    }
}
