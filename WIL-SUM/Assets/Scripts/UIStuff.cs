using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStuff : MonoBehaviour
{
    public Text officerCountText;
    public Text scoreText;
    public Text crimeListText;

    public ResourceManager resourceManager;
    public CrimeManager crimeManager;
    public Money money;
    [SerializeField] TMP_Text moneyText;

    void Update()
    {
        UpdateMoneyText();
        UpdateOfficerText();
        UpdateCrimeText();
    }

    void UpdateMoneyText()
    {
        moneyText.text = $"Money: {money.moneyValue}";
    }

    void UpdateOfficerText()
    {
        officerCountText.text = $"Officers Available: {resourceManager.availableOfficers}";
        scoreText.text = $"Score: {crimeManager.score}";
    }

    void UpdateCrimeText()
    {
        crimeListText.text = "Active Crimes:\n";

        foreach (KeyValuePair<int, CrimeManager.Crime> entry in crimeManager.activeCrimes)
        {
            CrimeManager.Crime crime = entry.Value;
            if (!crime.isResolved)
            {
                crimeListText.text += $"ID: {crime.id} | {crime.crimeType} ({crime.severity})\n" +
                                      $"Officers Needed: {crime.officersRequired}\n" +
                                      $"Location: {crime.streetAddress} #{crime.streetNumber}\n" +
                                      $"Details: {crime.crimeDesc1} {crime.crimeDesc2} {crime.crimeDesc3}\n\n";
            }
        }
    }
}

