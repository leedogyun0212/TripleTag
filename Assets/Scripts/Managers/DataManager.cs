//C++을 하시는 분이다!
//#include다!
//C++은 #include를 해야 대상을 볼 수 있는데
//C#은 사실 모든게 다 보입니다!
//근데 앞에다가 이걸 원래 써야 해요!
//NameSpace기 때문에
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

//using UnityEditor; <<이게 자동완성 되는 경우가 있음
//이게 들어오면 빌드가 안됨!

public class DataManager : ManagerBase
{
    //전체 데이터를 저장하는 딕셔너리
    static Dictionary<System.Type,Dictionary<string, Object>> dataDictionary = new();

    // 프로퍼티는 변수모양이지만 함수
    //         int GetLoadCount();
    public override int LoadCount
    {
        get 
        {
            //Async => 비동기 => 남한테 시켜놓고 제 할 일 하는 거
            //LoadCount를 가지고 싶어서 여기 온거 잖아요?
            //LoadCount찾아놔~! 해놓고 제 할일을 하러 떠날 수 있을까?
            //다시 돌아감 : LoadCount가지고 온 거였으니까...? 그럼 이제 모함?
            //비동기가 아니라 동기로 만들어야 합니다.
            var task = Addressables.LoadResourceLocationsAsync("Global");
            var result = task.WaitForCompletion();
            int count = result.Count; //개수를 찾아오기
            //가게에 손님이 찾아 왔다
            //화장실을 가러 나왔음 => 문을 잠그죠
            //손님이 갇힌다
            //파일같은건 열어놓으면 닫겠다는 것까지 알려줘야 해요
            //닫으셔야 한다
            task.Release();
            return count; // 그래서 그 개수를 돌려줌
        }
    }


    protected override IEnumerator OnConnected(GameManager newManager)
    {
        //나는 로딩 스크린이 어떻게 생겼는지 모른다.
        //하지만 로딩스크린을 업데이트 해주고싶다.
        UIBase loading = UIManager.ClaimGetUI(UIType.Loading);
        IProgress<int> progressUI = loading as IProgress<int>;
        IStatus<string> StatusUI = loading as IStatus<string>;

        int loaded = 0;
        int total = LoadCount;
        string loadString = "인원 모으는 중";
        //람다는 도대체 왜 있는 거예요? 왜 가르쳐 주는 거임?
        //람다 Lambda λ => 이름이 없는 함수 anonymous funcion
        //함수 안에서 만들어지는 함수 => 변수로 저장할 수 있다!
        //내 함수 안에서 만든 함수니까 내 함수 안에서 만든 변수도 그냥 사용할 수 있다!
        System.Action ProgressOnLoad = () => 
        {
            loaded++;
            progressUI?.AddCurrent(1);
            StatusUI?.SetCurrentStatus($"{loadString} ({loaded}/{total})");
        };


        //새로운 타입의 무언가를 추가하실 때마다 여기다 넣기!
        loadString = "인원 모으는 중";
        //기다릴건데             파일불러오기<GameObject>                           끝날때까지
        yield return LoadAllFromAssetBundle<GameObject>("Global", ProgressOnLoad).WaitForTask();
        
        loadString = "장소 정하는 중";
        yield return LoadAllFromAssetBundle<PoolRequest>("Global", ProgressOnLoad).WaitForTask();

        //그냥 함수를 실행하는 것이 아니라, 이 작업을 시작할 인원을 모집해야 한다! -> 해당 스레드한테 시켜야 한다!
        //LoadFileFromAssetBundle<GameObject>("Origin/Prefabs/Square.prefab");

        //Interface : 연결고리 => 무엇이 무엇을 사용할 수 있도록 열어주는 기능
        //            GUI : 그래픽 보여줌, 마우스 움직임, 누르기, 떼기, 클릭하기, 드래그
        //윈도우를 하다가, 맥으로 넘어간다! => 클릭하기 어려울까요?
        //이게 "클릭"이야 => GUI는 클릭이 가능하구나! => GUI이기만 하면 클릭을 지원하겠구나!
        //"어떤 기능이 있을 거야"라는 [약속]이 바로 Interface
        //IOpenable => 열기 닫기 토글, 열렸는지 확인도 가능하다!

        //로딩 진행율 => 최대 몇 개인지, 현재 몇 개까지 했는지
        //              현재 / 최대     1 / 100 = 0.01
        //10개
        yield return null;
    }

    protected override void OnDisconnect()
    {

    }

