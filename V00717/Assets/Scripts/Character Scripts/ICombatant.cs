public interface ICombatant
{
    public string CombatName();
    public bool IsEnemyAI();
    public float GetHealth();
    public void DealDamage(ICombatant opponent);
    public void TakeDamage(float damage);
}
