using UnityEngine;

public delegate void PoolEnqueueEvent(GameObject target);
public delegate void PoolDequeueEvent(GameObject target);

public class PooledObject : MonoBehaviour
{
    //event는 왜 써야 하는가
    //   delegate와    event의 차이 
    //남들이 구독/실행  남들이 구독
    //      행동          이벤트
    public event PoolEnqueueEvent OnEnqueueEvent;
    public event PoolDequeueEvent OnDequeueEvent;

    //오브젝트 풀링.. 이라고 하는 게 뭐였더라
    //생성 / 삭제를 하는 대신 => 켜고 끄는걸로 대체!
    //안좋아지는 게 있는 거 아닐까요?
    //삭제하는 것의 미학 => 정보의 유지

    //큐로 돌아갈 때 할 일
    public void OnEnqueue()
    {
        if (OnEnqueueEvent != null) OnEnqueueEvent.Invoke(gameObject);
        else Destroy(gameObject);
    }
    //큐로 나올 때 할 일
    public void OnDequeue()
    {
        OnDequeueEvent?.Invoke(gameObject);
    }
}
