namespace EasyButtons.Editor
{
    using System.Linq;
    using SolidUtilities.Editor.Helpers;
    using UnityEditor;

    internal class NoScriptFieldEditor : Editor
    {
        private GeneralProperty[] _endProperties;

        private void OnEnable()
        {
            _endProperties = GetEndProperties(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script");
        }

        public void ApplyModifiedProperties()
        {
            if (serializedObject.hasModifiedProperties)
            {
                foreach (GeneralProperty childProperty in _endProperties)
                {
                    childProperty.UpdateObjectValue();
                }

                // serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static GeneralProperty[] GetEndProperties(SerializedObject serializedObject)
        {
            var children = new ChildProperties(serializedObject, true);
            return children
                .Where(childProperty => !childProperty.hasVisibleChildren)
                .Select(childProperty => new GeneralProperty(childProperty))
                .ToArray();
        }
    }
}