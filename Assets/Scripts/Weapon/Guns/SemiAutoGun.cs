using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutoGun : BaseGun
{
    public override void Shoot(bool state)
    {
        if (state)
        {
            ShootGun();
        }
    }
}
