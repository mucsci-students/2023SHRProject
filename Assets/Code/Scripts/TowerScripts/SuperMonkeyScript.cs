
public class SuperMonkeyScript : MonkeyScript
{
    /// <inheritdoc />
    protected override void Upgrade1_1()
    {
        radiusSpriteRenderer.transform.localScale *= 1.1f;
    }

    /// <inheritdoc />
    protected override void Upgrade1_2()
    {
        radiusSpriteRenderer.transform.localScale *= 1.1f;
    }

    /// <inheritdoc />
    protected override void Upgrade2_1()
    {
        firingRate = 0.15f;
    }

    /// <inheritdoc />
    protected override void Upgrade2_2()
    {
        firingRate = 0.10f;
    }
}
