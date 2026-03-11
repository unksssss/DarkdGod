// time:2025/12/30/16
// path:E:\unityproject\DarkGod\Assets\Script\Common\FanShapeGizmo.csFanShapeGizmo.cs
// func:绘制扇形区域
// www34

using UnityEngine;

[ExecuteInEditMode] // 编辑器模式下也能显示
public class FanShapeGizmo : MonoBehaviour
{
    [Header("扇形参数")]
    public float fanRadius = 4.5f; // 扇形半径
    public float fanAngle = 60f; // 扇形角度（60度）
    public Color gizmoColor = new Color(1, 0, 0, 0.3f); // 扇形颜色（带透明度）

    private void OnDrawGizmos()
    {
        
        DrawFanShape(transform.position, transform.forward, fanRadius, fanAngle, gizmoColor);
    }

    /// <summary>
    /// 绘制扇形
    /// </summary>
    /// <param name="center">扇形中心点（物体位置）</param>
    /// <param name="forwardDir">扇形朝向（物体前方向）</param>
    /// <param name="radius">半径</param>
    /// <param name="angle">扇形角度（度数）</param>
    /// <param name="color">颜色</param>
    private void DrawFanShape(Vector3 center, Vector3 forwardDir, float radius, float angle, Color color)
    {
        Gizmos.color = color;
        
        // 1. 计算扇形左右边界的方向向量（转弧度）
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
        Quaternion leftRot = Quaternion.Euler(0, -halfAngle * Mathf.Rad2Deg, 0);
        Quaternion rightRot = Quaternion.Euler(0, halfAngle * Mathf.Rad2Deg, 0);
        Vector3 leftDir = leftRot * forwardDir;
        Vector3 rightDir = rightRot * forwardDir;

        // 2. 绘制扇形的两条边（中心点→左边界、中心点→右边界）
        Gizmos.DrawLine(center, center + leftDir * radius);
        Gizmos.DrawLine(center, center + rightDir * radius);

        // 3. 绘制扇形的弧形轮廓（用多个线段近似圆弧）
        int segmentCount = 30; // 分段数（越多越平滑）
        Vector3 prevPoint = center + leftDir * radius;
        for (int i = 1; i <= segmentCount; i++)
        {
            float currentAngle = -halfAngle + (i * angle * Mathf.Deg2Rad / segmentCount);
            Quaternion rot = Quaternion.Euler(0, currentAngle * Mathf.Rad2Deg, 0);
            Vector3 currentDir = rot * forwardDir;
            Vector3 currentPoint = center + currentDir * radius;
            
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }

        // 4. 可选：绘制扇形填充（用三角形面片近似）
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Begin(GL.TRIANGLES);
        GL.Color(new Color(color.r, color.g, color.b, color.a * 0.2f)); // 填充色更透明
        for (int i = 0; i < segmentCount; i++)
        {
            float angle1 = -halfAngle + (i * angle * Mathf.Deg2Rad / segmentCount);
            float angle2 = -halfAngle + ((i + 1) * angle * Mathf.Deg2Rad / segmentCount);
            Vector3 dir1 = Quaternion.Euler(0, angle1 * Mathf.Rad2Deg, 0) * forwardDir;
            Vector3 dir2 = Quaternion.Euler(0, angle2 * Mathf.Rad2Deg, 0) * forwardDir;
            
            GL.Vertex(center);
            GL.Vertex(center + dir1 * radius);
            GL.Vertex(center + dir2 * radius);
        }
        GL.End();
        GL.PopMatrix();
    }
}