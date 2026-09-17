using UnityEngine;
using Unity.Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
public class CameraTest : CinemachineExtension
{
    [Header("카메라 높이 설정")]
    [Tooltip("평지에서의 기본 카메라 Y 높이")]
    public float defaultCameraY = 8.0f;

    [Tooltip("평지 기준 캐릭터 Head(머리)의 Y 높이")]
    public float baseTargetY = 1.0f;

    [Header("부드러운 추적")]
    [Tooltip("높이가 변할 때 카메라가 따라가는 속도 (값이 크면 부드러움)")]
    public float yDamping = 2.0f;

    private float currentCameraY;
    private float lastGroundedCameraY; // 💡 점프 직전(바닥에 있을 때)의 카메라 Y 높이를 저장할 변수
    private MovementModule playerMovement;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentCameraY = defaultCameraY;
        lastGroundedCameraY = defaultCameraY;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            float targetY = defaultCameraY;

            if (vcam.Follow != null)
            {
                // 1. 플레이어 MovementModule 캐싱 (최초 1회 또는 플레이어 오브젝트가 바뀌었을 때)
                if (playerMovement == null || playerMovement.transform != vcam.Follow.root)
                {
                    playerMovement = vcam.Follow.GetComponentInParent<MovementModule>();
                }

                float currentHeadY = vcam.Follow.position.y;

                // 2. 작성해주신 메서드 IsjumpCheck() 사용
                bool isJumping = (playerMovement != null) && playerMovement.IsjumpCheck();

                // 3. 점프 중이 아닐 때(!isJumping) -> 바닥에 서서 이동 중인 상태
                if (!isJumping)
                {
                    // 평지 기준(baseTargetY)보다 지형이 높은 곳에 올라와 있다면 차이만큼 카메라 Y 상승
                    if (currentHeadY > baseTargetY)
                    {
                        float heightDifference = currentHeadY - baseTargetY;
                        targetY = defaultCameraY + heightDifference;
                    }
                    else
                    {
                        targetY = defaultCameraY;
                    }

                    // 💡 점프 직전 '현재 바닥 높이 기준 목표 Y값'을 변수에 저장해둡니다.
                    lastGroundedCameraY = targetY;
                }
                else
                {
                    // 💡 점프 중(isJumping == true)일 때 -> 계산을 새로 하지 않고 '점프 직전 저장한 Y값'을 유지!
                    targetY = lastGroundedCameraY;
                }
            }

            // 4. Y 높이 이동을 부드럽게 연결 (Lerp)
            if (Application.isPlaying && yDamping > 0)
            {
                currentCameraY = Mathf.Lerp(currentCameraY, targetY, deltaTime * (10.0f / yDamping));
            }
            else
            {
                currentCameraY = targetY;
            }

            // 5. 계산된 Y값 강제 적용
            Vector3 pos = state.RawPosition;
            pos.y = currentCameraY;
            state.RawPosition = pos;
        }
    }
}