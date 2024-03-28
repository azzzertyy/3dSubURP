using UnityEngine;
using UnityEditor;

// Defines a class attribute that will tell the editor if the script needs to have a collider or a trigger.
[System.AttributeUsage(System.AttributeTargets.Class)]
public class RequireCollider : UnityEngine.Scripting.PreserveAttribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public class RequireTrigger : UnityEngine.Scripting.PreserveAttribute { }

#if UNITY_EDITOR

[CustomEditor(typeof(MonoBehaviour), true)]
public class RequireColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var monoBehaviour = target as MonoBehaviour;

        // Check if the component requires a regular collider.
        var requiresCollider = System.Attribute.GetCustomAttribute(monoBehaviour.GetType(), typeof(RequireCollider)) != null;
        if (requiresCollider && monoBehaviour.GetComponent<Collider>() == null)
        {
            EditorGUILayout.HelpBox("This component requires a Collider component.", MessageType.Error);
        }

        // Check if the component requires a trigger collider.
        var requiresTrigger = System.Attribute.GetCustomAttribute(monoBehaviour.GetType(), typeof(RequireTrigger)) != null;
        if (requiresTrigger && monoBehaviour.GetComponent<Collider>() == null)
        {
            EditorGUILayout.HelpBox("This component requires a Trigger Collider component.", MessageType.Error);
        }
    }
}

#endif
