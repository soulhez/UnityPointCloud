using UnityEngine;
using System.Collections;
using MathNet.Numerics.LinearAlgebra;

public class PointCloud : MonoBehaviour
{
  private Camera cam;
  private ParticleSystem.Particle[] cloud;
  // Enable this in realtime if you want to re-render the points and camera background.
  public bool updatePoints = false;
  public bool controlsEnabled = true;
  public bool autoRotateEnabled = false;
  // The 2-D array of all points to plot.
  public float[,] p;
  // The 1-D array of the color of all points to plot.
  public Color[] c;
  // The maximum x-value, y-value, and z-value among the points. This is used for coloring.
  public float max = 1.0f;
  public Color backgroundRGBA = Color.white/4;

  void Start ()
  {
    // Set the control camera to be the Main Camera.
    cam = Camera.mainCamera.camera;

    // Get new CloudPoint data.
    CloudPoint[] cloudP = new CloudPoint[1];
    cloudP[0] = new CloudPoint(new Vector(new double[] {0.5,0.5,0.5}), Color.red, new Vector(new double[] {0.5,0.5,0.5}));
    // Convert the CloudPoint data and fill the p locations matrix and c colors matrix.
    getCloudPoints(cloudP);
    
    // Initial point cloud rendering.
    updatePoints = true;
  }

  void Update () 
  {
    if (updatePoints)
    {
      // RETRIEVE UPDATED POINTS HERE IF YOU WANT TO
      // Create new particles and set at new positions.
      SetPoints(p,c);
      // Redraw the points.
      particleSystem.SetParticles(cloud, cloud.Length);
      // Reset Camera distance according to coordinate range;
      cam.transform.position = new Vector3(0, max/2, 0);
      cam.transform.LookAt(Vector3.one * max/2);
      cam.transform.position -= cam.transform.forward*5/4*max;
      // Set the background of the control camera's display.
      cam.backgroundColor = backgroundRGBA;
      // Don't redraw the points until SetPoints() is called again.
      updatePoints = false;
    }

    if (controlsEnabled)
    {
      // Horizontal camera rotation controls:
      if (Input.GetKey("up"))
        cam.transform.RotateAround(new Vector3(max/2, max/2, max/2), Vector3.right, Mathf.Pow(max/2,1/3)*150*Time.deltaTime);
      else if (Input.GetKey("down"))
        cam.transform.RotateAround(new Vector3(max/2, max/2, max/2), -1*Vector3.right, Mathf.Pow(max/2,1/3)*150*Time.deltaTime);

      // Vertical camera rotation controls:
      if (Input.GetKey("right"))
        cam.transform.RotateAround(new Vector3(max/2, max/2, max/2), -1*Vector3.up, Mathf.Pow(max/2,1/3)*150*Time.deltaTime);
      else if (Input.GetKey("left"))
        cam.transform.RotateAround(new Vector3(max/2, max/2, max/2), Vector3.up, Mathf.Pow(max/2,1/3)*150*Time.deltaTime);
    }

    if (autoRotateEnabled)
    {
      // Rotate up and to the right around the center of the point cloud.
      cam.transform.RotateAround(new Vector3(max/2, max/2, max/2), Vector3.right + Vector3.up, Mathf.Pow(max/2,1/3)*20*Time.deltaTime);
    }
  }

  // Set particle positions according to point coordinates using one argument. Color points according to position.
  public void SetPoints(float[,] points)
  {
    Color[] rgba = new Color[points.GetLength(0)];
    
    // Find the max value of any coordinate and use it for normalizing the automatic coloring of the points.
    max = Max(p);
    for (int i = 0; i < rgba.Length; i++)
    {
      // Color points according to xyz location.
      rgba[i] = new Color(points[i,0], points[i,1], points[i,2]);
      // Normalize with max coordinate value.
      rgba[i] /= max;
      // Retain complete opacity.
      rgba[i].a = 255;
    }
    SetPoints(points,rgba);
  } 

  // Set particle positions according to point coordinates using two arguments. Color points according to inputted Color[] array.
  public void SetPoints(float[,] points, Color[] rgba)
  {
    cloud = new ParticleSystem.Particle[points.GetLength(0)];

    for (int i = 0; i < points.GetLength(0); ++i)
    {
      // Set position of particles to match those of the 3-D points.
      cloud[i].position = new Vector3(points[i,0], points[i,1], points[i,2]);       
      // Color points according to Color[] array.
      cloud[i].color = rgba[i];
      // Static size.
      cloud[i].size = max/20;
    }
    
    // Every time the points are set, redraw them.
    updatePoints = true;
  }

  private float Max (float[,] points) {
    float max = points[0,0];

    // Search through points[] array and return the max recorded value within the array.
    for (int i = 1; i < points.GetLength(0); i++)
      for (int j = 0; j < points.GetLength(1); j++) 
        if (points[i,j] > max)
          max = points[i,j];

    return max;
  }

  private void getCloudPoints(CloudPoint[] cloudPoints) {
    p = new float[cloudPoints.Length,3];
    c = new Color[cloudPoints.Length];
    // Convert CloudPoint array to 2-D location array and 1-D color array.
    for (int i = 0; i < cloudPoints.Length; i++) {
      p[i,0] = (float) cloudPoints[i].location[0];
      p[i,1] = (float) cloudPoints[i].location[1];
      p[i,2] = (float) cloudPoints[i].location[2];
      c[i] = cloudPoints[i].color;
    }
  }
}