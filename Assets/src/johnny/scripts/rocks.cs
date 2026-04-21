using UnityEngine;

public class Rock : MonoBehaviour
{
  [SerializeField] Sprite rockSprite;

  // Dyanmic binding of damage amount
  // Overriten in heavyRock to do more damage
  protected int damageAmount = 20;
  public virtual int DamageAmount => damageAmount;

  protected GameObject player;

  void Start()
  {
    GetComponent<SpriteRenderer>().sprite = rockSprite;
    player = GameObject.FindGameObjectWithTag("Player");
  }

  // Dynamic binding of collision behavior
  public virtual void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Boat"))
    {
      Debug.Log("Boat hit a rock!");
    }
  }

  public void DestroyRock()
  {
    Debug.Log("Rock destroyed!");
    Destroy(gameObject);
  }

}

public class HeavyRock : Rock
{
  // Different sprite for heavy rock
  [SerializeField] Sprite heavyRockSprite;

  // Dynamic binding of damage amount
  public override int DamageAmount => damageAmount * 2; // Heavy rocks do double damage

  void Start()
  {
    GetComponent<SpriteRenderer>().sprite = heavyRockSprite;
    player = GameObject.FindGameObjectWithTag("Player");
  }

  // Dynamic binding of collision behavior
  public override void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Boat"))
    {
      Debug.Log("Boat hit a heavy rock!");
    }
  }

  // Static binding of destruction behavior
  public new void DestroyRock()
  {
    Debug.Log("Heavy rock destroyed!");
    Destroy(gameObject);
  }

}