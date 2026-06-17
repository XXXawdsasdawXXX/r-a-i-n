using UnityEngine;

namespace Essential
{
    public static class UIUtils
    {
        public static bool IsCursorOverUIElement(RectTransform rectTransform, Camera uiCamera = null)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                Input.mousePosition,
                uiCamera
            );
        }
    }
}