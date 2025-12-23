// 인터페이스 : 클래스간의 약속
public interface IDamageable
{
    // IDamageable 약속을 지켜야 하는 클래스는 무조건 아래 메서드를 구현해야한다.
    // 외부에 노출되는 약속이기 때문에 무조건 public
    public bool TryTakeDamage(Damage damage);
}