    //파일을 가지고 올 건데, "경로"로 가져오는  것이 중요한 이유!
    //Resources => 유니티에서 Resources폴더를 만들고 나면 사용할 수 있다!
    //Resources/Prefabs/Square
    //드래그 - 드롭으로 넣는게 아니라 파일 경로로 찾는 이유는 무엇일까요?
    //파일이 많으면 드래그하는 데에 한 세월 걸림
    //폴더 째로 로드가 가능하다
    //폴더 내부에 있는 파일을 다른 사람(프로그래머 외)이 수정해도 괜찮다.
    //=> 원래 지정되었던 게 전부 풀리고 => 새로 들어온 건 그냥 멀뚱멀뚱 있음
    //기획 문서를 가지고 사람들이 무언가를 찾을 수 있습니다.
    //프로그래밍 팀 따로, 아트 팀 따로, 사운드 팀 따로...
    //프로그래밍 팀은 아트가 아직 안들어와도 진행해도 된다.
    //프로그래밍 팀이 그냥 "경로"를 설정해놓고 (예외처리만) 담날 왔습니다.
    //근데 원래 이미지가 없었는데 오늘 켜봤더니 이미지가 적용되어 있다!
    bool TryGetFileFromResources<T>(string path, out T result) where T : Object
    {
        //Resources.LoadAll<T>(path);
        result = Resources.Load<T>(path);
        return result != null;
    }
    //1. 경로로 찾는 건 좋은 거라서
    //2. 경로로 찾을 수밖에 없어서
    //파일을... 클라이언트가 모두 가지고 있을 수 있는가 여부
    //모바일 애플리케이션 => 플레이 스토어에서 200MB까지
    //컨텐츠 추가 다운로드 중...
    //Asset Bundle => 경로 (제가 임의로 지정한 카테고리)
    //DLC => 특정 카테고리에 있는 요소를 다운로드 하게 할 것인가 말 것인가?
    //Addressable
    //async함수는 비동기 함수 => 다른 함수와 같이 돌아갈 수 있는 함수!
    //Coroutine과의 차이점!
    //Coroutine은 "멀티 스레드"가 아니다
    //동시에 하는 것처럼 보이는 역할!
    //하나의 스레드가 공부하다가 청소하다가 공부하다 청소하다....
    //너무 빨라서 잔상이 사라지기 전에 다시 돌아오기 때문에
    //혼자서만 둘이서 일하는 것처럼 보인다. => 효율은 오지게 떨어진다 => 결국 한사람이니까
    //데드락이 걸릴 일이 없다! => 안전하다!
    //기다려야 하는 일이 없다! => 최대 화력으로 돌림!
    //관리가 잘된 멀티 스레드 > 코루틴
    //여러분들은 동시에 돌아가고 있는 여러개의 기능의 속도를 정확히 똑같이 맞출 수 있나요?
    //한 명은 탄막 발사 기능, 보스 패턴 계산을 한다
    //언어에는 2종류
    // 컴파일 인터프리터
    // C++, C#, Java
    //Compile : 엮다, 번역을 끝내둠 => 프로그램을 미리 기계가 돌릴 수 있는 "목적 코드"로 미리 만들어 둠
    //                유니티에서도 코드를 바꾼 다음에 유니티 클릭하면 컴파일 하면서 로딩창 나옴!
    //Python, JavaScript, Java
    //Interpreter : 통역사, 번역기 => 그 때 그 때 확인을 해서 목적코드를 한 줄씩 생성해서 실행

    //나랑 같이 작업할래?
    //나는 이런 일을 해!

    //여러분들이 다른 사람과 협업한다고 생각해봅시다.
    //각자 집에서 작업을 한다!
    //인형 눈을 붙이는 작업을 한다!
    //결과적으로 한 데 모아가지고 인형 눈을 납품해야 한다!
    //어떤 약속이 필요할까?
    //다 끝내면 "모아 놓을 장소"를 정해놓아야 해요!
    //작업이 끝나면 "어떤 프로세스"를 진행해야 할지 알려주기!
    //너 작업 다 하고 작업 한 거 동그라미 치고, 그 다음에 A동 박스에 넣어놓고 돌아와
    //할 일을 이야기한다! => 매개변수로 "할 일"을 넣는 방법이 있을까?
    //컴퓨터에서 "할 일"은 "기능" => "Function" => 함수
    //함수를 매개변수로 넘겨줄 수 있다.
    //
    //저장을 한다는 것은 무엇을 암시할까?
    //불러와야 합니다
    //저장을 할 때 가장 중요한 건 불러와야 합니다
    //냉장고 정리
    //신선칸 => 채소
    //냉장고 앞 문 쪽문 => 마실 것
    //데이터는 그러면 어떻게 저장하는게 편할까?
    //프리팹(게임오브젝트)
    //그림(스프라이트)
    //손님이 왔음. 프리팹을 주시오! => 어떤 프리팹을 원하시나요?
    //                               제품명을 좀 알려주세요
    //1. 종류로 저장한다
    //2. 세부 분류를 저장한다!
    //3. 이름으로 저장한다
    //종류로 내용물을 찾음 => Dictionary
    //GameObject Square17
    //Type                    => String => GameObject
    //                 Dictionary<String, GameObject>
    //Dictionary<Type,                               > 
    public static void SaveDataFile<T>(T target) where T : Object
    {
        if (target == null) return;
        Dictionary<string, Object> innerDictionary;

        //지금까지 이런 Object는 없었다. 처음보는 Type이다
        //innerDictionary가 존재하지 않을 것이기 때문에!
        if (!dataDictionary.TryGetValue(typeof(T), out innerDictionary))
        {
            //만들어야한다! 
            innerDictionary = new();
            //만들어서 해당 타입으로 등록해주기!
            dataDictionary.Add(typeof(T), innerDictionary);
        }

        //이 밑에서 부터는 무조건 innerDictionary가 있다!
        innerDictionary.TryAdd(target.name.ToLower(), target);
    }

