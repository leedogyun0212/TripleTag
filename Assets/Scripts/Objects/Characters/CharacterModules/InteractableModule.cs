using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableModule : CharacterModule
{
    public bool isPickUp = false;

    public sealed override System.Type RegistrationType => typeof(InteractableModule);

    CharacterBase targetCharTest;

    HitPointModule hitPointModule;

    bool isRespawn = false;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);
        hitPointModule  = GetComponent<HitPointModule>();
        MeshSetting();
        GameManager.OnUpdateCharacter -= Vision;
        GameManager.OnUpdateCharacter += Vision;
    }

    public override void OnUnregistration(CharacterBase oldOwner)
    {
        base.OnUnregistration(oldOwner);
        GameManager.OnUpdateCharacter -= Vision;
    }

    /// <summary> 부활 </summary>
    // 죽은 팀원을 5초동안 접촉하면 살린다
    // 죽어야한다(PlayerSet->Dead)
    // 5초동안 접촉한다 
    // 같은 팀원(PlayerGroup->나와 같음)이어야한다
    // 살린다 -> 죽은 시간동안 얻을 포인트의 절반을 얻는다ㅒ
    // CharacterBase target 얘를 언제 받냐?
    // 접촉이면  OnTrigger나 OnCollision을 쓸건데 그러면 GameObject는 쉽게 얻을수 있어도 CharacterBase는???
    // 그렇다고 접촉할때마다 CharacterBase얻을려고 겟컴포넌트는 좀.. 
    // 그래서 방법중 하나가 yield를 이용하는것이라 생각


    public IEnumerator Respawn(CharacterBase target)
    {
        if(target is null) yield return null;

        yield return new WaitForSecondsRealtime(5.0f);

        HitPointModule targetChar = target.GetComponentInParent<HitPointModule>();

        if (target.PlayerSet is PlayerSet.Dead)
        {
            if (target.PlayerGroup != Owner.PlayerGroup) yield return null;

            //부활기능
            targetChar.Respawn(); 
        }
        else
        {
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRespawn) return;

        CharacterBase target = other.GetComponent<CharacterBase>();

        if (target is null) return;
        isRespawn = false;

        StartCoroutine(Respawn(target));
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

    [Header("근거리 시야")]
    public float closeVisionRadius = 1f;
    public int closeVisionResolution = 24;

    MeshFilter closeMeshFilter;
    MeshRenderer closeMeshRenderer;
    Mesh closeMesh;

    Vector3 DirFromAngle(float angleInDegrees, bool isLocal)
    {
        float angleRad = angleInDegrees * Mathf.Deg2Rad;
        // 로컬 XZ 평면에서의 방향. Unity에서 전방(Z)이 0도라고 가정 
        Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
        return dir;
    }
    
    public void Vision(float deltaTime)
    {
        UpdateCloseVision();
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

    public void MeshSetting()
    {
        fovMeshFilter = GetComponentInChildren<MeshFilter>();
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

        if (fovMeshFilter.sharedMesh == null)
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

        GameObject closeVision = new GameObject("CloseVision_Mesh");
        closeVision.transform.SetParent(transform, false);
        closeVision.layer = LayerMask.NameToLayer("Vision");

        closeMeshFilter = closeVision.AddComponent<MeshFilter>();
        closeMeshRenderer = closeVision.AddComponent<MeshRenderer>();

        closeMeshRenderer.material = fovStencilMaterial;
        closeMeshRenderer.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;
        closeMeshRenderer.receiveShadows = false;

        closeMesh = new Mesh();
        closeMesh.name = "CloseVision_Mesh_Generated";

        closeMeshFilter.sharedMesh = closeMesh;
    }

    void UpdateCloseVision()
    {
        if (closeMesh == null) return;

        int segments = Mathf.Max(8, closeVisionResolution);

        Vector3[] vertices = new Vector3[segments + 1];
        Vector2[] uv = new Vector2[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * closeVisionRadius;
            float z = Mathf.Sin(angle) * closeVisionRadius;

            vertices[i + 1] = new Vector3(x, 0, z);

            uv[i + 1] = new Vector2(
                (x / closeVisionRadius + 1f) * 0.5f,
                (z / closeVisionRadius + 1f) * 0.5f
            );
        }
        //
        int triIndex = 0;

        for (int i = 0; i < segments; i++)
        {
            int current = i + 1;
            int next = (i + 1) % segments + 1;

            triangles[triIndex++] = 0;
            triangles[triIndex++] = next;
            triangles[triIndex++] = current;
        }

        closeMesh.Clear();

        closeMesh.vertices = vertices;
        closeMesh.uv = uv;
        closeMesh.triangles = triangles;

        closeMesh.RecalculateNormals();
    }
}