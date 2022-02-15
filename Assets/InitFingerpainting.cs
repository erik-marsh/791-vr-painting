using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InitFingerpainting : MonoBehaviour
{
    /// <summary>
    /// We need to instantiate the Fingerpainting script with network capabilities in order for color chnages to be propogated across the network.
    /// </summary>
    private void Awake()
    {
        var fingerpaint = PhotonNetwork.Instantiate(
            "Fingerpainting",
            this.transform.position,
            this.transform.rotation
        );
        fingerpaint.transform.SetParent(this.transform);
    }
}
