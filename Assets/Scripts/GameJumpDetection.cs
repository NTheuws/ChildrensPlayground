using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Windows.Kinect;
using System.Threading;
using System;
using Unity.VisualScripting;
using System.Drawing;
using Color = UnityEngine.Color;

public class GameJumpDetection : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private bool _Initialized1 = false;
    private bool _Initialized2 = false;
    private bool _Initialized3 = false;
    private bool _Initialized4 = false;
    public Vector3 restPoint1 = new Vector3(0, 0, 0);
    private Vector3 restPoint2 = new Vector3(0, 0, 0);
    private Vector3 restPoint3 = new Vector3(0, 0, 0);
    private Vector3 restPoint4 = new Vector3(0, 0, 0);
    public PlayerBehaviour player1;
    public PlayerBehaviour player2;
    public PlayerBehaviour player3;
    public PlayerBehaviour player4;
    public PlayerBehaviour playerPrefab1;
    public PlayerBehaviour playerPrefab2;
    public PlayerBehaviour playerPrefab3;
    public PlayerBehaviour playerPrefab4;

    private List<ulong> _PlayerIds = new List<ulong>();
    private ulong[] PlayerIDs = { 0, 0, 0, 0 };


    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private void Start()
    {
        BodySourceManager = GameObject.FindGameObjectWithTag("SkeletonTracker");
    }
    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);

                // Remove the player and character.
                RemoveFromSlot(trackingId);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        // Add ID into the array and spawn in player.
        //InsertIntoFirstEmptySlot(id);
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.material = BoneMaterial;
#pragma warning disable CS0618 // Type or member is obsolete
            lr.SetVertexCount(2);
            lr.SetWidth(0f, 0f);
            //lr.SetWidth(0.05f, 0.05f);
