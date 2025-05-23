using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    [Header("Rigs visuels")]
    public GameObject normalRig;
    public GameObject rareRig;

    [Header("Chance")]
    [Tooltip("1 chance sur ce nombre d'activer le rig rare")]
    public int chanceValue = 416;

    private Animator currentAnimator;
    private bool hasSwitchedToRare = false;

    void Start()
    {
        // Active le rig normal au démarrage
        if (normalRig != null) normalRig.SetActive(true);
        if (rareRig != null) rareRig.SetActive(false);

        // Prend l’Animator du rig normal
        currentAnimator = normalRig != null ? normalRig.GetComponent<Animator>() : null;
    }

    public void TryTriggerRareVisual()
    {
        if (hasSwitchedToRare) return;

        // 🎲 Proba 1 sur 416
        int roll = Random.Range(1, chanceValue + 1);
        if (roll == 1)
        {
            Debug.Log("🔥 MODE 416 ACTIVÉ - Changement de rig !");

            if (normalRig != null) normalRig.SetActive(false);
            if (rareRig != null) rareRig.SetActive(true);

            currentAnimator = rareRig != null ? rareRig.GetComponent<Animator>() : null;
            hasSwitchedToRare = true;
        }
    }

    public Animator GetCurrentAnimator()
    {
        return currentAnimator;
    }

    public bool IsInRareMode()
    {
        return hasSwitchedToRare;
    }
}
