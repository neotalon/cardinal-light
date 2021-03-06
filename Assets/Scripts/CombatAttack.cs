﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            //only hurt unpacified enemies
            if (other.GetComponent<Enemy>().m_pacified)
            {
                other.GetComponent<Enemy>().Damage(15f * Time.deltaTime);
            }
        }
    }
}
