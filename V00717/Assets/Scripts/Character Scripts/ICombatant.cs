public interface ICombatant
{
    public string Name();
    public bool IsEnemyAI();
    public float GetHealth();
    public void DealDamage(ICombatant opponent);
    public void TakeDamage(float damage);
    public void SetLastEvent(string lastEvent);
}
