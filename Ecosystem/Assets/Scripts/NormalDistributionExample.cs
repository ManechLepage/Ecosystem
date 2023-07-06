using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDistributionExample : MonoBehaviour
{

    public int numberOfPoints = 100;
    public float threshold = 1.0f;

    public float mean = 0.0f;
    public float standardDeviation = 1.0f;
    void Start()
    {
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 3;
        BellCurve bellCurve = new BellCurve(mean, standardDeviation);
        Vector3[] points = new Vector3[numberOfPoints];
        double[] curvePoints = bellCurve.GenerateCurvePoints(threshold, numberOfPoints);
        for (int i = 0; i < numberOfPoints; i++)
        {
            points[i] = new Vector3(i, (float)curvePoints[i], 0);
        }

        lineRenderer.SetPositions(points);
    }
}
