using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceHitableObj
{
    void Hit(RaycastHit hit, int damage = 1);
}
