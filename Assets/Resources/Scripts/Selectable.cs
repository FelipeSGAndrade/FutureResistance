using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    private List<BlockBehaviour> interactions = new List<BlockBehaviour>();
    private Buildable buildable;
    
    void Start() {
        buildable = GetComponent<Buildable>();
    }

    public void Select() {
        if (buildable && !buildable.complete)
            return;

        UIController.instance.ShowBlockUI(gameObject, this);
    }

    public void AddInteraction(BlockBehaviour behaviour) {
        interactions.Add(behaviour);
    }

    public List<BlockBehaviour> GetInteractions() {
        return interactions;
    }
}
