using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerpaintingHook : HookBase
{
    public string menuSystemName;

    protected override void Hook()
    {
        GameObject obj = GameObject.Find(menuSystemName);
        if (!obj)
        {
            Debug.LogError("FingerpaintingHook::Hook(): Could not find an object in the scene with name " + menuSystemName);
            return;
        }

        MenuController menu = obj.GetComponent<MenuController>();
        if (!menu)
        {
            Debug.LogError("FingerpaintingHook::Hook(): Object " + menuSystemName + " has no component of type MenuController");
            return;
        }

        Fingerpainting fp = GetComponent<Fingerpainting>();
        menu.fingerpainting = fp;
        menu.UpdateColor();
    }
}
