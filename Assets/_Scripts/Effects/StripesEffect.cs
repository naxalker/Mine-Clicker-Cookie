using DG.Tweening;
using UnityEngine;

public class StripesEffect : MonoBehaviour
{
    void Start()
    {
        GetComponent<RectTransform>()
            .DORotate(new Vector3(0, 0, 360), 60f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }
}
