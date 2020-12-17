namespace EasyButtons.Editor
{
    using System.Collections.Generic;
    using System.Reflection;
    using JetBrains.Annotations;
    using NUnit.Framework;
    using SolidUtilities.Extensions;
    using UnityEditor;
    using UnityEngine;

    internal class GeneralProperty
    {
        private static readonly Dictionary<SerializedPropertyType, string> TypeValueDict = new Dictionary<SerializedPropertyType, string>
        {
            { SerializedPropertyType.Generic, null },
            { SerializedPropertyType.Integer, nameof(SerializedProperty.intValue) },
            { SerializedPropertyType.Boolean, nameof(SerializedProperty.boolValue) },
            { SerializedPropertyType.Float, nameof(SerializedProperty.floatValue) },
            { SerializedPropertyType.String, nameof(SerializedProperty.stringValue) },
            { SerializedPropertyType.Color, nameof(SerializedProperty.colorValue) },
            { SerializedPropertyType.ObjectReference, nameof(SerializedProperty.objectReferenceValue) },
            { SerializedPropertyType.LayerMask, null },
            { SerializedPropertyType.Enum, nameof(SerializedProperty.enumValueIndex) },
            { SerializedPropertyType.Vector2, nameof(SerializedProperty.vector2Value) },
            { SerializedPropertyType.Vector3, nameof(SerializedProperty.vector3Value) },
            { SerializedPropertyType.Vector4, nameof(SerializedProperty.vector4Value) },
            { SerializedPropertyType.Rect, nameof(SerializedProperty.rectValue) },
            { SerializedPropertyType.ArraySize, null },
            { SerializedPropertyType.Character, null },
            { SerializedPropertyType.AnimationCurve, nameof(SerializedProperty.animationCurveValue) },
            { SerializedPropertyType.Bounds, nameof(SerializedProperty.boundsValue) },
            { SerializedPropertyType.Gradient, null },
            { SerializedPropertyType.Quaternion, nameof(SerializedProperty.quaternionValue) },
            { SerializedPropertyType.ExposedReference, nameof(SerializedProperty.exposedReferenceValue) },
            { SerializedPropertyType.FixedBufferSize, null },
            { SerializedPropertyType.Vector2Int, nameof(SerializedProperty.vector2IntValue) },
            { SerializedPropertyType.Vector3Int, nameof(SerializedProperty.vector3IntValue) },
            { SerializedPropertyType.RectInt, nameof(SerializedProperty.rectIntValue) },
            { SerializedPropertyType.BoundsInt, nameof(SerializedProperty.boundsIntValue) },
            { SerializedPropertyType.ManagedReference, nameof(SerializedProperty.managedReferenceValue) },
        };

        private readonly SerializedProperty _property;
        private readonly PropertyInfo _valueGetter;
        private readonly FieldInfo _objectField;
        private readonly Object _target;

        public readonly string PropertyPath;

        public GeneralProperty(SerializedProperty property)
        {
            _property = property.Copy();
            PropertyPath = _property.propertyPath;

            string propertyName = TypeValueDict[_property.propertyType];

            if (string.IsNullOrEmpty(propertyName))
            {
                _valueGetter = null;
                return;
            }

            _valueGetter = typeof(SerializedProperty).GetProperty(propertyName);
            Assert.IsNotNull(_valueGetter);

            _target = _property.serializedObject.targetObject;
            _objectField = GetObjectField();
        }

        [NotNull]
        private FieldInfo GetObjectField()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            FieldInfo targetFieldInfo = _target.GetType().GetFieldAtPath(PropertyPath, flags);
            Assert.IsNotNull(targetFieldInfo);
            return targetFieldInfo;
        }

        public void UpdateObjectValue()
        {
            if (_valueGetter == null)
                return;

            object propertyValue = _valueGetter.GetValue(_property);

            if (propertyValue != _objectField.GetValue(_target))
            {
                _objectField.SetValue(_target, propertyValue);
                Debug.Log($"set value of {PropertyPath}");
            }
        }
    }
}