using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using x8;

public class Viewer : MonoBehaviour
{
    public Viewer3d viewer3d;
    public bool isScaleStarted;
    public Vector2 zoomDelta;
    public Vector2 rotateDelta;
    public Image inputPanel;

    // Start is called before the first frame update
    void Start()
    {
        var event_trig = inputPanel.GetOrAddComponent<EventTrigger>();
        // PointerDown
        event_trig.triggers.Add(Utils.NewPointerEventTriggerEntry(
            EventTriggerType.PointerDown,
            (data) =>
            {
                if (Input.GetMouseButton(1)) {
                    isScaleStarted = viewer3d.SetScaleCenter(data.position);
                }
            }
            ));
        // Drag
        event_trig.triggers.Add(Utils.NewPointerEventTriggerEntry(
            EventTriggerType.Drag,
            (data) =>
            {
                if (isScaleStarted)
                {
                    viewer3d.SetZoomByDrag(data.delta);
                }
                else
                {
                    viewer3d.RotateByDrag(data.delta);
                }
            }
            ));
        // PointerUp
        //  ※EndDragだとドラッグが始まらな場合に取りこぼすことに注意！！
        event_trig.triggers.Add(Utils.NewPointerEventTriggerEntry(
            EventTriggerType.PointerUp,
            (data) =>
            {
                isScaleStarted = false;
            }
            ));
    }

    // Update is called once per frame
    void Update()
    {
        // 中クリックでリセット
        if (Input.GetMouseButtonDown(2))
        {
            viewer3d.Reset();
        }
    }
}
