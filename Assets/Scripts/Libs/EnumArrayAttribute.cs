using UnityEngine;
using System;

public class EnumArrayAttribute : PropertyAttribute {
    public Type Type;
    public EnumArrayAttribute (Type type) {
        Type = type;
    }
}