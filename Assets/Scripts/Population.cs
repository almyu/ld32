using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Population : MonoSingleton<Population> {

    public GameObject sellEffect;
    public UnityEvent onDone;

    private bool working = false;

    private void Update() {
        if (working) return;

        var factionCount = 0;

        for (int i = 0; i < Target.numFactions; ++i)
            if (Target.targets[i].Count > 0)
                ++factionCount;

        if (factionCount == 1 || Weapon.existingWeaponCount == 0) StartCoroutine(DoEndBrawl());
        if (factionCount == 0) StartCoroutine(DoLoot());
    }

    private IEnumerator DoEndBrawl() {
        working = true;
        yield return new WaitForSeconds(1f);

        foreach (var visitor in FindObjectsOfType<Visitor>())
            visitor.Leave();

        working = false;
    }

    private IEnumerator DoLoot() {
        yield return StartCoroutine(DoLootCorpses());
        yield return StartCoroutine(DoSellFurniture());
        onDone.Invoke();
    }

    private IEnumerator DoLootCorpses() {
        working = true;
        yield return new WaitForSeconds(1f);

        foreach (var body in FindObjectsOfType<DeadBody>()) {
            body.Loot();
            yield return new WaitForSeconds(0.5f);
        }
        working = false;
    }

    private IEnumerator DoSellFurniture() {
        working = true;
        yield return new WaitForSeconds(2f);

        foreach (var piece in FindObjectsOfType<Furniture>()) {
            Cash.instance.transactionPosition = piece.transform.position;
            Cash.instance.Earn(piece.cost);
            Instantiate(sellEffect, piece.transform.position, Quaternion.AngleAxis(Random.value * 360f, Vector3.up));
            Destroy(piece.gameObject);
            yield return new WaitForSeconds(0.5f);
        }
        working = false;
    }
}
