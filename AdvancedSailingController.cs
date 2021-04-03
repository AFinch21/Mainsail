// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

// Shout out to @holdingjason who posted a first version of this script here: https://github.com/huwb/crest-oceanrender/pull/100

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Crest
{
    /// <summary>
    /// Boat physics by sampling at multiple probe points.
    /// </summary>
    public class AdvancedSailingController : FloatingObjectBase
    {
        [Header("Forces")]
        [Tooltip("Override RB center of mass, in local space."), SerializeField]
        Vector3 _centerOfMass = Vector3.zero;
        [SerializeField, FormerlySerializedAs("ForcePoints")]
        FloaterForcePoints[] _forcePoints = new FloaterForcePoints[] { };

        [Tooltip("Vertical offset for where engine force should be applied."), SerializeField]
        float _forceHeightOffset = 0f;
        [SerializeField]
        float _forceMultiplier = 10f;
        [Tooltip("Width dimension of boat. The larger this value, the more filtered/smooth the wave response will be."), SerializeField]
        float _minSpatialLength = 12f;
        [SerializeField, Range(0, 1)]
        float _turningHeel = 0.35f;

        [Header("Drag")]
        [SerializeField]
        float _dragInWaterUp = 3f;
        [SerializeField]
        float _dragInWaterRight = 2f;
        [SerializeField]
        float _dragInWaterForward = 1f;

        [Header("Propulsion")]
        [SerializeField, FormerlySerializedAs("EnginePower")]
        public bool controlsEnabled;
        public bool Engine;
        public bool SailPower;
        float _enginePower = 7;
        [SerializeField, FormerlySerializedAs("TurnPower")]
        float _turnPower = 0.5f;
        [SerializeField]
        bool _playerControlled = true;
        [Tooltip("Used to automatically add throttle input"), SerializeField]
        float _engineBias = 0f;
        [Tooltip("Used to automatically add turning input"), SerializeField]
        float _turnBias = 0f;

        [Header("Sailing")]
        [SerializeField]

        public GameObject hull;
        public GameObject sail;
        public GameObject boom;
        public GameObject mast;
        public float windDirection;
        public float sailAngle;
        private float hullAngle;
        private float sailWindAngle;
        private float hullWindAngle;
        private float sailHullDif;
        public float sailPower;
        public float windSpeed;
        private float windSpeedUpper;
        private float windSpeedLower;
        public float keelWeight = 1000f;
        public float heel = 99999f;
        private float roll;
        public float clothWindStrength = 100f;

        [Header("UI")]
        [SerializeField]
        public Text sailPowerPerc;
        public Text boatSpeedText;
        public Text boatHeading;
        public Text windDirectionText;
        public Text windSpeedText;

        [Header("Zoomed UI")]
        [SerializeField]
        public Text boatHeadingZoomed;
        public Text boatSpeedZoomed;
        public Text windSpeedZoomed;

        [Header("Water Effects")]
        [SerializeField]
        public GameObject waterSplashEffects;

        private const float WATER_DENSITY = 1000;

        public override Vector3 Velocity => _rb.velocity;

        Rigidbody _rb;

        public override float ObjectWidth { get { return _minSpatialLength; } }
        public override bool InWater { get { return true; } }

        float _totalWeight;

        Vector3[] _queryPoints;
        Vector3[] _queryResultDisps;
        Vector3[] _queryResultVels;

        SampleFlowHelper _sampleFlowHelper = new SampleFlowHelper();

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.centerOfMass = _centerOfMass;

            if (OceanRenderer.Instance == null)
            {
                enabled = false;
                return;
            }

            CalcTotalWeight();

            _queryPoints = new Vector3[_forcePoints.Length + 1];
            _queryResultDisps = new Vector3[_forcePoints.Length + 1];
            _queryResultVels = new Vector3[_forcePoints.Length + 1];

            windSpeedUpper = windSpeed + 5;
            windSpeedLower = windSpeed - 5;
        }

        void CalcTotalWeight()
        {
            _totalWeight = 0f;
            foreach (var pt in _forcePoints)
            {
                _totalWeight += pt._weight;
            }
        }

        private void FixedUpdate()
        {
#if UNITY_EDITOR
            // Sum weights every frame when running in editor in case weights are edited in the inspector.
            CalcTotalWeight();
#endif

            if (OceanRenderer.Instance == null)
            {
                return;
            }

            var collProvider = OceanRenderer.Instance.CollisionProvider;

            // Do queries
            UpdateWaterQueries(collProvider);

            var undispPos = transform.position - _queryResultDisps[_forcePoints.Length];
            undispPos.y = OceanRenderer.Instance.SeaLevel;

            var waterSurfaceVel = _queryResultVels[_forcePoints.Length];

            {
                _sampleFlowHelper.Init(transform.position, _minSpatialLength);
                _sampleFlowHelper.Sample(out var surfaceFlow);
                waterSurfaceVel += new Vector3(surfaceFlow.x, 0, surfaceFlow.y);
            }



            // Buoyancy
            windSpeedRandomiser();
            FixedUpdateBuoyancy();
            FixedUpdateDrag(waterSurfaceVel);
            FixedUpdatePropulsion();
            sailAngleCalcs();
            boatControls();
            hullHeel();
            showUI();
            effectsController();


        }


        void UpdateWaterQueries(ICollProvider collProvider)
        {
            // Update query points
            for (int i = 0; i < _forcePoints.Length; i++)
            {
                _queryPoints[i] = transform.TransformPoint(_forcePoints[i]._offsetPosition + new Vector3(0, _centerOfMass.y, 0));
            }
            _queryPoints[_forcePoints.Length] = transform.position;

            collProvider.Query(GetHashCode(), ObjectWidth, _queryPoints, _queryResultDisps, null, _queryResultVels);
        }

        void FixedUpdatePropulsion()
        {

            if (Engine)
            {
                var forcePosition = _rb.position;

                var forward = _engineBias;
                if (_playerControlled) forward += Input.GetAxis("Vertical");
                _rb.AddForceAtPosition(transform.right * _enginePower * forward, forcePosition, ForceMode.Acceleration);

                if (controlsEnabled)
                {
                    var sideways = _turnBias;
                    if (_playerControlled) sideways += (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
                    var rotVec = transform.up + _turningHeel * transform.forward;
                    _rb.AddTorque(rotVec * _turnPower * sideways, ForceMode.Acceleration);
                }
            }

            if (SailPower)
            {
                var forcePosition = _rb.position + _forceHeightOffset * Vector3.up;

                float rawForward = sailPower * windSpeed;

                _rb.AddForceAtPosition(-(transform.right * rawForward), forcePosition, ForceMode.Acceleration);

                if (controlsEnabled)
                {
                    float reverseMultiplier = (rawForward < 0f ? -1f : 1f);

                    float sideways = _turnBias;
                    if (_playerControlled) sideways +=
                            (Input.GetKey(KeyCode.A) ? reverseMultiplier * -1f : 0f) +
                            (Input.GetKey(KeyCode.D) ? reverseMultiplier * 1f : 0f);
                    var rotVec = transform.up + _turningHeel * transform.forward;
                    _rb.AddTorque(rotVec * _turnPower * sideways, ForceMode.Acceleration);
                }
            }
        }

        void FixedUpdateBuoyancy()
        {
            var archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y);

            for (int i = 0; i < _forcePoints.Length; i++)
            {
                var waterHeight = OceanRenderer.Instance.SeaLevel + _queryResultDisps[i].y;
                var heightDiff = waterHeight - _queryPoints[i].y;
                if (heightDiff > 0)
                {
                    _rb.AddForceAtPosition(archimedesForceMagnitude * heightDiff * Vector3.up * _forcePoints[i]._weight * _forceMultiplier / _totalWeight, _queryPoints[i]);
                }
            }
        }

        void FixedUpdateDrag(Vector3 waterSurfaceVel)
        {
            // Apply drag relative to water
            var _velocityRelativeToWater = _rb.velocity - waterSurfaceVel;

            var forcePosition = _rb.position + _forceHeightOffset * Vector3.up;
            _rb.AddForceAtPosition(Vector3.up * Vector3.Dot(Vector3.up, -_velocityRelativeToWater) * _dragInWaterUp, forcePosition, ForceMode.Acceleration);
            _rb.AddForceAtPosition(transform.right * Vector3.Dot(transform.right, -_velocityRelativeToWater) * _dragInWaterRight, forcePosition, ForceMode.Acceleration);
            _rb.AddForceAtPosition(transform.forward * Vector3.Dot(transform.forward, -_velocityRelativeToWater) * _dragInWaterForward, forcePosition, ForceMode.Acceleration);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.TransformPoint(_centerOfMass), Vector3.one * 0.25f);

            for (int i = 0; i < _forcePoints.Length; i++)
            {
                var point = _forcePoints[i];

                var transformedPoint = transform.TransformPoint(point._offsetPosition + new Vector3(0, _centerOfMass.y, 0));

                Gizmos.color = Color.red;
                Gizmos.DrawCube(transformedPoint, Vector3.one * 0.5f);
            }
        }

        public void sailAngleCalcs()
        {
            sailAngle = sail.transform.eulerAngles.y;

            hullAngle = hull.transform.eulerAngles.y;
            sailHullDif = mast.transform.localRotation.z;

            if (sailAngle > windDirection)
            {
                sailWindAngle = sailAngle - windDirection;
            }
            else if (sailAngle < windDirection)
            {
                sailWindAngle = sailAngle - windDirection + 360;
            }

            if (hullAngle > windDirection)
            {
                hullWindAngle = hullAngle - windDirection;
            }
            else if (hullAngle < windDirection)
            {
                hullWindAngle = hullAngle - windDirection + 360;
            }

            //Get and correct sail angle (I just want to measure things in 180 or -180 degree hemispheres... also because you're a dumbass who likes to overcomplicate things

            if (hullWindAngle > 0)
            {
                hullWindAngle = hullWindAngle - 180;
            }
            if (sailWindAngle > 0)
            {
                sailWindAngle = sailWindAngle - 180;
            }

            //Debug.Log("Hull Angle:" + hullAngle + " Sail angle to wind is " + (sailWindAngle) + " and hull wind angle is " + (hullWindAngle) + " wind direction is " + windDirection);


            //find hull angle to the wind, correct the boom and move the sail appropriately
            if ((hullWindAngle < 0))
            {
                if (sailHullDif < 0.05)
                {
                    //Debug.Log("Sail is on wrong side! It should be on port");
                    mast.transform.Rotate(0.0f, 0.0f, 0.8f, Space.Self);
                }
                Vector3 wndy = new Vector3(0.0f, 0.0f, clothWindStrength);
                sail.GetComponent<Cloth>().externalAcceleration = wndy;
            }
            else if ((hullWindAngle > 0))
            {
                if (sailHullDif > -0.05)
                {
                    //Debug.Log("Sail is on wrong side! It should be on port");
                    mast.transform.Rotate(0.0f, 0.0f, -0.8f, Space.Self);
                }
                Vector3 wndy = new Vector3(0.0f, 0.0f, -clothWindStrength);
                sail.GetComponent<Cloth>().externalAcceleration = wndy;
            }

            if (((hullWindAngle > -45) && (hullWindAngle < 0)) || ((hullWindAngle < 45) && (hullWindAngle > 0)))
            {
                //Debug.Log(hullWindAngle);
                Debug.Log("DEADZONE");
                sailPower = 0.1f;
            }
            else if (sailWindAngle >= 0)
            {
                if ((sailWindAngle > 0) && (sailWindAngle < 90))
                {
                    sailPower = sailWindAngle / 90;
                    //Debug.Log(sailPower);
                }
                else if ((sailWindAngle > 90) && (sailWindAngle < 180))
                {
                    sailPower = (180 - sailWindAngle) / 90;
                    //Debug.Log(sailPower);
                }
            }
            else if (sailWindAngle <= 0)
            {
                if ((sailWindAngle < 0) && (sailWindAngle > -90))
                {
                    sailPower = sailWindAngle / -90;
                    //Debug.Log(sailPower);
                }
                else if ((sailWindAngle < -90) && (sailWindAngle > -180))
                {
                    sailPower = (180 + sailWindAngle) / 90;
                    //Debug.Log(sailPower);
                }
            }
        }

        public void boatControls()
        {
            //control boom wihth Q and E
            if (Input.GetKey(KeyCode.E) && (mast.transform.localRotation.z > -0.65))
            {
                mast.transform.Rotate(0.0f, 0.0f, -0.1f, Space.Self);
            }
            if (Input.GetKey(KeyCode.Q) && (mast.transform.localRotation.z < 0.65))
            {
                mast.transform.Rotate(0.0f, 0.0f, 0.1f, Space.Self);
            }
        }

        public void hullHeel()
        {

            if (hullWindAngle > 0)
            {
                roll = 45 / hullWindAngle;
            }
            else
            {
                roll = -45 / hullWindAngle;
            }

            //Debug.Log("Roll: " + roll);
            Rigidbody hrb = hull.GetComponent<Rigidbody>();
  
            if ((hullWindAngle < -90) & (hullWindAngle > -180))
            {
                float heelValue = ((heel * roll) * -sailPower) / keelWeight;
                hrb.AddRelativeTorque(heelValue, 0.0f, 0.0f);
                //Debug.Log("Roll Port down wind");
            }

            if ((hullWindAngle > -90) & (hullWindAngle < -45))
            {
                float heelValue = ((heel * roll) * -sailPower) / keelWeight;
                hrb.AddRelativeTorque(heelValue, 0.0f, 0.0f);
                //Debug.Log("Roll Port up wind");
            }


            if ((hullWindAngle < 90) & (hullWindAngle > 45))
            {
                float heelValue = ((heel * roll) * sailPower) / keelWeight;
                hrb.AddRelativeTorque(heelValue, 0.0f, 0.0f);
                //Debug.Log("Roll Starboard up wind");
            }

            if ((hullWindAngle > 90) & (hullWindAngle < 180))
            {
                float heelValue = ((heel * roll) * sailPower) / keelWeight;
                hrb.AddRelativeTorque(heelValue, 0.0f, 0.0f);
                //Debug.Log("Roll Starboard up wind");
            }

        }

        public void showUI()
        {
            float sailPowerRounded = Mathf.Round(sailPower * 100);
            sailPowerPerc.text = sailPowerRounded.ToString();

            Vector3 boatSpeed = hull.GetComponent<Rigidbody>().velocity;
            float boatSpeedKnts = Mathf.Round((boatSpeed.magnitude * 1.96f));
            boatSpeedText.text = boatSpeedKnts.ToString();
            boatSpeedZoomed.text = boatSpeedKnts.ToString();

            float hullAngleRounded = Mathf.Round(hullAngle) + 90f;
            boatHeading.text = hullAngleRounded.ToString();
            boatHeadingZoomed.text = hullAngleRounded.ToString();

            if (windDirection > 22.5 && windDirection < 67.5)
            {
                windDirectionText.text = "NW";
            }
            else if (windDirection > 67.5 && windDirection < 112.5)
            {
                windDirectionText.text = "W";
            }
            else if (windDirection > 112.5 && windDirection < 157.5)
            {
                windDirectionText.text = "SW";
            }
            else if (windDirection > 157.5 && windDirection < 202.5)
            {
                windDirectionText.text = "S";
            }
            else if (windDirection > 202.5 && windDirection < 247.5)
            {
                windDirectionText.text = "SE";
            }
            else if (windDirection > 247.5 && windDirection < 292.5)
            {
                windDirectionText.text = "E";
            }
            else if (windDirection > 292.5 && windDirection < 337.5)
            {
                windDirectionText.text = "NE";
            }
            else
            {
                windDirectionText.text = "N";
            }

            windSpeedText.text = windSpeed.ToString();
        }

        public void windSpeedRandomiser()
        {
            float xx = UnityEngine.Random.Range(windSpeedLower, windSpeedUpper);
            windSpeed = Mathf.Round(xx);
        }

        public void effectsController()
        {
            Vector3 boatSpeed = hull.GetComponent<Rigidbody>().velocity;
            float boatSpeedKnts = Mathf.Round((boatSpeed.magnitude * 1.96f));

            if (boatSpeedKnts >= 10)
            {
                waterSplashEffects.SetActive(true);
            }
            else if(boatSpeedKnts < 10)
            {
                waterSplashEffects.SetActive(false);
            }
        }

    }

    [Serializable]
    public class FloaterForcePoints
    {
        [FormerlySerializedAs("_factor")]
        public float _weight = 1f;

        public Vector3 _offsetPosition;
    }

}
