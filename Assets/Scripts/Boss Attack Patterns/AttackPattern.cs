using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern : ScriptableObject {
    public virtual bool ExecutePattern(Transform origin) {
        return false;
    }

    public virtual void Reinitialize(Transform origin) { }
}
