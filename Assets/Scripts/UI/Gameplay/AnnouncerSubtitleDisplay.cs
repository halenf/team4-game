// Announcer Subtitle Display - Halen
// Shows announcer quotes during gameplay.
// Last edit: 24/11/23

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncerSubtitleDisplay : MonoBehaviour
{
    public TMP_Text subtitleDisplay;
    public GameObject display;
    public float displayTime;

    [Header("Round Quotes")]
    public string[] beforeRoundQuotes;
    public string[] endRoundQuotes;

    [Header("Player Death Quotes")]
    public string[] bulletQuotes;
    public string[] spikeballQuotes;
    public string[] fireQuotes;
    public string[] explosionQuotes;

    public enum AnnouncementType
    {
        BeforeRound,
        DeathBullet,
        DeathExplosion,
        DeathFire,
        DeathZapper,
        DeathSpikeball,
        EndRound
    }

    public void StartAnnouncement(AnnouncementType announcementType)
    {
        display.SetActive(false);
        subtitleDisplay.gameObject.SetActive(false);
        subtitleDisplay.gameObject.SetActive(true);
        string chosenText = "";

        // randomly select a string from the specifided announcement type list
        switch (announcementType)
        {
            case AnnouncementType.BeforeRound:
                chosenText = beforeRoundQuotes[Random.Range(0, beforeRoundQuotes.Length)];
                break;
            case AnnouncementType.EndRound:
                chosenText = endRoundQuotes[Random.Range(0, endRoundQuotes.Length)];
                break;
            case AnnouncementType.DeathBullet:
                chosenText = bulletQuotes[Random.Range(0, bulletQuotes.Length)];
                break;
            case AnnouncementType.DeathExplosion:
                chosenText = explosionQuotes[Random.Range(0, explosionQuotes.Length)];
                break;
            case AnnouncementType.DeathFire:
                chosenText = fireQuotes[Random.Range(0, fireQuotes.Length)];
                break;
            case AnnouncementType.DeathSpikeball:
                chosenText = spikeballQuotes[Random.Range(0, spikeballQuotes.Length)];
                break;
        }

        // display the chosen string and play the talking sound effect
        subtitleDisplay.text = chosenText;
        //SoundManager.Instance.PlaySound("Announcer/VA-ROBOTCHATTERLONGER");
    }

    public void StopText()
    {
        subtitleDisplay.gameObject.SetActive(false);
        display.SetActive(true);
    }
}
