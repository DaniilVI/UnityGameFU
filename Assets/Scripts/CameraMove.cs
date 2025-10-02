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

        var newPos = Vector3.Lerp(transform.position, targetPos, velocity * Time.fixedDeltaTime); // интерполяция
        newPos.z = transform.position.z;
        
        // Можно удалить эту тупую проверку
        if (maxBounds.y - minBounds.y < cameraHeight * 2)
        {
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y + cameraHeight, float.PositiveInfinity);
        }
        else
        {
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y + cameraHeight, maxBounds.y - cameraHeight);
        }

        // Дёргается при отходе камеры от края сцены
        //if (IsOutsideBounds(newPos, minBounds, maxBounds))
        //{
        //    newPos.x = transform.position.x;
        //    Debug.Log("+");
        //}

        // Нет дёрганья (любая оптимизация этой конструкции приведёт к обратному)
        float tmpX = newPos.x;
        float smoothingCoeff = 10f;

        if (IsOutsideBounds(newPos, minBounds, maxBounds))
        {
            tmpX = Mathf.Lerp(transform.position.x, Mathf.Clamp(newPos.x, minBounds.x + cameraWidth + offset.x, maxBounds.x - cameraWidth - offset.x), smoothingCoeff * Time.deltaTime);
        }

        newPos.x = tmpX;

        transform.Translate(transform.InverseTransformPoint(newPos));
    }

    bool IsOutsideBounds(Vector3 pos, Vector3 minBounds, Vector3 maxBounds)
    {
        return minBounds.x + cameraWidth + offset.x > pos.x || maxBounds.x - cameraWidth - offset.x < pos.x;
    }
}