#pragma warning restore CS0618 // Type or member is obsolete
            jointObj.transform.localScale = new Vector3(0f, 0f, 0f);
            //jointObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint, body);

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value, body));
#pragma warning disable CS0618 // Type or member is obsolete
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }
    // Get the placement of the joints and detect if a player has jumped or not.
    private Vector3 GetVector3FromJoint(Kinect.Joint joint, Body body)
    {
        Vector3 point = new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        // If the joint matches the tracked one.
        if (joint.JointType == Kinect.JointType.SpineShoulder)
        {
            // Check if the ID is known.
            bool isKnown = false;
            foreach (ulong player in PlayerIDs)
            {
                if (player == body.TrackingId)
                {
                    isKnown = true;
                }
            }
            // When the skeleton has been detected for the first time.
            if (!isKnown)
            {
                // Most left / player 1.
                if (joint.Position.X >= -1.1 && joint.Position.X < -0.7)
                {
                    // First time registering.
                    if (!_Initialized1)
                    {
                        // Set initial point.
                        restPoint1 = point;
                        _Initialized1 = true;
                        // Link player to correct playerslot.
                        PlayerIDs[0] = body.TrackingId;
                        // Create player.
                        player1 = (PlayerBehaviour)Instantiate(playerPrefab1);
                        Vector2 spawnPoint = new Vector3(GameObject.Find("Camera").GetComponent<BackGroundLoop>().spawnPoint.transform.position.x, player1.transform.position.y, player1.transform.position.z);
                        player1.transform.position = spawnPoint;
                    }
                }
                // 2nd left / player 2.
                else if (joint.Position.X >= -0.7 && joint.Position.X < -0.3)
                {
                    if (!_Initialized2)
                    {
                        // Set initial point.
                        restPoint2 = point;
                        _Initialized2 = true;
                        // Link player to correct playerslot.
                        PlayerIDs[1] = body.TrackingId;
                        // Create player.
                        player2 = (PlayerBehaviour)Instantiate(playerPrefab2);
                        Vector2 spawnPoint = new Vector3(GameObject.Find("Camera").GetComponent<BackGroundLoop>().spawnPoint.transform.position.x, player2.transform.position.y, player2.transform.position.z);
                        player2.transform.position = spawnPoint;
                    }
                }
                // 2nd right / player 3.
                else if (joint.Position.X >= -0.3 && joint.Position.X < 0.1)
                {
                    if (!_Initialized3)
                    {
                        // Set initial point.
                        restPoint3 = point;
                        _Initialized3 = true;
                        // Link player to correct playerslot.
                        PlayerIDs[2] = body.TrackingId;
                        // Create player.
                        player3 = (PlayerBehaviour)Instantiate(playerPrefab3);
                        Vector3 spawnPoint = new Vector3(GameObject.Find("Camera").GetComponent<BackGroundLoop>().spawnPoint.transform.position.x, player3.transform.position.y, player3.transform.position.z);
                        player3.transform.position = spawnPoint;
                    }
                }
                // Most right / player 4.
                else if (joint.Position.X >= 0.1 && joint.Position.X <= 0.5)
                {
                    if (!_Initialized4)
                    {
                        // Set initial point.
                        restPoint4 = point;
                        _Initialized4 = true;
                        // Link player to correct playerslot.
                        PlayerIDs[3] = body.TrackingId;
                        // Create player.
                        player4 = (PlayerBehaviour)Instantiate(playerPrefab4);
                        Vector2 spawnPoint = new Vector3(GameObject.Find("Camera").GetComponent<BackGroundLoop>().spawnPoint.transform.position.x, player4.transform.position.y, player4.transform.position.z);
                        player4.transform.position = spawnPoint;
                    }
                }

                if (_Initialized1 || _Initialized2 || _Initialized3 || _Initialized4)
                {
                    // Start the game.
                    BackGroundLoop background = (BackGroundLoop)FindObjectOfType<BackGroundLoop>();
                    background.GameStarted();
                }
            }

            // Move players.
            if (body.TrackingId == PlayerIDs[0])
            {
                MovementDefiner(restPoint1, point, player1);
            }
            else if (body.TrackingId == PlayerIDs[1])
            {
                MovementDefiner(restPoint2, point, player2);
            }
            else if (body.TrackingId == PlayerIDs[2])
            {
                MovementDefiner(restPoint3, point, player3);
            }
            else if (body.TrackingId == PlayerIDs[3])
            {
                MovementDefiner(restPoint4, point, player4);
            }
        }
        return point;
    }

    private void MovementDefiner(Vector3 restPoint, Vector3 point,PlayerBehaviour player)
    {
        if (player != null)
        {
            // Vertical movement.
            // jump.
            if (point.y > restPoint.y + 1)
            {
                player.PlayerJump();
            }
            // Crouch.
            else if (point.y < restPoint.y - 1.5)
            {
                player.PlayerCrouch();
            }
            // Horizontal movement.
            // Moving left.
            if (point.x < restPoint.x - 1)
            {
                player.isMovingLeft = true;
                player.isMovingRight = false;
            }
            // Moving right.
            else if (point.x > restPoint.x + 1)
            {
                player.isMovingLeft = false;
                player.isMovingRight = true;
            }
            // Standing still.
            else
            {
                player.isMovingLeft = false;
                player.isMovingRight = false;
            }
        }
    }

    // Called when a skeleton is no longer detected.
    // IN: val being the skeletons ID
    private void RemoveFromSlot(ulong val)
    {
        int index = 0;
        foreach (ulong id in PlayerIDs)
        {
            // If the ID matches the slot of the array.
            if (id == val)
            {
                PlayerIDs[index] = 0;
                // Remove the associated player.
                switch (index)
                {
                    case 0:
                        player1.RemovePlayer();
                        player1 = null;
                        _Initialized1 = false;
                        break;
                    case 1:
                        player2.RemovePlayer();
                        player2 = null;
                        _Initialized2 = false;
                        break;
                    case 2:
                        player3.RemovePlayer();
                        player3 = null;
                        _Initialized3 = false;
                        break;
                    case 3:
                        player4.RemovePlayer();
                        player4 = null;
                        _Initialized4 = false;
                        break;
                }
            }
            index++;
        }
    }
}
