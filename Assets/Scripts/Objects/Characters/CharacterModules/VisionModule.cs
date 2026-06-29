using UnityEngine;

public class VisionModule : MonoBehaviour
{

    [Header("References")]
    public Transform playerTransform;     // 플레이어 Transform
    public Material fogMaterial;          // 셰이더로 만든 머티리얼

    [Header("FOV Settings")]
    public float viewDistance = 15f;      // 시야 거리
    [Range(0f, 360f)]
    public float viewAngle = 90f;         // 시야각 (부채꼴 각도)
    public Color fogColor = Color.black;  // 안개 색상

    private void OnEnable()
    {
        GameManager.OnUpdateObject -= UpdateFog;
        GameManager.OnUpdateObject += UpdateFog;
    }

    private void OnDisable()
    {
        GameManager.OnUpdateObject -= UpdateFog;
    }

    void UpdateFog(float deltaTime)
    {
        if (playerTransform == null || fogMaterial == null) return;
        //123456815
        // 1. 플레이어 위치 및 정면 방향 전달
        fogMaterial.SetVector("_PlayerPos", playerTransform.position);
        fogMaterial.SetVector("_PlayerDir", playerTransform.forward);

        // 2. 최대 시야 거리 전달
        fogMaterial.SetFloat("_MaxDistance", viewDistance);

        // 3. 셰이더 내적 연산용 호도각(Cos) 계산 후 전달
        float halfAngleInRadians = (viewAngle * 0.5f) * Mathf.Deg2Rad;
        float cosHalfAngle = Mathf.Cos(halfAngleInRadians);
        fogMaterial.SetFloat("_CosHalfAngle", cosHalfAngle);

        // 4. 안개 색상 전달
        fogMaterial.SetColor("_FogColor", fogColor);
    }
}
