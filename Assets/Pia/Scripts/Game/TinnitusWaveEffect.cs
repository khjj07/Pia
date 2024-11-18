using UnityEngine;
using DG.Tweening;
using UnityEngine.UI.Extensions; // DoTween ���ӽ����̽� �߰�

[RequireComponent(typeof(UILineRenderer))]
public class TinnitusWaveEffect : MonoBehaviour
{
    public int sampleCount = 32;       // ������ ���е�
    public float waveformWidth = 15.0f; // ������ ���� ����
    public float heightMultiplier = 0f; // ������ �ʱ� ���� ���� ��

    private UILineRenderer lineRenderer;
    private float timeOffset;
    private float distortion; // X�� �ְ�

    public float waveSpeed = 3.0f;      // �ְ� �ӵ� ����
    public float shakeDuration = 4.0f;
    public float shakeStrength = 50;

    void OnEnable()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        lineRenderer.Points = new Vector2[sampleCount];

        // Sequence�� ����Ͽ� ���� �ִϸ��̼��� ���������� ����
        Sequence seq = DOTween.Sequence();

        // 0~2�� ���� ���̵� ��
        seq.Append(DOTween.To(() => distortion, x => distortion = x, 100f, shakeDuration/2)
            .SetEase(Ease.InOutQuad));

        // 2~6�� ���� 0.43~1.43���� ����/���� �ݺ�
        seq.Append(DOTween.To(() => distortion, x => distortion = x, 0f, shakeDuration/2)
            .SetEase(Ease.InOutQuad));

    }

    void Update()
    {

        timeOffset += Time.deltaTime * waveSpeed;

        for (int i = 0; i < sampleCount; i++)
        {
            // ���� �������� x, y ��ǥ ���
            float x = (i - sampleCount / 2) * waveformWidth / sampleCount;
            float y = Mathf.Sin(x * 5 + timeOffset) * heightMultiplier;
            y += Random.Range(-distortion, distortion); // �ұ�Ģ���� �߰��� �ְ� ȿ��

            // ���� ��ǥ���� ������ �� ����Ʈ�� ������Ʈ�� ��ġ�� ȸ���� �ݿ��Ͽ� ���� ��ǥ�� ��ȯ
            Vector3 localPosition = new Vector3(x, y, 0);
           
            lineRenderer.Points[i] = localPosition;
        }
        lineRenderer.SetAllDirty();
    }
}
