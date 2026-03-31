using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectManager : ManagerBase
{
    //이제 새로운 Global 파일을 추가할 때 글자 하나만 추가하면 됨!
    //바꿀 필요가 없다 => 변수가 아니라 상수인 셈! => 나중에 바뀌면 안됨!
    //일반적인 상수는 constant variable이 맞습니다.
    //"읽기 전용"으로 바꿔야 한다
    readonly string[] globalPoolSettings =
    {
        "GlobalCharacterPool",
        "GlobalControllerPool",
        "GlobalEffectPool",
        "GlobalObjectPool",
        "GlobalUIPool"
    };

    //직렬화 가능한 => 유니티에서 보기 위해서 쓴 것!
    //public이라고 하는 건 사실 필요 없고 직렬화만 되면 유니티에서 볼 수 있다!
    //직렬화 변수
    //[SerializeField] PoolSetting[] testSetting;

    //PoolRequest가 있고, 그것을 위한 풀링을 준비하기
    //PoolRequest을 가져와서 저장하려면 어떤 자료구조가 필요할까?
    //리스트 : 배열과 비슷한데 추가 제거가 쉬움, 용량△, 찾는 속도가 느리다
    //추가 제거가 많고, 전체를 도는 일이 적음

    //배열 : 리스트와 비슷한데 추가 제거가 어려움, 용량▽, 찾는 속도가 빠르다
    //추가 제거가 적고, 전체를 도는 일이 많은

    //PoolRequest는... 얼마나 자주 추가될까? 로딩할 떄 즈음?
    //로딩되는 횟수보다 대상이 개수가 부족하면 새로 추가하거나 하는 일!
    List<PoolRequest> loadedPoolRequest = new();

    //해당하는 이름의 대상으로 불러주기 위해서
    //[이름 - 게임 오브젝트] 자료구조
    static Dictionary<string, ObjectPoolModule> poolDictionary = new();

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        RegistrationPool(globalPoolSettings);
        InitializePool();

        yield return null;
    }

    protected override void OnDisconnect()
    {

    }

    public static GameObject CreateObject(string wantName, Transform parent = null)
    {
        GameObject result = null;

        //이름으로 풀링이 등록되어 있대요!
        if (poolDictionary.TryGetValue(wantName, out ObjectPoolModule pool))
        {
            result = pool.CreateObject(parent); // 갖고 와야겠다 ㅎㅎ
        }
        else
        {
            //풀에 등록되지 않은 야생의 오브젝트를 만드는 방법
            //데이터에는 있는지 확인
            GameObject prefab = DataManager.LoadDataFile<GameObject>(wantName);
            if (prefab)
            {
                result = Instantiate(prefab, parent);
            }
        }

        //등록해주는 것 까지
        RegistrationObject(result); //둘 중 하나라도 했겠지? 아님 말고!

        return result;
    }

    //                                                      부모게임오브젝트는 "Transform"으로 저장함
    public static GameObject CreateObject(GameObject prefab, Transform parent = null)
    {
        if (prefab == null) return null;

        //                                      누가 주인인가
        GameObject result = Instantiate(prefab, parent); //만들고
        //이 친구가 등록 가능한지를 어떻게 체크할까?
        //저희가 만드는 건 "컴포넌트"를 만드는 것이지
        //"게임 오브젝트"를 만드는 것이 아니기 때문에
        //IFunctionable이 들어간 곳은 "컴포넌트"다
        RegistrationObject(result);//등록함
        return result;
    }

    //크기는?
    //정말 애매한 친구
    //부모 자식간의 크기 차이로 결정되기 때문에 이상한 행동이 많이 나옴
    public static GameObject CreateObject(string wantName, Vector3 position)
    {
        GameObject result = CreateObject(wantName);
        if(result) result.transform.position = position;
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position)
    {
        GameObject result = CreateObject(prefab);
        if(result) result.transform.position = position;
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(wantName);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result = CreateObject(prefab);
        if (result)
        {
            result.transform.position = position;
            result.transform.rotation = rotation;
            result.transform.localScale = scale;
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch(space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch(space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    break;
            }
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch(space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch(space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    break;
            }
        }
        return result;
    }

    public static GameObject CreateObject(string wantName, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(wantName, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale;

                    //float scaledScaleX = scale.x * (result.transform.localScale.x / result.transform.lossyScale.x);
                    //float scaledScaleY = scale.y * (result.transform.localScale.y / result.transform.lossyScale.y);
                    //float scaledScaleZ = scale.z * (result.transform.localScale.z / result.transform.lossyScale.z);
                    //result.transform.localScale = new Vector3(scaledScaleX, scaledScaleY, scaledScaleZ);
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    result.transform.localScale    = scale;
                    break;
            }
        }
        return result;
    }
    public static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, Vector3 scale, Space space = Space.Self)
    {
        GameObject result = CreateObject(prefab, parent);
        if (result)
        {
            switch (space)
            {
                case Space.World:
                    result.transform.position = position; //절대값을 기준으로
                    result.transform.rotation = rotation;
                    result.transform.localScale = scale;

                    //float scaledScaleX = scale.x * (result.transform.localScale.x / result.transform.lossyScale.x);
                    //float scaledScaleY = scale.y * (result.transform.localScale.y / result.transform.lossyScale.y);
                    //float scaledScaleZ = scale.z * (result.transform.localScale.z / result.transform.lossyScale.z);
                    //result.transform.localScale = new Vector3(scaledScaleX, scaledScaleY, scaledScaleZ);
                    break;
                case Space.Self:
                    result.transform.localPosition = position; //부모를 기준으로
                    result.transform.localRotation = rotation;
                    result.transform.localScale    = scale;
                    break;
            }
        }
        return result;
    }

    public static void RegistrationObject(GameObject target)//실제로 등록하는 기능
    {
        if (target)
        {
            foreach (var currrent in target.GetComponentsInChildren<IFunctionable>())
            {
                currrent.RegistrationFunctions();
            }
        }
    }

    public static void DestroyObject(GameObject target)
    {
        if (!target) return;
        UnregistrationObject(target);
        if (target.TryGetComponent(out PooledObject pool))
        {
            pool.OnEnqueue();
        }
        else
        {
            Destroy(target);
        }
    }

    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;

        foreach (var currrent in target.GetComponentsInChildren<IFunctionable>())
        {
            currrent.UnregistrationFunctions();
        }
    }

    public void RegistrationPool(string poolName)
    {
        //명령!
        PoolRequest currentRequest = DataManager.LoadDataFile<PoolRequest>(poolName);
        if (currentRequest == null) return;
        loadedPoolRequest.Add(currentRequest);

        foreach (PoolSetting currentSetting in currentRequest.settings)
        {
            string currentName = currentSetting.poolName;
            GameObject currentPrefab = currentSetting.target;
            if (currentPrefab == null) continue;
            //문제가 생길 여지가 하나 더 있다!
            //프리펩을 찾아봤으니까, 이름에서 문제가 생길 수 있는 여지!
            //딕셔너리에는 같은 키값을 두 개 넣을 수 없다!
            if (poolDictionary.ContainsKey(currentName)) continue;

            poolDictionary.Add(currentName, new (currentSetting));
        }
    }

    //"가변 인자" => 인자의 개수가 무한정 늘어날 수 있는 함수
    //"변인" => 영어로 뭐죠? Parameter : "변인들"이 된다면? Parameters
    //Parameters => Params
    public void RegistrationPool(params string[] poolNames)
    {
        foreach (string poolName in poolNames)
        {
            //가변인자는 "우선순위가 낮습니다"
            //가변인자다 보니까 개수가 "고정인자를"가진 함수랑 똑같아질 수 있잖아요?
            //"고정된 인자"를 가지고 있는 함수를 먼저 인식해서 실행한다.
            RegistrationPool(poolName);
        }
    }

    public void InitializePool()
    {
        foreach(ObjectPoolModule currentPool in poolDictionary.Values)
        {
            currentPool?.Initialize();
        }
    }
}
