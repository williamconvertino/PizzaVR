using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Config")]
    public GameObject rayOrigin;
    public LayerMask attachableLayers;
    public float maxRayDistance = 100f;

    [Header("Display")] 
    public LineRenderer activeLinePrefab;
    public LineRenderer lockedLinePrefab;
    private LineRenderer _activeLine;
    private LineRenderer _lockedLine;
    [HideInInspector] public Vector2 JoystickValue { get; set; }
    //[HideInInspector] public Vector3 DestinationCoords { get; set; }
    [HideInInspector] public GameObject DestinationMarker { get; set; }
    [HideInInspector] public Vector3 OriginCoords { get; set;}
    [HideInInspector] public ShooterState shooterState = ShooterState.Inactive;

    public enum ShooterState
    {
        Inactive,
        Active,
        Locked
    }
    
    private void Update()
    {
        if (shooterState == ShooterState.Active && !IsValueStill(JoystickValue.y))
        {
            LockShooter();
        }

        if (shooterState == ShooterState.Active && _activeLine != null)
        {
            _activeLine.SetPosition(0, rayOrigin.transform.position);
            _activeLine.SetPosition(1, DestinationMarker.transform.position);
        }
        
        if (shooterState == ShooterState.Locked && _lockedLine != null)
        {
            _lockedLine.SetPosition(0, rayOrigin.transform.position);
            _lockedLine.SetPosition(1, DestinationMarker.transform.position);
        }
        
        if (shooterState != ShooterState.Locked)
        {
            OriginCoords = rayOrigin.transform.position;    
        }
    }

    #region Shooter Functions
    public void FireShooter()
    {
        RaycastHit hit;
        
        if (!Physics.Raycast(rayOrigin.transform.position, rayOrigin.transform.forward, out hit, maxRayDistance, attachableLayers))
        {
            return;
        }

        if (DestinationMarker != null)
        {
            Destroy(DestinationMarker);
        }

        DestinationMarker = new GameObject();
        DestinationMarker.transform.parent = hit.collider.transform;
        DestinationMarker.transform.localPosition = hit.collider.transform.InverseTransformPoint(hit.point);
        ActivateShooter();
    }

    public void ReleaseShooter()
    {
        ShooterState previousShooterState = shooterState;
        if (previousShooterState == ShooterState.Active)
        {
            DeactivateShooter();
        }

        if (previousShooterState == ShooterState.Locked)
        {
            UnlockShooter();
        }
        shooterState = ShooterState.Inactive;
    }

    //Add/remove any activated visuals
    public void ActivateShooter()
    {
        shooterState = ShooterState.Active;
        _activeLine = Instantiate(activeLinePrefab, transform.position, Quaternion.identity)
            .GetComponent<LineRenderer>();
        _activeLine.SetPosition(0, OriginCoords);
        _activeLine.SetPosition(1, DestinationMarker.transform.position);
    }
    
    public void DeactivateShooter()
    {
        shooterState = ShooterState.Inactive;
        Destroy(_activeLine.gameObject);
        _activeLine = null;
    }
    
    //Add and remove any locked visuals
    public void LockShooter()
    {
        DeactivateShooter();
        shooterState = ShooterState.Locked;
        _lockedLine = Instantiate(lockedLinePrefab, transform.position, Quaternion.identity)
            .GetComponent<LineRenderer>();
        _lockedLine.SetPosition(0, OriginCoords);
        _lockedLine.SetPosition(1, DestinationMarker.transform.position);
    }

    public void UnlockShooter()
    {
        shooterState = ShooterState.Inactive;
        Destroy(_lockedLine.gameObject);
        _lockedLine = null;
    }

    #endregion

    #region Util
    //Checks if an axis is basically not moving - useful for seeing
    //if the user is trying to move or not
    public static bool IsAxisStill(Vector2 axis)
    {
        return IsValueStill(axis.x) && IsValueStill(axis.y);
    }

    
    //Checks if a value is basically not moving
    public static bool IsValueStill(float a)
    {
        return a <= 0.02f;
    }
    #endregion
    
}
