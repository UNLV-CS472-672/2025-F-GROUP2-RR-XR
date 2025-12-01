using UnityEngine;
using Immersal;

public class DisableOnLocalization : MonoBehaviour
{
    private ImmersalSDK sdk;
    private bool hasDeactivated = false;

    void Start()
    {
        sdk = ImmersalSDK.Instance;
    }

    void Update()
    {
        if (sdk == null || hasDeactivated)
            return;

        var status = sdk.TrackingStatus;
        if (status != null)
        {
            // Check if we have at least 1 successful localization
            if (status.LocalizationSuccessCount > 0)
            {
                // Disable this object (your GIF object)
                gameObject.SetActive(false);
                hasDeactivated = true;
            }
        }
    }
}
