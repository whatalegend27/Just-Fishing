public interface IHealable
{
    void Heal();
}

public interface IRiskReducible
{
    void ReduceRisk(int amount);
}

public interface IDamageable
{
    void TakeDamage(int amount);
}
