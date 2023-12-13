using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullAutoGun : BaseGun
{
    public override void Shoot(bool state)
    {
        Shooting(state);
    }

    void Update()
    {
        ShootUpdate();
    }
}
