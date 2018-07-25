/*using UnityEngine;

namespace PixelCrushers.DialogueSystem.DialogueEditor
{

    public class GUIZoom
    {

        private static Matrix4x4 originalMatrix;
        private static float kEditorWindowTabHeight = 22f;

        public static void BeginZoomView(Rect screenCoordsArea, float zoomScale, bool docked)
        {
            GUI.EndGroup();
            kEditorWindowTabHeight = (docked ? 19f : 22f);
            Rect rect = ZoomRect(screenCoordsArea, 1f / zoomScale, GetUpperLeft(screenCoordsArea));
            rect.y = rect.y + kEditorWindowTabHeight;
            GUI.BeginGroup(rect);
            originalMatrix = GUI.matrix;
            Matrix4x4 matrix4x4 = Matrix4x4.TRS(GetUpperLeft(rect), Quaternion.identity, Vector3.one);
            Vector3 vector3 = Vector3.one;
            float single = zoomScale;
            float single1 = single;
            vector3.y = single;
            vector3.x = single1;
            Matrix4x4 matrix4x41 = Matrix4x4.Scale(vector3);
            GUI.matrix = ((matrix4x4 * matrix4x41) * matrix4x4.inverse) * GUI.matrix;
        }

        public static void EndZoomView()
        {
            GUI.matrix = originalMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0f, GUIZoom.kEditorWindowTabHeight, (float)Screen.width, (float)Screen.height));
        }

        private static Vector2 GetUpperLeft(Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        private static Rect ZoomRect(Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect rect1 = rect;
            rect1.x = rect1.x - pivotPoint.x;
            rect1.y = rect1.y - pivotPoint.y;
            rect1.xMin = rect1.xMin * scale;
            rect1.xMax = rect1.xMax * scale;
            rect1.yMin = rect1.yMin * scale;
            rect1.yMax = rect1.yMax * scale;
            rect1.x = rect1.x + pivotPoint.x;
            rect1.y = rect1.y + pivotPoint.y;
            return rect1;
        }
    }
}
*/