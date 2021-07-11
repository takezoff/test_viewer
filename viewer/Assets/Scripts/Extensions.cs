using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

//==============================================================================
//
//  エクステンションいろいろ
//
//==============================================================================

// Object .. ♪リフレクションは遅いかもなので要調査..
public static class ObjectExtensions
{
    private const string SEPARATOR = ",";       // 区切り記号として使用する文字列
    private const string FORMAT = "{0}:{1}";    // 複合書式指定文字列
 
    /// <summary>
    /// すべての公開フィールドの情報を文字列にして返します
    /// </summary>
    public static string ToStringFields<T>(this T obj)
    {
        return string
            .Join(SEPARATOR, obj
                  .GetType()
                  .GetFields(BindingFlags.Instance | BindingFlags.Public)
                  .Select(c => string.Format(FORMAT, c.Name, c.GetValue(obj))));
    }
    
    /// <summary>
    /// すべての公開プロパティの情報を文字列にして返します
    /// </summary>
    public static string ToStringProperties<T>(this T obj)
    {
        return string
            .Join(SEPARATOR, obj
                  .GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .Where(c => c.CanRead)
                  .Select(c => string.Format(FORMAT, c.Name, c.GetValue(obj, null))));
    }
    
    /// <summary>
    /// すべての公開フィールドと公開プロパティの情報を文字列にして返します
    /// </summary>
    public static string ToStringReflection<T>(this T obj)
    {
        return string
            .Join(SEPARATOR, 
                  obj.ToStringFields(), 
                  obj.ToStringProperties());
    }
}

// GameObject
public static class GameObjectExtensions
{
    // コンポーネント取得、なければ付加する
    public static T GetOrAddComponent<T>(this GameObject self) where T:Component
    {
        var cmp = self.GetComponent<T>();
        if (cmp != null) return cmp;
        else return self.AddComponent<T>();
    }

    // GetComponentInChildren()の自分を含まない版
    public static T GetComponentInChildrenWithoutSelf<T>(this GameObject self) where T:Component
    {
        foreach (Transform child in self.transform) {
            var cmp = child.GetComponentInChildren<T>();
            if (cmp != null) return cmp;
        }
        return null;
    }

    // GetComponentsInChildren()の自分を含まない版
    public static T[] GetComponentsInChildrenWithoutSelf<T>(this GameObject self, bool includeInactive) where T:Component
    {
        return self.GetComponentsInChildren<T>(includeInactive)
            .Where(e => self != e.gameObject).ToArray();
    }
}

// Component
public static class ComponentExtensions
{
    // コンポーネント取得、なければ付加する
    public static T GetOrAddComponent<T>(this Component self) where T:Component
    {
        var go = self.gameObject;
        if (go == null) return null;
        var cmp = go.GetComponent<T>();
        if (cmp != null) return cmp;
        else return go.AddComponent<T>();
    }

    // GetComponentInChildren()の自分を含まない版
    public static T GetComponentInChildrenWithoutSelf<T>(this Component self) where T:Component
    {
        foreach (Transform child in self.transform) {
            var cmp = child.GetComponentInChildren<T>();
            if (cmp != null) return cmp;
        }
        return null;
    }

    // GetComponentsInChildren()の自分を含まない版
    public static T[] GetComponentsInChildrenWithoutSelf<T>(this Component self, bool includeInactive) where T:Component
    {
        return self.GetComponentsInChildren<T>(includeInactive)
            .Where(e => self.gameObject != e.gameObject).ToArray();
    }
}

// Transform
public static class TransformExtensions
{
    // ヒエラルキーパスからゲームオブジェクトを取得
    public static GameObject GetChildObject(this Transform self, string path, GameObject root = null)
    {
        var from_tr = (root != null) ? root.transform : self;
        var tr = from_tr.Find(path);
        return (tr != null) ? tr.gameObject : null;
    }

