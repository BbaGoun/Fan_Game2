using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionPart
{
    public static class Utility
    {
        public static void DrawBox(Vector3 position, Vector2 size, Color color)
        {
            Vector2 center = new Vector2(position.x, position.y);
            var sizeX = size.x;
            var sizeY = size.y;
            Debug.DrawLine(center + new Vector2(-sizeX, sizeY) / 2, center + new Vector2(sizeX, sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(sizeX, sizeY) / 2, center + new Vector2(sizeX, -sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(sizeX, -sizeY) / 2, center + new Vector2(-sizeX, -sizeY) / 2, color);
            Debug.DrawLine(center + new Vector2(-sizeX, -sizeY) / 2, center + new Vector2(-sizeX, sizeY) / 2, color);
        }

        public static float EaseOutQuint(float x)
        {
            return 1 - Mathf.Pow(1 - x, 5);
        }
    }
}