using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.EventSystems;

//확장메소드들을 가지고 있을 친구들!
//영토 확장
//확장판
//=>추가 컨텐츠
//원본 오리지널 게임에서는 없었던 것을 추가해주는 개념!
//내가 원하는 클래스에 내가 원하는 기능을 추가해줄 수 있다!
//=>객체화가 필요한 친구일까?
public static class Extensions
{
    //숫자놀이!
    //연산을 하는 놀이!
    //Normalize => 방향을 유지한 채 크기를 1로 만드는 것
    //벡터에만 있잖아요?
    //Float에도 만들어보면 어떨까?
    //Normalize는 본인을 1로 바꿔버리는 것이고
    //Normalized는 크기가 1인 값을 돌려주는 것
    //float를 정규화하면 어떤 모양이 될까?
    // 13.0f => 1.0f
    // -2.0f => -1.0f 
    // 0.0f  => 0.0f
    // float => float
    //                             내가 함수를 넣고 싶은 대상
    //                             this
    public static float normalized(this float target)
    {
        if (target > 0)      return 1.0f;
        else if (target < 0) return -1.0f;
        else                 return 0.0f;
    }

    //Try Add Component => 추가를 시도 => 있는지 확인 => 없으면 추가
    public static T TryAddComponent<T>(this GameObject target) where T : Component
    {
        T result = null;

        if (target == null) return result; //RVO
        Debug.Log($"[Extensions] TryAddComponent<{typeof(T).Name}> called on '{target.name}'");

        result = target.GetComponent<T>() ?? target.AddComponent<T>();
        //Debug.Log($"[Extensions] Result: {(result == null ? "null" : result.GetType().Name)}");

        //result = target.GetComponent<T>();
        //result ??= target.AddComponent<T>();

        //result = target.GetComponent<T>();
        //if(result is null) result = target.AddComponent<T>();

        return result;
    }

    public static T TryAddComponent<T>(this Component target) where T : Component
    {
        if (target == null) return null;
        else                return target.gameObject.TryAddComponent<T>();//NRVO
        //                         TryAddComponent<T>(target.gameObject);
    }

    //왼쪽 오른쪽 친구를 가지고 비교를 해서 그 결과가 bool로 나오는 형태의 함수를 Comparison(비교)

    public static bool IsBigger(float a, float b) => a > b;
    public static bool IsSmaller(float a, float b) => a < b;

    public static T GetExtreme<T>(this IEnumerable targetList, float defaultScore,
        System.Func<T, float> Evalueator,
        System.Func<float, float, bool> Comparison)
    {
        T result = default;
        float firstScore = defaultScore;
        
        //foreach에 in역할로 넣을 수 있는 것들 List, Array, Dictionary 같은 애들!
        //IEnumerable : 열거할 수 있는 / 반복할 수 있는
        foreach (T currentTarget in targetList)
        {
            float currentScore = Evalueator(currentTarget);

            //현재 스코어와 일등 스코어를 비교!
            if (Comparison(currentScore, firstScore))
            {
                result = currentTarget;
                firstScore = currentScore;
            }
        }
        return result;
    }

    public static T GetMaximum<T>(this IEnumerable targetList, System.Func<T, float> Evalueator)
        => targetList.GetExtreme(float.MinValue, Evalueator, IsBigger);

    public static T GetMinmum<T>(this IEnumerable targetList, System.Func<T, float> Evalueator)
        => targetList.GetExtreme(float.MaxValue, Evalueator, (a, b) => a < b);

    public static IEnumerator WaitForTask(this Task targetTask)
    {
        //WaitWjile : true인 동안 작동함
        //WaitUntil : flase인 동안 작동함 true가 될 때까지 기다림
        //            기다린다 끝까지          타겟작업 끝나기
        //              Wait Until Target Task is Completed
        yield return new WaitUntil(() => targetTask.IsCompleted);
        //작업을 제거하다
        targetTask.Dispose();
    }

    public static float GetPenetratedDistance(float aHalf, float bHalf, float aPos, float bPos)
    {
        float absAHalf = Mathf.Abs(aHalf);
        float absBHalf = Mathf.Abs(bHalf);
        //그래서 겹쳤다면, 만약에 원래 안 겹쳤을 때에 있을 수 있는 공간
        float minSpace = absAHalf + absBHalf;
        //지금 이 둘사이의 거리가 얼마나 가까운지!
        float distance = aPos - bPos;
        //x최소 거리와 둘 사이의 거리 차이! => 예외처리!
        float penetration = minSpace - Mathf.Abs(distance);
        //어느 방향으로 묻혀있는지 확인하는 것도 중요!
        //A가 왼쪽 => +로 보여줄까 -로 보여줄까!
        //xDistance의 부호를 그대로 따라가게 하려면!
        //              마이너스면 -1 / 0이상여면 1
        penetration *= Mathf.Sign(distance);
        return penetration;
    }

    //A와 B가 있는데, 얼마나 깊게 묻혀 있는지 확인
    //기준은 A로 잡을거예요
    public static Vector2 AABB(this Rect A, Rect B)
    {
        Vector2 result = Vector2.zero;
        Vector2 aMin = A.min;
        Vector2 aMax = A.max;
        Vector2 aHalf = A.size * 0.5f;
        Vector2 bMin = B.min;
        Vector2 bMax = B.max;
        Vector2 bHalf = B.size * 0.5f;

        //한쪽의 최대 위치가 다른 쪽의 최소 위치보다 높아야 함!
        if(aMax.x > bMin.x && bMax.x > aMin.x)
        {
            result.x = GetPenetratedDistance(aHalf.x, bHalf.x, A.position.x, B.position.x);
        }

        if (aMax.y > bMin.y && bMax.y > aMin.y)
        {
            result.y = GetPenetratedDistance(aHalf.y, bHalf.y, A.position.y, B.position.y);
        }

        return result;
    }

    //Clamp를 만드는 것과 비슷하다!
    //숫자 하나를 범위내에 유지하도록 만들어 주는 것!
    //대신 숫자 하나가 아니라 범위를 범위 내에 있도록 해주는 것!
    //내부적으로 확률을 맟출 때라던지
    public static float GetOutboundDistance(float inMin, float outMin, float inMax, float outMax)
    {
        float result = 0.0f;

        bool leftOut = inMin < outMin;
        bool rightOut = inMax > outMax;

        if (leftOut ^ rightOut)
        {
            if (leftOut) result = outMin - inMin;
            if (rightOut) result = outMax - inMax;
        }
        return result;
    }

    //빠져 나온 양을 체크하는 방법!
    //오른쪽으로 2만큼 빠져 나왔다면 (-2, 0)  
    //왼쪽으로 3만큼 빠져 나왔다면 (3, 0)  
    //아래로 1만큼 빠져 나왔다면 (0, 1)  
    //위로 1만큼 빠져 나왔다면 (0, -1)  
    public static Vector2 InversedAABB(this Rect target, Rect bound)
    {
        Vector2 result;
        result.x = GetOutboundDistance(target.xMin, bound.xMin, target.xMax, bound.xMax);
        result.y = GetOutboundDistance(target.yMin, bound.yMin, target.yMax, bound.yMax);

        return result;
    }
}
