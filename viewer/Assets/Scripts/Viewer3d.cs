using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer3d : MonoBehaviour
{
    public Transform modelPivot;

    public Camera modelCamera;
    public Vector3 moveTargetPos;
    public Vector3 scaleCenterPos;  // モデルのローカル座標

    public float dragZoomRate = 0.01f;

    public float orgFov;
    public float targetZoom;
    public float Zoom { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        orgFov = modelCamera.fieldOfView;
        Zoom = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

        // スケールセンターの画面内の位置が変化しないように補正

        // ズーム変更前のスケールセンターの画面位置を算出
        var scSrcWorldPos = modelPivot.TransformPoint(scaleCenterPos);
        var scSrcScreenPos = RectTransformUtility.WorldToScreenPoint(modelCamera, scSrcWorldPos);

        // ズーム更新
        modelCamera.fieldOfView = orgFov / Zoom;

        // ズーム変更後にズーム変更前のスケールセンターのワールド座標を算出
        var scDstWorldPos = modelCamera.ScreenToWorldPoint(new Vector3(
            scSrcScreenPos.x,
            scSrcScreenPos.y,
            scSrcWorldPos.z - modelCamera.transform.position.z
            ));

        // 画面位置のズレをワールド座標に換算した値
        var scWorldMove = new Vector3(
            scDstWorldPos.x - scSrcWorldPos.x,
            scDstWorldPos.y - scSrcWorldPos.y,
            0.0f);

        Debug.LogFormat($"scWorldMove:{scWorldMove}");

        modelPivot.position = modelPivot.position + scWorldMove;
    }

    public void Reset()
    {
        Zoom = 1.0f;
        targetZoom = 1.0f;
        scaleCenterPos = Vector3.zero;
        modelPivot.localPosition = Vector3.zero;
    }

    //
    public void SetZoomByDrag(Vector2 delta)
    {
        Zoom = Mathf.Clamp(Zoom + delta.y * dragZoomRate, 0.5f, 3.0f)   ;
    }

    // スケールのセンター位置を設定
    public bool SetScaleCenter(Vector2 screenPos)
    {
        var ray = modelCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // ヒット位置をモデルのローカル座標で記録
            //  ※これはモデルが動いていない前提なことに注意！
            scaleCenterPos = modelPivot.InverseTransformPoint(hit.point);
            return true;
        }
        return false;
    }
}
