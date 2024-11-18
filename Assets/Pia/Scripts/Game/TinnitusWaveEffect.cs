using UnityEngine;
using DG.Tweening;
using UnityEngine.UI.Extensions; // DoTween 네임스페이스 추가

[RequireComponent(typeof(UILineRenderer))]
public class TinnitusWaveEffect : MonoBehaviour
{
    public int sampleCount = 32;       // 파형의 세밀도
    public float waveformWidth = 15.0f; // 파형의 가로 길이
    public float heightMultiplier = 0f; // 파형의 초기 높이 조절 값

    private UILineRenderer lineRenderer;
    private float timeOffset;
    private float distortion; // X축 왜곡

    public float waveSpeed = 3.0f;      // 왜곡 속도 조절
    public float shakeDuration = 4.0f;
    public float shakeStrength = 50;

    void OnEnable()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        lineRenderer.Points = new Vector2[sampleCount];

        // Sequence를 사용하여 여러 애니메이션을 순차적으로 실행
        Sequence seq = DOTween.Sequence();

        // 0~2초 동안 페이드 인
        seq.Append(DOTween.To(() => distortion, x => distortion = x, 100f, shakeDuration/2)
            .SetEase(Ease.InOutQuad));

        // 2~6초 동안 0.43~1.43으로 증가/감소 반복
        seq.Append(DOTween.To(() => distortion, x => distortion = x, 0f, shakeDuration/2)
            .SetEase(Ease.InOutQuad));

    }

    void Update()
    {

        timeOffset += Time.deltaTime * waveSpeed;

        for (int i = 0; i < sampleCount; i++)
        {
            // 로컬 공간에서 x, y 좌표 계산
            float x = (i - sampleCount / 2) * waveformWidth / sampleCount;
            float y = Mathf.Sin(x * 5 + timeOffset) * heightMultiplier;
            y += Random.Range(-distortion, distortion); // 불규칙성을 추가해 왜곡 효과

            // 로컬 좌표에서 생성한 각 포인트를 오브젝트의 위치와 회전을 반영하여 월드 좌표로 변환
            Vector3 localPosition = new Vector3(x, y, 0);
           
            lineRenderer.Points[i] = localPosition;
        }
        lineRenderer.SetAllDirty();
    }
}
