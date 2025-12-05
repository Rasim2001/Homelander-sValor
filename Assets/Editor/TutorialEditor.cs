using System.Linq;
using _Tutorial;
using Infastructure.StaticData.Tutorial;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TutorialStaticData))]
    public class TutorialEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TutorialStaticData tutorialData = (TutorialStaticData)target;

            if (GUILayout.Button("Collect"))
            {
                tutorialData.TutorialObjects = FindObjectsOfType<TutorialMarker>()
                    .Select(x => new TutorialData(x.transform.position, x.TypeId)).ToList();
            }

            EditorUtility.SetDirty(target);
        }
    }
}