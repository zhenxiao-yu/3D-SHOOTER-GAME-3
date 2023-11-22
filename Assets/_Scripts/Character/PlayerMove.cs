using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PlayerMove : MonoBehaviour
{
    public static System.Action OnLevelFinished = delegate { };

    [SerializeField] PathCreator path; // Reference to the path
    [SerializeField] EndOfPathInstruction endOfPath;
    [SerializeField] float speed = 3f; // Camera movement speed
    [SerializeField] bool isMoving = true;
    [SerializeField] ShootOutEntry[] shootOutEntries; // List of shoot-out points

    [Header("Debug Options")]
    [SerializeField] float previewDistance = 0f; // Debugging variable
    [SerializeField] bool enableDebug;

    private float distanceTravelled;
    private int areaCleared;

    void Start()
    {
        InitializeShootOutPoints(); // Initialize shoot-out points
    }

    void Update()
    {
        if (path != null && isMoving)
        {
            MoveAlongPath(); // Move the camera along the path
            CheckShootOutPoints(); // Check if the camera has reached a shoot-out point
        }
    }

    private void InitializeShootOutPoints()
    {
        foreach (var entry in shootOutEntries)
        {
            entry.shootOutPoint.Initialize(this); // Initialize each shoot-out point
        }
    }

    private void MoveAlongPath()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = path.path.GetPointAtDistance(distanceTravelled, endOfPath);
        transform.rotation = path.path.GetRotationAtDistance(distanceTravelled, endOfPath);
    }

    private void CheckShootOutPoints()
    {
        for (int i = 0; i < shootOutEntries.Length; i++)
        {
            var shootOutEntry = shootOutEntries[i];
            if ((path.path.GetPointAtDistance(shootOutEntry.distance) - transform.position).sqrMagnitude < 0.01f)
            {
                if (shootOutEntry.shootOutPoint.AreaCleared)
                    return;

                if (isMoving)
                    shootOutEntry.shootOutPoint.StartShootOut(shootOutEntry.areaTimer);
            }
        }
    }

    private void OnValidate()
    {
        if (enableDebug)
        {
            MoveToPreviewDistance(); // Move to the preview distance for debugging
        }
    }

    private void MoveToPreviewDistance()
    {
        transform.position = path.path.GetPointAtDistance(previewDistance, endOfPath);
        transform.rotation = path.path.GetRotationAtDistance(previewDistance, endOfPath);
    }

    public void AreaCleared()
    {
        areaCleared++;

        if (areaCleared == shootOutEntries.Length)
        {
            OnLevelFinished(); // Trigger the level finished event if all areas are cleared
            return;
        }

        SetPlayerMovement(true);
    }

    public void SetPlayerMovement(bool isEnabled)
    {
        isMoving = isEnabled; // Enable or disable camera movement
    }
}

[System.Serializable]
public class ShootOutEntry
{
    public ShootOutPoint shootOutPoint;
    public float distance;
    public float areaTimer = 15f; // Time limit for the shoot-out area
}
