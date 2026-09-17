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
    public uint countInitial;   //처음에 준비할 개수
    public uint countAdditional;//부족하면 추가할 개수
}

//                           기본 파일명                           메뉴위치
[CreateAssetMenu(fileName = "PoolRequest", menuName = "PoolRequest/DefaultPoolRequest")]
public class PoolRequest : ScriptableObject
{
    public PoolSetting[] settings;
}
