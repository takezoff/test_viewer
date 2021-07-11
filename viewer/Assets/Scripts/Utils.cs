using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using x8;

namespace x8 {
//------------------------------------------------------------------------------

//==============================================================================
//
//  ユーティリティ
//
//==============================================================================

//------------------------------------------------------------------------------
//  ユーティリティ
//------------------------------------------------------------------------------
class Utils
{
    // ポインターイベントのイベントトリガエントリーを生成
    //  ※シングルタッチ対応も考慮する
    public static EventTrigger.Entry
        NewPointerEventTriggerEntry(EventTriggerType type,
                                    UnityAction<PointerEventData> callback,
                                    bool multiTouchEnabled = false)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((base_data) => {
                var data = (PointerEventData)base_data;
                // シングルタッチ対応（※有効なタッチ以外は無視）
                if (multiTouchEnabled || data.IsSinglePointerEvent()) {
                    callback(data);
                }
            });
        return entry;
    }

}   // Utils
    
//------------------------------------------------------------------------------
}   // namespace x8
