﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.iOS; //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.

public class WRLDARKitAnchorHandler : MonoBehaviour 
{
    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    private UnityARAnchorManager m_unityARAnchorManager;

    public Transform wrldMapParent;

    public Transform wrldMapMask;

    private WRLDARStreamingCameraHandler m_arStreamingController;

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    private Dictionary<string, ARPlaneAnchor> m_planeAnchorMap;
    private ARPlaneAnchor m_currentAnchor;
    private bool m_hasFoundAnchor = false;

    void Start()
    {
        m_arStreamingController = GameObject.FindObjectOfType<WRLDARStreamingCameraHandler> ();

        //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
        m_planeAnchorMap = new Dictionary<string, ARPlaneAnchor> ();

        //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
    }

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        m_planeAnchorMap.Add (arPlaneAnchor.identifier, arPlaneAnchor);

        if (m_hasFoundAnchor == false) 
        {
            m_hasFoundAnchor = true;
            m_currentAnchor = arPlaneAnchor;
            UpdateMapPositionWithAnchor (m_currentAnchor);
            UpdateMapMaskWithAnchor (m_currentAnchor);
        }
    }

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (m_planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) 
        {
            m_planeAnchorMap.Remove (arPlaneAnchor.identifier);
        }

        if (m_hasFoundAnchor && arPlaneAnchor.identifier == m_currentAnchor.identifier) 
        {
            if (m_planeAnchorMap.Count > 0) 
            {
                m_currentAnchor = m_planeAnchorMap.Values.First ();
                UpdateMapMaskWithAnchor (m_currentAnchor);
            }
            else
            {
                m_hasFoundAnchor = false;
            }
        }
    }

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (m_planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) 
        {
            m_planeAnchorMap [arPlaneAnchor.identifier] = arPlaneAnchor;
        }

        if (m_hasFoundAnchor && arPlaneAnchor.identifier == m_currentAnchor.identifier) 
        {
            m_currentAnchor = arPlaneAnchor;
            UpdateMapMaskWithAnchor (m_currentAnchor);
        }
    }

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    void UpdateMapPositionWithAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        //Setting the position of our map to match the position of anchor
        wrldMapParent.position = UnityARMatrixOps.GetPosition (arPlaneAnchor.transform);
    }

    //If you are seeing error on this line then kindly import Unity ARKit Plugin to remove this error.
    void UpdateMapMaskWithAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        //Setting the position of our map to match the position of anchor
        wrldMapMask.parent.position = UnityARMatrixOps.GetPosition (arPlaneAnchor.transform);

        //Updating our mask according to the ARKit anchor
        wrldMapMask.parent.rotation = UnityARMatrixOps.GetRotation (arPlaneAnchor.transform);

        wrldMapMask.localPosition = new Vector3(arPlaneAnchor.center.x, arPlaneAnchor.center.y, -arPlaneAnchor.center.z);
        wrldMapMask.localScale  = new Vector3(arPlaneAnchor.extent.x, wrldMapMask.localScale.y, arPlaneAnchor.extent.z);

        m_arStreamingController.UpdateStreamingCamera ();
    }

    public void ForceRepositionMap()
    {
        if (m_hasFoundAnchor) 
        {
            UpdateMapPositionWithAnchor (m_currentAnchor);
            UpdateMapMaskWithAnchor (m_currentAnchor);
        }
    }
}