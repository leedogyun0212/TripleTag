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

    // 외부에서 호출하거나 autoUpdate가 켜져있으면 매 프레임 실행됩니다.
    //public void Vision(float detlaTime)
    //{
    //    UpdateFOVMesh();
    //}

    //void UpdateFOVMesh()
    //{
    //    if (fovMesh == null) return;
    //    int stepCount = Mathf.Max(1, meshResolution); // 분할수
    //    float stepAngleSize = fovAngle / stepCount; // 각도fovAngle을 stepCount만큼 나눔

    //    // 버텍스: origin + stepCount+1 points
    //    Vector3[] vertices = new Vector3[stepCount + 2]; // Mesh의 꼭짓점 배열 (원점 + 각 단계의 점들)
    //    Vector2[] uv = new Vector2[vertices.Length]; // UV 배열 (텍스처 매핑용, 필요에 따라 조정)
    //    int[] triangles = new int[(stepCount) * 3]; // 삼각형 배열 (각 단계마다 3개씩)

    //    Vector3 origin = Vector3.zero; // local space origin
    //    vertices[0] = origin;

    //    int triIndex  = 0;
    //    int vertIndex = 1; 

    //    // 각 단계마다 레이캐스트로 장애물 체크
    //    float halfFov = fovAngle * 0.5f; // 시야의 절반
    //    for (int i = 0; i <= stepCount; i++) // stepCount+1 만큼 반복하여 각 단계의 점 계산
    //    {
    //        float angle = -halfFov + stepAngleSize * i; // local forward 기준으로 왼쪽에서 오른쪽으로 각도 계산
    //        Vector3 dir = DirFromAngle(angle, true); // 캐릭터의 방향 계산 (로컬 기준)

    //        // 월드 위치/방향 계산
    //        Vector3 worldOrigin = transform.position; // 월드 위치는 캐릭터의 위치
    //        Vector3 worldDir = transform.TransformDirection(dir); // 방향은 캐릭터 회전에 따라 변환

    //        RaycastHit hit;
    //        Vector3 point;
    //        if (Physics.Raycast(worldOrigin, worldDir, out hit, viewDistance, obstacleMask)) // 레이캐스트를 쏴서 장애물(obstacleMask) 체크
    //        {
    //            // 장애물에 닿으면 그 지점까지 시야
    //            point = transform.InverseTransformPoint(hit.point); // 맞으면 맞은 곳 까지 시야
    //        }
    //        else
    //        {
    //            // 최대 거리까지
    //            point = transform.InverseTransformPoint(worldOrigin + worldDir * viewDistance); // 안 맞으면 최대 거리까지 시야
    //        }

    //        vertices[vertIndex] = point;
    //        uv[vertIndex] = new Vector2((float)vertIndex / vertices.Length, 0); // UV는 필요에 따라 조정 (여기서는 간단히 정규화된 값 사용)

    //        if (i > 0) // 첫 번째 점 이후부터 삼각형 생성 (원점, 이전 점, 현재 점) 
    //        {
    //            triangles[triIndex + 0] = 0;
    //            triangles[triIndex + 1] = vertIndex - 1;
    //            triangles[triIndex + 2] = vertIndex;
    //            triIndex += 3;
    //        }

    //        vertIndex++;
    //    }

    //    fovMesh.Clear();
    //    fovMesh.vertices = vertices; // 곡짓점 배열 설정 
    //    fovMesh.uv = uv; // UV 배열 설정 (텍스처 매핑용, 필요에 따라 조정)
    //    fovMesh.triangles = triangles; // 삼각형 연결
    //    fovMesh.RecalculateNormals();
    //}

    // angle: 도 단위. isGlobalAngle=true이면 월드 기준, false면 로컬(전방 기준)
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
