using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMode : MonoSingleton<GameMode> {

    public Text label;
    public AudioSource musicPlayer;
    public AudioClip shopMusic, funMusic;
    public float shopMusicVolume = 0.7f, funMusicVolume = 1f;
    public Grid grid;
    public GameObject shopUI, funUI;
    public GameObject freeBeerButton;

    public void GoShopping() {
        label.text = "Morning";
        musicPlayer.clip = shopMusic;
        musicPlayer.volume = shopMusicVolume;
        musicPlayer.Play();

        Entrance.instance.enabled = false;
        Population.instance.enabled = false;

        grid.gameObject.SetActive(true);
        grid.Apply((x, z, s, r) => grid.SetState(x, z, 0));
        funUI.SetActive(false);
        shopUI.SetActive(true);
    }

    public void GoServing() {
        label.text = "Evening";
        musicPlayer.clip = funMusic;
        musicPlayer.volume = funMusicVolume;
        musicPlayer.Play();

        Tension.instance.Reset();

        Entrance.instance.enabled = true;
        Population.instance.enabled = false;

        grid.gameObject.SetActive(false);
        shopUI.SetActive(false);
        funUI.SetActive(true);
        freeBeerButton.SetActive(true);

        foreach (var club in FindObjectsOfType<GenericClub>()) {
            club.GetComponentInChildren<Collider>().enabled = false;
            club.GetComponentInChildren<Rigidbody>().isKinematic = true;
        }
    }

    public void GoBrawling() {
        Entrance.instance.enabled = false;
        Population.instance.enabled = true;
        StartCoroutine(DoEnrageFolks());
    }

    private IEnumerator DoEnrageFolks() {
        foreach (var prop in FindObjectsOfType<Destructible>())
            prop.GetComponent<Rigidbody>().isKinematic = false;

        foreach (var club in FindObjectsOfType<GenericClub>()) {
            club.GetComponentInChildren<Collider>().enabled = true;
            club.GetComponentInChildren<Rigidbody>().isKinematic = false;
        }

        foreach (var drinker in FindObjectsOfType<BeerDrinker>())
            Destroy(drinker);

        foreach (var visitor in FindObjectsOfType<Visitor>()) {
            visitor.Enrage();
            //yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
        yield break;
    }
}