    // ヒエラルキーパスからコンポーネントを取得
    public static T GetChildComponent<T>(this Transform self, string path, GameObject root = null) where T:Component
    {
        var obj = self.GetChildObject(path, root);
        return (obj != null) ? obj.GetComponent<T>() : null;
    }
}

// UI
public static class UIExtensions
{
    // Scrollbar／コールバックを呼ばずに値を設定
    static Scrollbar.ScrollEvent emptyScrollEvent = new Scrollbar.ScrollEvent();
    public static void SetValue(this Scrollbar instance, float value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyScrollEvent;
        instance.value = value;
        instance.onValueChanged = originalEvent;
    }

    // Scrollbar／コールバックを呼ばずにステップ数を設定
    public static void SetNumberOfSteps(this Scrollbar instance, int value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyScrollEvent;
        instance.numberOfSteps = value;
        instance.onValueChanged = originalEvent;
    }

    // Slider／コールバックを呼ばずに値を設定
    static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
    public static void SetValue(this Slider instance, float value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptySliderEvent;
        instance.value = value;
        instance.onValueChanged = originalEvent;
    }
 
    // Toggle／コールバックを呼ばずに値を設定
    static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
    public static void SetValue(this Toggle instance, bool value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyToggleEvent;
        instance.isOn = value;
        instance.onValueChanged = originalEvent;
    }
 
    // InputField／コールバックを呼ばずに値を設定
    static InputField.OnChangeEvent emptyInputFieldEvent = new InputField.OnChangeEvent();
    public static void SetValue(this InputField instance, string value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyInputFieldEvent;
        instance.text = value;
        instance.onValueChanged = originalEvent;
    }
 
    // Dropdown／コールバックを呼ばずに値を設定
    static Dropdown.DropdownEvent emptyDropdownFieldEvent = new Dropdown.DropdownEvent();
    public static void SetValue(this Dropdown instance, int value)
    {
        var originalEvent = instance.onValueChanged;
        instance.onValueChanged = emptyDropdownFieldEvent;
        instance.value = value;
        instance.onValueChanged = originalEvent;
    }
}

// Vector2Int
public static class Vector2IntExtensions
{
    // 矩形内にクランプ
    public static Vector2Int ClampToRect(this Vector2Int self, RectInt limit)
    {
        // ※Clamp()を使ってないのはlimitのサイズが0なら座標をMinにするため
        self.x = Math.Max(limit.xMin, Math.Min(limit.xMax - 1, self.x));
        self.y = Math.Max(limit.yMin, Math.Min(limit.yMax - 1, self.y));
        return self;
    }
}

// RectInt
public static class RectIntExtensions
{
    // width, heigtのうちどちらか一方でもゼロ？
    public static bool IsSizeZero(this RectInt self)
    {
        return (self.width == 0) || (self.height == 0);
    }

    // width, heigtが両方ともゼロより大きい？
    public static bool IsSizePositive(this RectInt self)
    {
        return (self.width > 0) && (self.height > 0);
    }

    // width, heigtが両方ともゼロより小さい？
    public static bool IsSizeNegative(this RectInt self)
    {
        return (self.width < 0) && (self.height < 0);
    }

    // サイズ(width, height)が正になるように位置(x, y)を矯正する
    //  ※構造体の場合のthisの参照渡しはC#7.2以降なので使えない..
    public static RectInt CorrectPosition(this RectInt self)
    {
        self.SetMinMax(self.min, self.max);
        return self;
    }

    // 値同値？
    //  ※パラメータ違いの同じ矩形は考慮していないので、 必要なら
    //    CorrectPosition() などで揃えてからチェックしてね
    public static bool ValueEquals(this RectInt self, RectInt other)
    {
        return (self.x == other.x) && (self.y == other.y)
            && (self.width == other.width) && (self.height == other.height);
    }

