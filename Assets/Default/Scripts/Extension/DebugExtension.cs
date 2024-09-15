using UnityEngine;

namespace Default.Scripts.Extension
{
    public class DebugExtension
    {
        //Draws just the box at where it is currently hitting.
        public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation,
            Vector3 direction, float hitInfoDistance, Color color)
        {
            origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
            DrawBox(origin, halfExtents, orientation, color);
        }

        //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
        public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation,
            Vector3 direction, float distance, Color color)
        {
            direction.Normalize();
            var bottomBox = new Box(origin, halfExtents, orientation);
            var topBox = new Box(origin + direction * distance, halfExtents, orientation);

            UnityEngine.Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft, color);
            UnityEngine.Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
            UnityEngine.Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
            UnityEngine.Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight, color);
            UnityEngine.Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft, color);
            UnityEngine.Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
            UnityEngine.Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
            UnityEngine.Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight, color);

            DrawBox(bottomBox, color);
            DrawBox(topBox, color);
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
            DrawBox(new Box(origin, halfExtents, orientation), color);
        }

        public static void DrawBox(Box box, Color color)
        {
            UnityEngine.Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
            UnityEngine.Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
            UnityEngine.Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
            UnityEngine.Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

            UnityEngine.Debug.DrawLine(box.backTopLeft, box.backTopRight, color);
            UnityEngine.Debug.DrawLine(box.backTopRight, box.backBottomRight, color);
            UnityEngine.Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color);
            UnityEngine.Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color);

            UnityEngine.Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color);
            UnityEngine.Debug.DrawLine(box.frontTopRight, box.backTopRight, color);
            UnityEngine.Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
            UnityEngine.Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color);
        }

        //This should work for all cast types
        private static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
        {
            return origin + direction.normalized * hitInfoDistance;
        }

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            var direction = point - pivot;
            return pivot + rotation * direction;
        }

        public struct Box
        {
            public Vector3 localFrontTopLeft { get; private set; }
            public Vector3 localFrontTopRight { get; private set; }
            public Vector3 localFrontBottomLeft { get; private set; }
            public Vector3 localFrontBottomRight { get; private set; }
            public Vector3 localBackTopLeft => -localFrontBottomRight;
            public Vector3 localBackTopRight => -localFrontBottomLeft;
            public Vector3 localBackBottomLeft => -localFrontTopRight;
            public Vector3 localBackBottomRight => -localFrontTopLeft;

            public Vector3 frontTopLeft => localFrontTopLeft + origin;
            public Vector3 frontTopRight => localFrontTopRight + origin;
            public Vector3 frontBottomLeft => localFrontBottomLeft + origin;
            public Vector3 frontBottomRight => localFrontBottomRight + origin;
            public Vector3 backTopLeft => localBackTopLeft + origin;
            public Vector3 backTopRight => localBackTopRight + origin;
            public Vector3 backBottomLeft => localBackBottomLeft + origin;
            public Vector3 backBottomRight => localBackBottomRight + origin;

            public Vector3 origin { get; }

            public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
            {
                Rotate(orientation);
            }

            public Box(Vector3 origin, Vector3 halfExtents)
            {
                localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
                localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
                localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
                localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

                this.origin = origin;
            }


            public void Rotate(Quaternion orientation)
            {
                localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
                localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
                localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
                localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
            }
        }
    }
}