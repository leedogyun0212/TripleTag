using Unity.VisualScripting;
using UnityEngine;

public class InteractableModule : CharacterModule
{
    public bool isPickUp = false;

    public sealed override System.Type RegistrationType => typeof(InteractableModule);

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        MeshSetting();
        GameManager.OnUpdateCharacter -= Vision;
        GameManager.OnUpdateCharacter += Vision;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnUpdateCharacter -= Vision;
    }

    /// <summary> 줍기 </summary>
    //부활 표식 줍기
    public void PickUp()
    {

    }

    /// <summary> 부활 </summary>
    // 주운 부활 표식으로 부활장소로 이동하면 활성화 하는 기능
    public void Respawn()
    {

    }

    /// <summary> 시야 </summary>

    // 간단한 FOV Mesh 생성기:
    // - 원점(origin)에서 지정 각도(fovAngle)와 거리(viewDistance)만큼의 부채꼴을 메쉬로 생성합니다.
    // - meshResolution으로 분할 수를 정해 매끄럽게 만듭니다.
    // - mask 레이어에 충돌(장애물)이 있으면 해당 지점까지 시야를 줄입니다 (Raycast 사용).
    // 사용법:
    //  - Inspector에서 fovAngle, viewDistance, mask, meshResolution을 조정.
    //  - autoUpdate를 켜면 매 프레임 시야를 갱신합니다.
    //  - 외부에서 수동 갱신하려면 Vision()을 호출하세요.
    [Header("FOV 설정")]
    public float fovAngle = 90f; // 시야 각도(도)
    public float viewDistance = 5f; // 시야 거리
    public int meshResolution = 30; // 분할 수 (높을수록 부드러움)
    public LayerMask obstacleMask; // 시야를 막는 레이어
    public bool autoUpdate = true; // 매 프레임 갱신 여부

    MeshFilter fovMeshFilter;
    MeshRenderer fovMeshRenderer;
    Mesh fovMesh;
    [SerializeField] Material fovStencilMaterial;

    Vector3 DirFromAngle(float angleInDegrees, bool isLocal)
    {
        float angleRad = angleInDegrees * Mathf.Deg2Rad;
        // 로컬 XZ 평면에서의 방향. Unity에서 전방(Z)이 0도라고 가정 
        Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
        return dir;
    }

    public void MeshSetting()
    {
        fovMeshFilter  = GetComponentInChildren<MeshFilter>();
        fovMeshRenderer = GetComponentInChildren<MeshRenderer>();

        if (fovMeshFilter == null)
        {
            GameObject fov = new GameObject("FOV_Mesh");
            fov.transform.SetParent(transform, false);
            fov.layer = LayerMask.NameToLayer("Vision");

            fovMeshFilter = fov.AddComponent<MeshFilter>();
            fovMeshRenderer = fov.AddComponent<MeshRenderer>();
            fovMeshRenderer.material = fovStencilMaterial;
            fovMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            fovMeshRenderer.receiveShadows = false;
        }

        if(fovMeshFilter.sharedMesh == null)
        {
            fovMesh = new Mesh();
            fovMesh.name = "FOV_Mesh_Generated";
            fovMeshFilter.sharedMesh = fovMesh;
        }
        else
        {
            fovMesh = fovMeshFilter.sharedMesh;
            fovMesh.Clear();
        }
    }

    public void Vision(float deltaTime)
    {
        UpdateVision();
    }

    void UpdateVision()
    {
        if (fovMesh == null) return;
        int stepCount = Mathf.Max(1, meshResolution);
        float stepAngleSize = fovAngle / stepCount;
        float halfFov = fovAngle * 0.5f;

        Vector3[] vertices = new Vector3[stepCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(stepCount) * 3];

        vertices[0] = Vector3.zero;

        int triIndex = 0;
        int vertIndex = 1;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -halfFov + stepAngleSize * i;
            Vector3 dir = DirFromAngle(angle, true);

            Vector3 worldOrigin = transform.position;
            Vector3 worldDir = transform.TransformDirection(dir);

            RaycastHit hit;
            Vector3 point;
            if (Physics.Raycast(worldOrigin, worldDir, out hit, viewDistance, obstacleMask))
            {
                point = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                point = transform.InverseTransformPoint(worldOrigin + worldDir * viewDistance);

            }

            vertices[vertIndex] = point;
            uv[vertIndex] = new Vector2((float)vertIndex / vertices.Length, 0);

            if(i>0)
            {
                triangles[triIndex + 0] = 0;
                triangles[triIndex + 1] = vertIndex - 1;
                triangles[triIndex + 2] = vertIndex;
                triIndex += 3;
            }

            vertIndex++;
        }


        fovMesh.Clear();
        fovMesh.vertices = vertices; // 곡짓점 배열 설정 
        fovMesh.uv = uv; // UV 배열 설정 (텍스처 매핑용, 필요에 따라 조정)
        fovMesh.triangles = triangles; // 삼각형 연결
        fovMesh.RecalculateNormals();
    }
}
