using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

public class ClockController : SoundObject
{
    [SerializeField]Animation anim;
    public override void Effect()
    {
        base.Effect();
        anim.Play();

    }
}