    public static T LoadDataFile<T>(string fileName) where T : Object
    {
        fileName = fileName.ToLower();

        if(dataDictionary.TryGetValue(typeof(T), out Dictionary<string, Object> innerDictionary))
        {
            if(innerDictionary.TryGetValue(fileName, out Object result))
            {
                return result as T;
            }
        }

        //else는 안 적어야 위에 있는 두겹의 if를 모두 처리 가능!
        return null;
    }

    //LoadAssets로 넘어오는 순간 생긴 문제!
    //하나가 아니다 => 오래 걸린다 => 하나 할때마다 할 일
    //                                         Action => 행동
    //                                         행동은 언제나 함수! => 변환값이 없는 함수!
    //                                         Action<>           => void Function()
    //                                         Action<int>        => void Function(int a)
    //                                         Action<float>      => void Function(float a)
    //                                         Action<int, float> => void Function(int a, float b)
    //                                         최대 16개의 매개변수까지 등록할 수 있다
    
    //                                         Func => 함수
    //                                         수식은 반환값이 있어야 하니까 => 맨 오른쪽에 반환 자료형
    //                                         Func<float, int>           => int Function(float a)
    //                                         Func<float, string, int>   => int Function(float a, string b)
    public async Task LoadAllFromAssetBundle<T>(string label, System.Action actionForEachLoad) where T : Object
    {
        //                                 V                (매개변수) => {내용}
        var finder = Addressables.LoadAssetsAsync<T>(label,(T loaded) => 
        {
            SaveDataFile(loaded); // 로드 되었으니까 저장해 놓아야지
            actionForEachLoad(); // 할 일 있다고 하니까 해줘야지
        });
        Task result = finder.Task;
        await result;
        finder.Release();
    }

    public async void LoadFileFromAssetBundle<T>(string address) where T : Object
    {
        //기다리긴 하는데, "비동기"로 기다릴 거임
        var finder = Addressables.LoadAssetAsync<T>(address);
        await finder.Task; //Start / Run에 해당하는 부분!
        SaveDataFile(finder.Result);
        finder.Release();

        //A의 뜻이 뭘까?
        //An-
        //"~이 아닌"
        //"반대되는" 접두사
        //Tan => ATan
        //동기화하지 않는다! => 비동기
        //프로세스가 동기화되지 않는다
        //=> 하나의 프로세스로 돌리는 것이 아니다
        //                     유니티
        //=> 멀티 스레드 <-> 싱글 스레드
        //       Thread
        //       줄,실
        //한 번에 실행하는 기능의 개수
        //밥 먹으면서 게임 하면서 유튜브보면서 음악틀면서
        //시간이 빠르게 완료될 수 있다
        //게임을 하는 동안에 밥을 먹고 있단 말이죠.
        //지금 한타하느라 스킬을 조준해야 되는데, 숟가락을 들고 있어서
        //근데 ...저희는 그 상황에서 "결정"을 하잖아요?
        //손을 어따 써야 할지? => 우선 순위가 있어야 함!
        //컴퓨터 입장에서는.. 지금 할 일 스레드마다 하나씩
        //어차피 이거 안하고 다음으로 넘어갈 수가 없습니다
        //데미지 주는 기능이다!
        //생명력 감소하려고 했는데.. 생명력을 누가 쓰고 있어서 못바꾼다!
        //생명력 감소 안하고 죽었는지 체크할 것인가?
        //=> 데드락
        //원래 밥만 먹었을 때보다 밥먹는 시간은 느려진다
        //왜?
        //밥 먹는 애, 유튜브보는 애, 게임하는 애, 음악 듣는 애
        //   O             O            X             O
        //다른 애들이 전부 게임하는 애가 기다렸다가 다음 작업을 해야해요!
        //게임하는 애가 뭔가 중요한 변화를 주고 끝낼 수도 있잖아요?
    }
}
