using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform player;  // 玩家的Transform组件
    private Camera cam;
    private Vector3 cameraOffset;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("请将玩家对象拖拽到SimpleClassicCameraController的Player字段中！");
            return;
        }

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("此脚本必须附加到Camera对象上！");
            return;
        }

        // 计算相机偏移量（假设相机初始位置正确）
        cameraOffset = transform.position - player.position;
        cameraOffset.z = transform.position.z; // 保持z轴不变
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 viewportPosition = cam.WorldToViewportPoint(player.position);

        if (viewportPosition.x < 0)
        {
            MoveCamera(Vector3.left);
        }
        else if (viewportPosition.x > 1)
        {
            MoveCamera(Vector3.right);
        }

        if (viewportPosition.y < 0)
        {
            MoveCamera(Vector3.down);
        }
        else if (viewportPosition.y > 1)
        {
            MoveCamera(Vector3.up);
        }
    }

    private void MoveCamera(Vector3 direction)
    {
        Vector3 movement = new Vector3(
            direction.x * cam.orthographicSize * 2 * cam.aspect,
            direction.y * cam.orthographicSize * 2,
            0
        );

        transform.position += movement;
    }
}