    // 正方形に矯正する
    //  ※(x, y)を固定し、(width, height)で調整する
    //  ・small .. true:小さい辺に合わせる／false:大きい辺に合わせる
    public static RectInt CorrectToSquare(this RectInt self, bool small = false)
    {
        var abs_w = Math.Abs(self.width);
        var abs_h = Math.Abs(self.height);
        var use_width = small ? (abs_w <= abs_h) : (abs_w >= abs_h);
        if (use_width) {
            return new RectInt(new Vector2Int(self.x, self.y), new Vector2Int(self.width, abs_w * ((self.height >= 0) ? 1 : -1)));
        }
        else {
            return new RectInt(new Vector2Int(self.x, self.y), new Vector2Int(abs_h * ((self.width >= 0) ? 1 : -1), self.height));
        }
    }

    // 矩形内にクランプ
    //  ※ClampToBounds()の挙動がいまいちだったので..
    public static RectInt ClampToRect(this RectInt self, RectInt limit)
    {
        //Debug.LogFormat("【Clamp】self{0}, min{1}, max{2}", self, self.min, self.max);
        //Debug.LogFormat("【Clamp】limit{0}, min{1}, max{2}", limit, limit.min, limit.max);
        self.xMin = Math.Max(self.xMin, limit.xMin);
        self.yMin = Math.Max(self.yMin, limit.yMin);
        self.xMax = Math.Min(self.xMax, limit.xMax);
        self.yMax = Math.Min(self.yMax, limit.yMax);
        //Debug.LogFormat("【Clamp】rect{0}, min{1}, max{2}", self, self.min, self.max);
        return self;
    }

    // 矩形内に収まるよう位置だけで調整
    //  ※サイズは変更しない
    //  ※はみ出る場合はサイズ方向にはみ出る
    public static RectInt ClampPositionToRect(this RectInt self, RectInt limit)
    {
        //Debug.LogFormat("【ClampPosition】self{0}, min{1}, max{2}", self, self.min, self.max);
        //Debug.LogFormat("【ClampPosition】limit{0}, min{1}, max{2}", limit, limit.min, limit.max);
        // 先にMaxの方を調整
        if (self.xMax > limit.xMax) {
            self.x -= self.xMax - limit.xMax;
        }
        if (self.yMax > limit.yMax) {
            self.y -= self.yMax - limit.yMax;
        }
        // 後にMinの方を調整（Minの調整を優位にさせるため）
        if (self.xMin < limit.xMin) {
            self.x += limit.xMin - self.xMin;
        }
        if (self.yMin < limit.yMin) {
            self.y += limit.yMin - self.yMin;
        }
        //Debug.LogFormat("【ClampPosition】rect{0}, min{1}, max{2}", self, self.min, self.max);
        return self;
    }

    // サイズ内にクランプ
    //  ※サイズの方向を維持する
    public static RectInt ClampToSize(this RectInt self, Vector2Int limit)
    {
        var w = Mathf.Abs(limit.x);
        var h = Mathf.Abs(limit.y);
        self.width = (self.width >= 0)
            ? Mathf.Clamp(self.width, 0, w) : Mathf.Clamp(self.width, -w, 0);
        self.height = (self.height >= 0)
            ? Mathf.Clamp(self.height, 0, h) : Mathf.Clamp(self.height, -h, 0);
        return self;
    }

    // 指定した位置を含むように拡大
    public static RectInt Encapsulate(this RectInt self, Vector2Int point)
    {
        if (point.x < self.xMin) self.xMin = point.x;
        if (point.y < self.yMin) self.yMin = point.y;
        if (point.x >= self.xMax) self.xMax = point.x + 1;
        if (point.y >= self.yMax) self.yMax = point.y + 1;
        return self;
    }

