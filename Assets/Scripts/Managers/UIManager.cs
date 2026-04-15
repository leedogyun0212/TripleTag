using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIType
{
    None, Loading, Title,Option, Movable,Profile,Message,Main,GameQuit,Shop,
    _Length
}

public enum ScreenChangeType
{
    None,
    ScreenChanger,
    _Length
}

public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{
    public static event PopUpEvent OnpopUp;

    readonly KeyValuePair<UIType, string>[] globalScreenArray =
    {
        new (UIType.Title,"TitleScreen"),
        new (UIType.Main, "MainScreen"),
        new (UIType.Option, "OptionScreen"),
    };

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    UIBase _movableScreen;
    RectTransform switcherTransform;
    RectTransform createdTransform;
    RectTransform changerTransform;

    GraphicRaycaster _raycaster;
    public GraphicRaycaster Raycaster => _raycaster;

    //어떤 창을 열어주세요!
    //         이 타입 어떤 오브젝트!
    Dictionary<UIType, UIBase> uiDictionary = new();

    Dictionary<ScreenChangeType, UI_ScreenChanger> screenChangerDictionary = new();

    Rect _uiBoundary;
    public static Rect UIBoundary => GameManager.Instance?.UI?._uiBoundary ?? Rect.zero;

    UIType _currentScreenType = UIType.None;
    public static UIType CurrentScreen => GameManager.Instance?.UI?._currentScreenType ?? UIType.None;

    UI_ScreenChanger currentScreenChanger;

    float _uiScale = 1.0f;
    public static float UIScale => GameManager.Instance?.UI?._uiScale ?? 1.0f;

    public IEnumerator Initialize(GameManager newManager)
    {
        //GameObject.FindGameObjectWithTag("MainCanvas");
        SetMainCanvas(GetComponentInChildren<Canvas>());
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    public RectTransform CreateFullScreen(string wantName)
    {
        GameObject instance = new GameObject(wantName);
        RectTransform result = instance.AddComponent<RectTransform>();
        //메인 캔버스에 넣고 
        result.SetParent(MainCanvas.transform);
        //맨 위로 올려주기!
        result.SetAsFirstSibling();
        //anchor를 streach - streach로 만들고 
        result.anchorMin = Vector3.zero;
        result.anchorMax = Vector3.one;
        //여백을 0,0,0,0
        result.offsetMin = Vector3.zero;
        result.offsetMax = Vector3.zero;
        //크기를 1로
        result.localScale = Vector3.one;

        return result;
    }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        createdTransform = CreateFullScreen("CreatedUI");
        _movableScreen = CreateUI(UIType.Movable, "MovableScreen", MainCanvas?.transform);

         switcherTransform = CreateFullScreen("ScreenSwitcher");

        foreach (var currenPair in globalScreenArray)
        {
            UIBase created = CreateUI(currenPair.Key, currenPair.Value, switcherTransform);

            if(created is IOpenable asOpenable) asOpenable.Close();
        
        }

        changerTransform = CreateFullScreen("ScreenChanger");
        changerTransform.SetAsLastSibling();

        for (ScreenChangeType currentChanger = (ScreenChangeType)1; // int i = 0;
            currentChanger < ScreenChangeType._Length;              // i < 3;
            currentChanger++)                                       // i++;
        {
            GameObject instance = ObjectManager.CreateObject(currentChanger.ToString(), changerTransform);
            //만든 대상에게서 스크린 체인저 기능을 가져오기
            if(instance?.TryGetComponent(out UI_ScreenChanger asChanger) ?? false)
            {
                //가져와졌으먄 딕셔너리에 추가
                screenChangerDictionary.Add(currentChanger, asChanger);
            }
            //끄고 갑시다
            instance.SetActive(false);
        }


        //{
        //    ClaimScreenChangeEffectStart(ScreenChangeType.ScreenChanger);
        //    yield return new WaitForSeconds(3);
        //    ClaimScreenChangeEffectEnd();
        //}

        yield return null;
    }

    protected override void OnDisconnect()
    {
        //싹 다 나가!
        UnsetAllUI();
    }

    protected void SetMainCanvas(Canvas newCanvas)
    {
        _mainCanvas = newCanvas;
        if (_mainCanvas)
        {
            _raycaster = _mainCanvas.GetComponent<GraphicRaycaster>();

            if(MainCanvas.transform is RectTransform mainRectTransform)
            {
                _uiScale = mainRectTransform.lossyScale.x;
                _uiBoundary = mainRectTransform.rect;
            }
        }
        else
        {
            _raycaster = null;
        }
    }
    protected UIBase CreateUI(UIType wantType, string wantName, Transform parent)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, parent);
        UIBase result = instance?.GetComponent<UIBase>();
        return SetUI(wantType, result);
    }
    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        UIBase result = CreateUI(wantType, wantName,createdTransform ?? MainCanvas?.transform);

        if(result?.GetComponentInChildren<UI_DraggableWindow>())
        {
            Debug.Log(result.name);
            _movableScreen?.SetChild(result.gameObject);
        }

        return result;
    }
    public static UIBase ClaimCreateUI(UIType wantType, string wantName) => GameManager.Instance?.UI?.CreateUI(wantType, wantName);

    protected void UnsetUI(UIType wantType)//담당 공무원의 부서를 알고 있는 경우
    {
        //그 직원을 찾아야 함
        //담당 공무원의 이름을 알고 있는 경우로 이동하시오
        if (uiDictionary.TryGetValue(wantType, out UIBase found))
        {
            UnsetUI(found);

            uiDictionary.Remove(wantType);
        }
    }
    protected void UnsetUI(UIBase wantUI)//담당 공무원의 이름을 알고 있는 경우
    {
        if(!wantUI) return;

        wantUI.Unregistration(this);
    }
    public static void ClaimUnsetUI(UIBase wantUI)                  => GameManager.Instance.UI?.UnsetUI(wantUI);
    public static void ClaimUnsetUI(GameObject wantObject)          => ClaimUnsetUI(wantObject?.GetComponent<UIBase>());
    
    protected void UnsetAllUI()//싹 다 해고야
    {
        foreach(UIBase ui in uiDictionary.Values)//애들 전부 돌면서
        {
            UnsetUI(ui);//나가라고 해주기
            //여기에서 나가라고 할때마다 Dictionary에서 빼려고 하면
            //안되는 이유
            //uiDictionary.Remove(wantType);
            //제거를 하는 경우 uiDictionary의 모양이 달라진다
        }
        //다 나갔으니까 직원 명부를 버렵버림!
        uiDictionary.Clear();
    }
    
    protected UIBase SetUI(UIBase wantUI)
    {
        wantUI?.Registration(this);
        return wantUI;
    }
    public static UIBase ClaimSetUI(UIBase wantUI)                  => GameManager.Instance?.UI?.SetUI(wantUI);
    public static UIBase ClaimSetUI(GameObject wantObject)          => ClaimSetUI(wantObject?.GetComponent<UIBase>());
    
    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //Set UI를 하려고 하는데 문제가 무엇일까!
        //InventoryType, InventoryInstance
        if(wantUI == null) return null;

        //어 뭐야? 이미 Inventory는 있는데? 너는 누구냐! => 서생원
        //일단 문전박대 => 프로그래밍에서는요? 똑같은 기능을 하는 친구면
        //음... 너가 원본인 건 무슨 상관인데?
        //뒤이어서 들어온 친구는 치워버리겠다!
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;

        //두 가지의 시련을 모두 통과하면, 너는 등록될 수 있는 자격을 갖추었다.
        uiDictionary.Add(wantType, wantUI);
        //등록완
        return SetUI(wantUI);
    }
    public static UIBase ClaimSetUI(UIType wantType, UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantType, wantUI);

    protected UIBase GetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType,out UIBase result)) return result;//있으면 result반환
        else return null;//없으면 null
    }
    public static UIBase ClaimGetUI(UIType wantType)                => GameManager.Instance?.UI?.GetUI(wantType);

    protected UIBase OpenUI(UIType wantType)
    {
        //result가 누군자 전혀 모름! 리스코프 치환 원칙
        //IOpenable이면 열게 해준다! 세부 요소는 모르겠는데, 상위 요소만으로 실행하기

        UIBase result = GetUI(wantType);
        //이게 "열 수 있는"인 건 어떻게 확인할까요?
        //IOpenable인지 체크해보면 열 수 있는지 알 수 있습니다
        //IOpenable로서 활동 할 수 있으면 IOpenable
        //result는 IOpenable인 opener 인가?
        if (result is IOpenable asOpenable) asOpenable.Open();

        if (result) EventSystem.current.SetSelectedGameObject(result.gameObject);

        //아랫줄이랑 같은 의미
        //IOpenable opener = result as IOpenable;
        //if (opener != null) opener.Open();
        return result;
    }
    public static UIBase ClaimOpenUI(UIType wantType)               => GameManager.Instance?.UI?.OpenUI(wantType);

    protected UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //              자료형     이름   => 변수 생성
        if (result is IOpenable asOpenable) asOpenable.Close();

        return result;
    }
    public static UIBase ClaimCloseUI(UIType wantType)              => GameManager.Instance?.UI?.CloseUI(wantType);

    protected UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }
    public static UIBase ClaimToggleUI(UIType wantType)             => GameManager.Instance?.UI?.ToggleUI(wantType);
    
    protected UIBase OpenScreen(UIType wantType)
    {
        CloseUI(CurrentScreen);             //원래 있던거 닫고
        _currentScreenType = wantType;      //이게 내 새로운 타입이다
        return OpenUI(wantType);            //그리고 열기
    }
    public static UIBase ClaimOpenScreen(UIType wantType)           => GameManager.Instance?.UI?.OpenScreen(wantType);

    public void ScreenChangeEffectStart(ScreenChangeType wantType)
    {

        if (screenChangerDictionary.TryGetValue(wantType, out UI_ScreenChanger result))
        {
            if (!result) return;
            //켠다!
            result.gameObject.SetActive(true);
            result?.ChangeStart(ScreenChangeEffectEnd);
            currentScreenChanger = result;
        }
    }
    public static void ClaimScreenChangeEffectStart(ScreenChangeType wantType) => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType);
    
    public void ScreenChangeEffectEnd()
    {
        if (currentScreenChanger == null) return;
        GameObject targetObject = currentScreenChanger.gameObject;
        currentScreenChanger.ChangeEnd(() => targetObject.SetActive(false));
        currentScreenChanger = null;
    }
    public static void ClaimScreenChangeEffectEnd() => GameManager.Instance?.UI?.ScreenChangeEffectEnd();

    public static void ClaimPopUp(string title, string context, string confirm)
    {
        OnpopUp?.Invoke(title, context, confirm);
    }
    public static void ClaimErrorMessage(string context)
    {
        OnpopUp?.Invoke("Error", context, "confirm");
    }
}
