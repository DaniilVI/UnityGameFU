using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float velocity;
    [SerializeField] private float minDistance;
    [SerializeField] private RectTransform canvasRect;
    private static float cameraHeight;
    private static float cameraWidth;

    void Start()
    {
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Screen.width / Screen.height;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Rect rect = canvasRect.rect;
        Vector3 minBounds = new Vector3(rect.min.x, rect.min.y, -10);
        Vector3 maxBounds = new Vector3(rect.max.x, rect.max.y, -10);
        minBounds = canvasRect.TransformPoint(minBounds);
        maxBounds = canvasRect.TransformPoint(maxBounds);

        // Раскомментировать на случай, если смещение по Y будет ненулевым
        //if (transform.position.y - minBounds.y > offset.y)
        //{
        //    offset.y = 0;
        //}

        var targetPos = target.transform.position + offset;

        if (Vector3.Distance(transform.position, targetPos) < minDistance)
        {
            return;
        }

        var newPos = Vector3.Lerp(transform.position, targetPos, velocity * Time.deltaTime);
        newPos.z = transform.position.z;
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y + cameraHeight, maxBounds.y - cameraHeight);
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x + cameraWidth, maxBounds.x - cameraWidth);

        transform.Translate(transform.InverseTransformPoint(newPos));
    }
}
