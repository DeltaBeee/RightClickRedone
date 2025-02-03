using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Reworked_Overpower_Explosion : MonoBehaviour {
    public float maxHpDmgScale;

    private void Awake() {
        Explosion component = GetComponent<Explosion>();
        component.hitPlayerAction = (Action<CharacterData, float>)Delegate.Combine(component.hitPlayerAction, new Action<CharacterData, float>(HitPlayer));
    }

    private void HitPlayer(CharacterData data, float rangeMultiplier) {
        SpawnedAttack component = GetComponent<SpawnedAttack>();
        if (component.IsMine()) {
            // vanilla: scales with users max hp
            // float num = component.spawner.data.maxHealth * maxHpDmgScale * 0.01f * base.transform.localScale.x;

            float num = 80;
            data.healthHandler.CallTakeDamage(num * (data.transform.position - component.spawner.transform.position).normalized, base.transform.position, null, GetComponent<SpawnedAttack>().spawner);
        }
    }
}
