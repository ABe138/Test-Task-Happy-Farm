using UnityEngine;

public interface IHarvestable
{
    bool Harvest(Vector3 hitFrom);

    Vector3 GetPosition();
}
