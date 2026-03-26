using System.Collections;
using UnityEngine;
//오브젝트를 생성하고 제거하는 것은 오랙 걸리는 작업이 맞습니다!
//제거하고 난 뒤에는 문제가 크게 생깁니다
//무언가를 제거하는 건 항상 신중하게!
//C#의 오브젝트는 삭제 되는 기능이 존재하지 않는다!
//사람이 언제 죽는지 아나? => 잊혀졌을 때 : 이 친구를 저장하고 있는 오브젝트가 없을 때
//기억하는 사람이 아무도 없어지면 => 쓰레기장으로 갑니다
//Garbage가 됩니다. => Garbage Collector => 폐품 수집가 => 주기적으로 순찰을 해요!
//나 이제 없어졌어! 라고 주장하는 친구들을 학살합니다
//내가 삭제되었어 라고 주장하는 애들이 많아지면 많아질수록 이 친구의 일은 많아진다!
//가비지컬렉터의 역할 : 얘가 쓰레기인지 판별도 해야해요
//                    이 세상에 얘를 기억하고 있는 애가 있는지 체크
//                    알고 있을 법한 모두한테 가서 "기억하고 있어"라고 물어봐야해요
//                    성능을 오지게 잡아먹는다
//만들어진다 없어진다 하는 과정이 있으면 힘드니까 => 안 할 수 있는 방법!
//없애지 않으면 된다 => 오브젝트를 껏다 켰다로 대체한다
//오브젝트 풀링
//만드는 과정을 인게임중에는 안 하고 로딩할 때 해버리고 싶다!
//매번 만들기 싫으니까 한 캐릭터가 50000개 만들면 잘 쓸 수 있지 않을까?
//웬만하면은 이 친구가 "일반적인 상황"에서 나올 수 있는 최대 개수
//없을 수 없으면 struct 
//pooling을 위한 설정
//                    직렬연결
//                  Serial Number는 숫자가 연속적으로 나열되어 있는 것
//                     직렬화할수있는    직렬화     직렬 
[System.Serializable]// Serializeable Serialize => serial
                     // 데이터를 한줄로 쭉 뽑아볼 수 있다. 유니티에서 확인 가능
public struct PoolSetting
{
    public string poolName;    //이 풀링 정보를 어떤 이름으로 보고 싶은가?
    public GameObject target;  //풀링할 대상 
    public int countInitial;   //처음에 준비할 개수
    public int countAdditional;//부족하면 추가할 개수
}

public class ObjectManager : ManagerBase
{
    //직렬화 가능한 => 유니티에서 보기 위해서 쓴 것!
    //public이라고 하는 건 사실 필요 없고 직렬화만 되면 유니티에서 볼 수 있다!
    //직렬화 변수
    [SerializeField] PoolSetting[] testSetting;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        yield return null;
    }

    protected override void OnDisconnect()
    {

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
    public static GameObject CreateObject(GameObject prefab, Vector3 position)
    {
        GameObject result = CreateObject(prefab);
        if(result) result.transform.position = position;
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
        Destroy(target);
    }

    public static void UnregistrationObject(GameObject target)
    {
        if (!target) return;

        foreach (var currrent in target.GetComponentsInChildren<IFunctionable>())
        {
            currrent.UnregistrationFunctions();
        }
    }
}
