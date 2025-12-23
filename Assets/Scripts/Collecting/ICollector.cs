using UnityEngine;

public interface ICollector
{
    bool Consume(ICollectable collectable);

    Transform CollectorTransform { get; }
}
