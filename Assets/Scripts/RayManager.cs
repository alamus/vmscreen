using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour {
    #region public variables

    public float sightlength = 100.0f;
    public GameObject eyeCamera;
    public float ChangeRate;

    #endregion

    #region private variables
    private GameObject selectedObj;
    private GameObject clickedObj;
    private SteamVR_TrackedController _controller;
    private float distance;
    private RaycastHit raycasthit;
    private Ray raydirection;
    private Dictionary<string, TempTransform> originalStats;
    #endregion

    #region events
    private void Start()
    {
        originalStats = new Dictionary<string, TempTransform>();
        GameObject[] allpanels = GameObject.FindGameObjectsWithTag("Panel");
        foreach(GameObject panel in allpanels)
        {
            originalStats.Add(panel.name, panel.transform.Clone());
        }
        if (ChangeRate == 0)
            ChangeRate = 0.2f;
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerUnclicked;
        _controller.PadClicked += HandlePadClicked;
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= HandleTriggerClicked;
        _controller.TriggerUnclicked -= HandleTriggerUnclicked;
        _controller.PadClicked -= HandlePadClicked;
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        raydirection = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(raydirection, out raycasthit, sightlength))
        {
            selectedObj = raycasthit.collider.gameObject;
            clickedObj = raycasthit.collider.gameObject;
            distance = Vector3.Distance(eyeCamera.transform.position, selectedObj.transform.position);
        }
    }
    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        selectedObj = null;
    }

    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        if (clickedObj != null)
        {
            if (e.padY > 0.7f)
            {
                scaleCube(clickedObj, ChangeRate, true);
            }

            else if (e.padY < -0.7f)
            {
                scaleCube(clickedObj, ChangeRate, false);
            }

            if (e.padY < 0.7f && e.padY > -0.7f)
            {
                clickedObj.transform.rotation = originalStats[clickedObj.name].rotation;
                clickedObj.transform.localScale = originalStats[clickedObj.name].localScale;
                clickedObj.transform.position = originalStats[clickedObj.name].position;
                clickedObj.transform.localPosition = originalStats[clickedObj.name].localPosition;
                clickedObj.transform.localRotation = originalStats[clickedObj.name].localRotation;
            }
        }
    }
    #endregion  

    void Update()
    {
        if (selectedObj != null)
        {
            raydirection = new Ray(transform.position, transform.forward);
            Vector3 pointPosition = raydirection.GetPoint(distance);
            selectedObj.transform.LookAt(eyeCamera.transform.position);
            selectedObj.transform.position = pointPosition;
        }
    }

    #region helper
    private void scaleCube(GameObject go, float changeRate, bool changeLarger)
    {
        Vector3 size = go.transform.localScale;
        float originalX = System.Math.Abs(size[0]);
        float originalY = System.Math.Abs(size[1]);

        float x = originalX;
        float y = originalY;
        if (changeLarger)
        {
            x = System.Math.Abs(size[0]) + changeRate;
            y = System.Math.Abs(size[1]) + changeRate;
        }
        else
        {
            x = System.Math.Abs(size[0]) - changeRate;
            y = System.Math.Abs(size[1]) - changeRate;
        }
        if (x <= 0 || y <= 0)
        {
            x = originalX;
            y = originalY;
        }

        clickedObj.transform.localScale = new Vector3(x, y, size[2]);
    }
    #endregion
}

public struct TempTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localPosition;
    public Vector3 localScale;
    public Quaternion localRotation;
}

public static class TransformUtils
{

    public static TempTransform Clone(this Transform transform)
    {
        TempTransform td = new TempTransform();

        td.position = transform.position;
        td.localPosition = transform.localPosition;

        td.rotation = transform.rotation;
        td.localRotation = transform.localRotation;

        td.localScale = transform.localScale;

        return td;
    }

}