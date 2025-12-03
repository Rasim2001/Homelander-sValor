using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Data
{
    public static class DataExtensions
    {
        public static string ToJson(this object obj) =>
            JsonUtility.ToJson(obj);

        public static T ToDeserialized<T>(this string json) =>
            JsonUtility.FromJson<T>(json);

        public static Vector2Data AsVectorData(this Vector3 vector) =>
            new(vector.x, vector.y);

        public static Vector2 AsUnityVector(this Vector2Data vector2Data) =>
            vector2Data == null ? new Vector2(0, -2.75f) : new Vector2(vector2Data.X, vector2Data.Y);

        public static void AddAndSort<T>(this List<T> list, T orderMarker) where T : MonoBehaviour
        {
            if (orderMarker == null)
                return;

            list.Add(orderMarker);

            if (orderMarker.transform.position.x > 0)
                list.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            else
                list.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));
        }

        public static T WithSetPosition<T>(this T component, Vector3 position) where T : Component
        {
            component.transform.position = position;
            return component;
        }
    }
}