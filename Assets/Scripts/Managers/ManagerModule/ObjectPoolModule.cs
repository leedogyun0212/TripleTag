using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolModule
{
    //오브젝트 하나를 관리하는 하청 모듈!
    //오브젝트 하나를 담당할 거라면 어떤 것들이 필요할까?
    //내가 뭘 하면 되는데?
    PoolSetting _setting;
    public PoolSetting Setting => _setting;

    Transform rootTransform;

    //"대기열"을 만들겁니다!
    //                                           선 입 선 출
    //                                Queue => 먼저 온 애가 먼저 간다
    //"대기열"에 스스로 들어가는 경우! =>큐를 잡는다! / 돌린다!
    //진열대 => 밖에 있는 거를 먼저 가져와요
    //          맨   마지막에
    //          후      입       선     출   => Stack
    //구인할 때 "슬라임"
    //생성할 때 정보를 넣어주기!
    //생성자! 반환값 => 본인, 이름 => 본인
    Queue<GameObject> prepareQueue = new();

    //작업중인 애들은 왜 Queue나 Stack 아니죠?
    //가는데 순서 없다
    //List<GameObject> inProgressList = new();

    //생성할 때 정보를 넣어주기
    //생성자! 반환값 => 본인, 이름 => 본인
    public ObjectPoolModule(PoolSetting newSetting)
    {
        _setting = newSetting;
    }

    public void Initialize()
    {
        rootTransform = new GameObject(Setting.poolName).transform;

        //풀링하려고 하는 원본 프리펩에 "PooledObject"라고 하는 것이
        //안들어있으면 => 너는 풀링된 친구야! => 추가해줄 필요가 있지 않을까?
        //이제부터 풀링되는 오브젝트는 싹 다 PooledObject를 가지게 됨!
        Setting.target?.TryAddComponent<PooledObject>();

        //게임 하면서 미니언을 30개 쓸 거니까! 그 만큼 준비를 미리 해놓아야지!
        //새로운 오브젝트를 미리 대기시킬거임!
        //탱커 7명 대기해주세요
        //드럼통 앞에 불 쬐고 있는 친구 3명
        PrepareObjects(Setting.countInitial);
    }

    //대기자! => 관리를 해주려면!
    //대기하고 있는 얘들 중에 아무나 데려와도 되는가?
    //새벽 6시 출근, 아침 9 출근 아침 10시 출근
    //1명 대려가려고 함 =>  아침 9시 너 나와! 
    //먼저 온 애는??
    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = CreateFromPrefab();
        EnqueueObject(result);
        return result;
    }

    void PrepareObjects(uint count)
    {
        if (!Setting.target) return;
        for (uint i = 0; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    //uint => 마이너스가 존재하면 안됨!
    //뭔가 작업을 애초에 안하는 게 있어야 => 빼돌리는 게 있어야 조금 더 성능상 좋다
    void PrepareObjects(uint count, out GameObject activeObject)
    {
        if (!Setting.target)
        {
            activeObject = null;
            return;
        }

        activeObject = CreateFromPrefab();

        for (uint i = 1; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    public GameObject CreateFromPrefab()
    {
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);

        if (result)
        {
            result.name = Setting.poolName;

            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnEnqueueEvent -= DestroyObject;
                pool.OnEnqueueEvent += DestroyObject;
            }
        }

        return result;
    }

    //오브젝트를 생성해달라고 부탁
    public GameObject CreateObject(Transform parent = null)
    {
        //어떻게 하는 걸까?
        //대기자 중에서 꺼내보기 
        GameObject result;

        //대기열에 아무도 없을 때
        if (!prepareQueue.TryDequeue(out result))
        {
            //새로 대기자를 뽑아서 가져오면 됩니다!
            //추가할 때마다 몇 개씩 넣으라고 하는 것을 숫자로 설정해놓았기 때문!
            PrepareObjects(Setting.countAdditional, out result);
        }

        if(result) // 만들어졌다면
        {
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnDequeue();
            }

            result.transform.SetParent(parent);
            result.SetActive(true);
        }

        return result;
    }

    //오브젝트를 제거해달라고 부탁
    public void DestroyObject(GameObject target)
    {
        //제거하는 방법은 어떻게 될까?
        EnqueueObject(target);
        if(target)
        {
            target.transform.SetParent(rootTransform);
        }
    }

    public void EnqueueObject(GameObject target)
    {
        if (!target) return;
        target.SetActive(false);
        prepareQueue.Enqueue(target);

    }
}
