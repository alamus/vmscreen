using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testControllers : MonoBehaviour {
    public GameObject APanel;
    public float ChangeRate;
    private SteamVR_TrackedController _controller;
    private Vector3 originalSize;
    private void Start()
    {
        originalSize = APanel.transform.localScale;
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.PadClicked += HandlePadClicked;
    }

    private void OnDisable()
    {
        _controller.PadClicked -= HandlePadClicked;
    }

    private void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padY > 0.7f)
        {
            scaleCube(APanel, ChangeRate, true);
            Debug.Log("Moving Up");
        }

        else if (e.padY < -0.7f)
        {
            scaleCube(APanel, ChangeRate, false);
            Debug.Log("Moving Down");
        }

        if (e.padY < 0.7f && e.padY > -0.7f)
        {
            scaleCube(APanel, ChangeRate, null);
            Debug.Log("Moving default");
        }
    }
    #region helper
    private void scaleCube(GameObject go, float changeRate, bool? changeLarger)
    {
        Vector3 size = go.transform.localScale;
        float x = System.Math.Abs(originalSize[0]);
        float y = System.Math.Abs(originalSize[1]);
        if (changeLarger != null)
        {
            if (changeLarger.GetValueOrDefault())
            {
                x = System.Math.Abs(size[0]) + changeRate;
                y = System.Math.Abs(size[1]) + changeRate;
            }
            else
            {
                x = System.Math.Abs(size[0]) - changeRate;
                y = System.Math.Abs(size[1]) - changeRate;
            }
        }
        if (x == 0 || y == 0)
        {
            x = System.Math.Abs(originalSize[0]);
            y = System.Math.Abs(originalSize[1]);
        }

        APanel.transform.localScale = new Vector3(x, y, originalSize[2]);
    }

    #endregion
}