    // 指定した矩形を含むように拡大
    public static RectInt Encapsulate(this RectInt self, RectInt rect)
    {
        if (rect.xMin < self.xMin) self.xMin = rect.xMin;
        if (rect.yMin < self.yMin) self.yMin = rect.yMin;
        if (rect.xMax > self.xMax) self.xMax = rect.xMax;
        if (rect.yMax > self.yMax) self.yMax = rect.yMax;
        return self;
    }

#if false   // この関数は不完全、使うなら要修正、意外と面倒..
    // 矩形内にクランプしつつ正方形に矯正する
    //  ※(x, y)を固定し、(width, height)で調整する
    //  ・small .. true:小さい辺に合わせる／false:大きい辺に合わせる
    public static RectInt ClampAndCorrectToSquare(this RectInt self, RectInt limit, bool small = false)
    {
        Debug.LogFormat("【self 】rect{0}, min{1}, max{2}", self, self.min, self.max);
        Debug.LogFormat("【limit】rect{0}, min{1}, max{2}", limit, limit.min, limit.max);

        var sign_w = ((self.width >= 0) ? 1 : -1);
        var sign_h = ((self.height >= 0) ? 1 : -1);

        var abs_w = Math.Abs(self.width);
        var abs_h = Math.Abs(self.height);
        var use_width = small ? (abs_w <= abs_h) : (abs_w >= abs_h);
        if (use_width) {

            var x1 = Math.Max(limit.xMin, Math.Min(limit.xMax, self.x));
            var x2 = self.x + self.width;
            x2 = Math.Max(limit.xMin, Math.Min(limit.xMax, x2));
            var safe_abs_w = Math.Abs(x2 - x1);

            var y1 = Math.Max(limit.yMin, Math.Min(limit.yMax, self.y));
            var y2 = self.y + (safe_abs_w * sign_h);
            y2 = Math.Max(limit.yMin, Math.Min(limit.yMax, y2));
            var safe_abs_h = Math.Abs(y2 - y1);
            
            var safe_abs_len = (safe_abs_w <= safe_abs_h) ? safe_abs_w : safe_abs_h;
            var rect = new RectInt(new Vector2Int(x1, y1),
                                   new Vector2Int(safe_abs_len * sign_w, safe_abs_len * sign_h));

            Debug.LogFormat("【use w】rect{0}, min{1}, max{2}", rect, rect.min, rect.max);
            return rect;
        }
        else {

            var y1 = Math.Max(limit.yMin, Math.Min(limit.yMax, self.y));
            var y2 = self.y + self.height;
            y2 = Math.Max(limit.yMin, Math.Min(limit.yMax, y2));
            var safe_abs_h = Math.Abs(y2 - y1);

            var x1 = Math.Max(limit.xMin, Math.Min(limit.xMax, self.x));
            var x2 = self.x + (safe_abs_h * sign_w);
            x2 = Math.Max(limit.xMin, Math.Min(limit.xMax, x2));
            var safe_abs_w = Math.Abs(x2 - x1);

            var safe_abs_len = (safe_abs_w <= safe_abs_h) ? safe_abs_w : safe_abs_h;
            var rect = new RectInt(new Vector2Int(x1, y1),
                                   new Vector2Int(safe_abs_len * sign_w, safe_abs_len * sign_h));

            Debug.LogFormat("【use h】rect{0}, min{1}, max{2}", rect, rect.min, rect.max);
            return rect;
        }
    }
#endif
}

// Color
public static class ColorExtensions
{
    // カラーのα値だけをいじって返す
    public static Color TweakAlpha(this Color self, float a)
    {
        self.a = a;
        return self;
    }

    // カラーのr,g,b値だけをいじって返す
    public static Color TweakRGB(this Color self, Color src)
    {
        self.r = src.r;
        self.g = src.g;
        self.b = src.b;
        return self;
    }
}

// PointerEventData
public static class PointerEventDataExtensions
{
    // シングルタッチモードで対応するイベント？
    public static bool IsSinglePointerEvent(this PointerEventData self)
    {
#if UNITY_EDITOR
        // ※UnityRemoteを利用するならタッチも取らないとだよ..
        return self.pointerId == -1;    // マウスの左ボタン
#else
        // ♪マウスが使えるデバイスに対応するならここを修正する必要あり
        //      - タッチかマウスのどちらかを選ぶ仕様だと楽かな..
        return self.pointerId == 0;     // ID:0のタッチ
#endif
    }
}

