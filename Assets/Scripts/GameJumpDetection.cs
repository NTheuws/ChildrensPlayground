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
    public Vector3 restPoint1 = new Vector3(0, 0, 0);
    private Vector3 restPoint2 = new Vector3(0, 0, 0);
    public PlayerBehaviour player1;
    public PlayerBehaviour player2;
    public PlayerBehaviour playerPrefab1;
    public PlayerBehaviour playerPrefab2;

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
        InsertIntoFirstEmptySlot(id);
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
            //// Player 1.
            //if (PlayerIDs[0] != 0 && player1 != null)
            //{
            //    if (body.TrackingId == PlayerIDs[0] && !_Initialized1)
            //    {
            //        //Set initial point.
            //        restPoint1 = point;
            //        _Initialized1 = true;
            //    }
            //    MovementDefiner(restPoint1, point, player1);
            //}
            //// Player 2.
            //if (PlayerIDs[1] != 0 && player2 != null)
            //{
            //    if (body.TrackingId == PlayerIDs[0] && !_Initialized1)
            //    {
            //        //Set initial point.
            //        restPoint2 = point;
            //        _Initialized2 = true;
            //    }
            //    MovementDefiner(restPoint2, point, player2);
            //}
            // Player 1.
            if (body.TrackingId == PlayerIDs[0])
            {
                if (PlayerIDs[0] != 0 && player1 != null)
                {
                    if (!_Initialized1)
                    {
                        //Set initial point.
                        restPoint1 = point;
                        _Initialized1 = true;
                    }
                    MovementDefiner(restPoint1, point, player1);
                }
            }
            else if (body.TrackingId == PlayerIDs[1])
            {
                if (PlayerIDs[1] != 0 && player2 != null)
                {
                    if (!_Initialized2)
                    {
                        //Set initial point.
                        restPoint2 = point;
                        _Initialized2 = true;
                    }
                    MovementDefiner(restPoint2, point, player2);
                }
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

    /* These methods are used to track the skeletons and link them to the correct player.
     * When spawning in a skeleton it'll have a personal ID linked to it.
     * This ID will be saved in an array slot which corresponds to a character.
     */

    // Called when a new skeleton has been detected.
    // in: val being the skeleton ID
    private void InsertIntoFirstEmptySlot(ulong val)
    {
        int index = 0;
        foreach (ulong id in PlayerIDs)
        {
            // Check for the first spot of the array thats emtpy.
            if (id == 0)
            {
                // Insert the skeleton's ID into the array.
                PlayerIDs[index] = val;
                switch (index)
                {
                    case 0:
                        // Spawn p1 and start the game
                        player1 = (PlayerBehaviour)Instantiate(playerPrefab1);
                        player1.transform.position = new Vector2(player1.transform.position.x + 4f, player1.transform.position.y);
                        // Start moving the game.
                        BackGroundLoop background = (BackGroundLoop)FindObjectOfType<BackGroundLoop>();
                        background.GameStarted();
                        break;
                    case 1:
                        // Spawn p2.
                        player2 = (PlayerBehaviour)Instantiate(playerPrefab2);
                        player2.transform.position = new Vector2(player1.transform.position.x + 4f, player1.transform.position.y);
                        break;
                }
                return;
            }
            index++;
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
                }
            }
            index++;
        }
    }
}
