using UnityEngine;

public class Duri : MonoBehaviour
{
    public bool HancurSetelahKena = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Pemain>() != null)
        {
            collision.gameObject.GetComponent<Pemain>().Die();

            if (HancurSetelahKena)
            {
                Destroy(gameObject);
            }
        }
    }
}
