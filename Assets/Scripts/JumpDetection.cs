using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Windows.Kinect;
using System.Threading;
using System;

public class JumpDetection : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private bool _Initialized1 = false;
    private bool _Initialized2 = false;
    public Vector3 restPoint1 = new Vector3(0, 0, 0);
    private Vector3 restPoint2 = new Vector3(0, 0, 0);
    private PlayerBehaviour player1;
    private PlayerBehaviour player2;
    public PlayerBehaviour PlayerPrefab;
    public MovingObstacle obstacle;

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

        // Add ID into the array.
        InsertIntoFirstEmptySlot(id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.material = BoneMaterial;
            #pragma warning disable CS0618 // Type or member is obsolete
            lr.SetVertexCount(2);            
            lr.SetWidth(0f, 0f);
            #pragma warning restore CS0618 // Type or member is obsolete

            jointObj.transform.localScale = new Vector3(0f, 0f, 0f);
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

    private Vector3 GetVector3FromJoint(Kinect.Joint joint, Body body)
    {
        Vector3 point = new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        // If the joint matches the tracked one.
        if (joint.JointType == Kinect.JointType.SpineShoulder)
        {
            if (PlayerIDs[0] != 0)
            {
                if (body.TrackingId == PlayerIDs[0] && !_Initialized1)
                {
                    //Set initial point.
                    restPoint1 = point;
                    _Initialized1 = true;
                }
                // Player jumped.
                if (point.y > restPoint1.y + 1)
                {
                    if (body.TrackingId == PlayerIDs[0])
                    {
                        player1.PlayerJump();
                    }
                }
            }
            // Player 2.
            if (PlayerIDs[1] != 0)
            {
                // Initializing player
                if (body.TrackingId == PlayerIDs[1] && !_Initialized2)
                {
                    //Set initial point
                    restPoint2 = point;
                    _Initialized2 = true;
                }
                // Player jumped
                if (point.y > restPoint2.y + 1)
                {
                    if (body.TrackingId == PlayerIDs[1])
                    {
                        player2.PlayerJump();
                    }
                }
            }
        }
        return point;
    }

    private void InsertIntoFirstEmptySlot(ulong val)
    {
        int index = 0;
        foreach (ulong id in PlayerIDs)
        {
            if (id == 0)
            {
                PlayerIDs[index] = val;
                switch (index)
                {
                    case 0:
                        // Spawn p1 and start the game
                        player1 = (PlayerBehaviour)Instantiate(PlayerPrefab);
                        obstacle.started = true;
                        break;
                    case 1:
                        player2 = (PlayerBehaviour)Instantiate(PlayerPrefab);
                        break;
                }
                return;
            }
            index++;
        }
    }

    private void RemoveFromSlot(ulong val)
    {
        int index = 0;
        foreach (ulong id in PlayerIDs)
        {
            if (id == val)
            {
                PlayerIDs[index] = 0;
                switch (index)
                {
                    case 0:
                        player1.RemovePlayer();
                        break;
                    case 1:
                        player2.RemovePlayer();
                        break;
                }
            }
            index++;
        }
    }
}
