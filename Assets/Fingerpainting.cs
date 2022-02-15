using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Photon.Pun;

public class Fingerpainting : MonoBehaviourPun
{
    public Color paintColor;
    public InputActionProperty paintAction;
    public InputActionProperty eraseAction;
    public GameObject voxelBrush;

    private VoxelMap map = new VoxelMap();

    private void Start()
    {
        Vector3[] corners = new Vector3[4]
        {
            map.origin,
            map.origin + new Vector3(map.bounds, 0, 0),
            map.origin + new Vector3(0, 0, map.bounds),
            map.origin + new Vector3(map.bounds, 0, map.bounds),
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject obj = new GameObject();
            var line = obj.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, corners[i]);
            line.SetPosition(1, corners[i] + new Vector3(0, map.bounds, 0));
            line.SetWidth(0.01f, 0.01f);
            obj.name = "Boundary Line";
        }
    }

    private void Update()
    {
        if (photonView.IsMine && paintAction.action.triggered)
        {
            Vector3Int voxel = map.getVoxelIndex(transform.position);
            if (!map.isVoxelUsed(transform.position))
            {
                Debug.Log(voxel);
                Vector3 normalizedPosition = new Vector3(
                    voxel.x * VoxelMap.voxelSize,
                    voxel.y * VoxelMap.voxelSize,
                    voxel.z * VoxelMap.voxelSize
                );

                var paintVoxel = Instantiate(voxelBrush);
                paintVoxel.transform.position = normalizedPosition;
                paintVoxel.transform.localScale = new Vector3(VoxelMap.voxelSize, VoxelMap.voxelSize, VoxelMap.voxelSize);
                paintVoxel.GetComponent<MeshRenderer>().material.SetColor("_Color", paintColor);

                map.setVoxel(voxel, true);
                map.objects[voxel.x][voxel.y][voxel.z] = paintVoxel;
            }
        }
        else if (photonView.IsMine && eraseAction.action.triggered)
        {
            Vector3Int voxel = map.getVoxelIndex(transform.position);
            if (map.isVoxelUsed(transform.position))
            {
                Debug.Log("Erasing " + voxel);
                map.setVoxel(voxel, false);
                GameObject obj = map.objects[voxel.x][voxel.y][voxel.z];
                map.objects[voxel.x][voxel.y][voxel.z] = null;
                Destroy(obj);
            }
        }
    }

    public void ChangeColor(Color c)
    {
        paintColor = c;
        photonView.RPC("RPC_ReceiveColorChange", RpcTarget.Others, c.r, c.g, c.b, c.a);
    }

    [PunRPC]
    private void RPC_ReceiveColorChange(float r, float g, float b, float a)
    {
        paintColor = new Color(r, g, b, a);
    }

    [PunRPC]
    private void RPC_SetVoxelUsed(int x, int y, int z)
    {
        
    }
}
