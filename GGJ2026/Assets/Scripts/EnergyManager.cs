using GabUnity;
using UnityEngine;
using UnityEngine.Events;

public class EnergyManager : MonoSingleton<EnergyManager>
{
    [SerializeField] private float cur_energy;
    public static float CurEnergy => Instance.cur_energy;

    [SerializeField] private float refil_per_sec;
    [SerializeField] private UnityEvent<float> onChangeEnergy;

    private void Update()
    {
        cur_energy += refil_per_sec * Time.deltaTime;

        cur_energy = Mathf.Clamp(cur_energy, 0, 1);

        onChangeEnergy.Invoke(cur_energy);
    }

    public static bool TryUseEnergy(float cost)
    {
        if (Instance.cur_energy > cost)
        {
            Instance.cur_energy -= cost;
            return true;
        }

        return false;
    }
}
