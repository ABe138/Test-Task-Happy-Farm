using UnityEngine;

public interface ICollectable
{
    string Id { get; }

    void Collect(ICollector collector);

    Vector3 GetPosition();
}
