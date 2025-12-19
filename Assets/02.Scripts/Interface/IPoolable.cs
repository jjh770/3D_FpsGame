/// <summary>
/// 오브젝트 풀에서 사용되는 객체가 구현해야 하는 인터페이스
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 풀에서 오브젝트를 꺼낼 때 호출됩니다.
    /// 오브젝트의 상태를 초기화하고 필요한 설정을 수행합니다.
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// 오브젝트를 풀에 반환할 때 호출됩니다.
    /// 오브젝트의 상태를 정리하고 다음 사용을 위해 준비합니다.
    /// </summary>
    void OnDespawn();
}
