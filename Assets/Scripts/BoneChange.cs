using System.Collections.Generic;
using UnityEngine;

public class BoneChange : MonoBehaviour
{
    [Header("기준이 되는 캐릭터 몸")]
    public SkinnedMeshRenderer targetRenderer;

    [Header("인스펙터에서 할당한 옷 오브젝트들")]
    public List<GameObject> clothes = new List<GameObject>();

    void Start()
    {
        SyncAllClothes();
    }

    [ContextMenu("Sync Bones Now")] // 인스펙터 우클릭으로 실행 가능
    public void SyncAllClothes()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("Target Renderer(몸)가 설정되지 않았습니다!");
            return;
        }

        // 1. 몸의 뼈 정보를 이름 기반으로 딕셔너리에 저장
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetRenderer.bones)
        {
            if (bone != null) boneMap[bone.name] = bone;
        }

        // 2. 리스트에 있는 각 옷마다 뼈 리매핑 진행
        foreach (GameObject clothingObj in clothes)
        {
            if (clothingObj == null) continue;

            // 자식에 있는 SkinnedMeshRenderer를 모두 찾음 (여러 파츠일 수 있음)
            SkinnedMeshRenderer[] smrs = clothingObj.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var smr in smrs)
            {
                RemapBones(smr, boneMap);
            }
        }
    }

    private void RemapBones(SkinnedMeshRenderer clothingRenderer, Dictionary<string, Transform> boneMap)
    {
        Transform[] newBones = new Transform[clothingRenderer.bones.Length];

        for (int i = 0; i < clothingRenderer.bones.Length; i++)
        {
            string boneName = clothingRenderer.bones[i].name;

            if (boneMap.TryGetValue(boneName, out Transform targetBone))
            {
                newBones[i] = targetBone;
            }
            else
            {
                Debug.LogWarning($"{clothingRenderer.name}의 뼈 '{boneName}'을 몸에서 찾을 수 없습니다.");
            }
        }

        // 실제 뼈 연결 및 루트 뼈 설정
        clothingRenderer.bones = newBones;
        if (targetRenderer.rootBone != null)
        {
            clothingRenderer.rootBone = targetRenderer.rootBone;
        }
    }
}
