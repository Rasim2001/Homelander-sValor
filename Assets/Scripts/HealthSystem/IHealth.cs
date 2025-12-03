namespace HealthSystem
{
    public interface IHealth
    {
        int CurrentHP { get; set; }
        int MaxHp { get; set; }
        bool IsDeath { get; set; }
        void TakeDamage(int damage);
        void Initialize(int hp);
        void Reset();
    }
}