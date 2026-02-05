using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    private PlayerStats playerStats;
    
    private bool isSignedIn;

    /*private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await SignIn();
        playerStats = FindObjectOfType<PlayerStats>();

        if (isSignedIn)
        {
            await LoadCloud();
        }
    }*/

    private async Task SignIn()
    {
        await SignInAnonymous();
    }

    private async Task SignInAnonymous()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            print("Signed In");
            print("Player ID: " + AuthenticationService.Instance.PlayerId);
            isSignedIn = true;  
        }
        catch (AuthenticationException e)
        {
            print("Signing in failed");
            Debug.Log(e);
            isSignedIn = false; 
        }
    }

    public async void SaveCloud()
    {
        var data = new Dictionary<string, object> { { "Money", playerStats.Money } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }

    private async Task LoadCloud()
    {
        Dictionary<string, string> serverData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "Money" });

        if (serverData.TryGetValue("Money", out var value))
        {
            if (float.TryParse(value, out float moneyValue))
            {
                playerStats.Money = moneyValue;
            }
            else
            {
                Debug.LogWarning("Money data parsing error");
            }
        }
        else
        {
            Debug.LogWarning("Key 'Money' not found in Cloud Save data");
        }
    }
}