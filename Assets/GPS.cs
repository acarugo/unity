using UnityEngine;
using System.Collections;

public class GPS : MonoBehaviour {

    private static long RADIUS = 6371;
    private TextMesh lblInfo;
    private GameObject _camera;
    private GameObject bola;
    private GameObject dirLight;
    private const string JUMP = "\r\n";

    // Use this for initialization
    void Start () {
        _camera = GameObject.Find("ARCamera");
        bola = GameObject.Find("bola");
        dirLight = GameObject.Find("dirLight");
        lblInfo = GameObject.Find("lblInfo").GetComponent<TextMesh>();
        addLog("Inicio...");

        Input.location.Start();
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
            addLog("Timed out");

        if (Input.location.status == LocationServiceStatus.Failed)
            addLog("Unable to determine device location");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            //moveElements(latitude, longitude);
            addLog();
        }
    }

    private void moveElements(float latitude, float longitude)
    {
        Vector3 pos = SphericalToCartesian(latitude, longitude);

        Vector3 v1 = new Vector3(pos.x, pos.y, pos.z);
        this._camera.transform.position = v1;

        float x = (float)(pos.x - 8.09);
        float y = (float)(pos.y + 2.92);
        float z = (float)(pos.y + 11.17);
        Vector3 v2 = new Vector3(x, y, z);
        this.lblInfo.transform.position = v2;

        x = (float)(pos.x - 2.46);
        y = (float)(pos.y + 2.92);
        z = (float)(pos.y + 13.01);
        Vector3 v3 = new Vector3(x, y, z);
        this.bola.transform.position = v3;

        Vector3 v4 = new Vector3(pos.x, pos.y + 3, pos.z);
        this.dirLight.transform.position = v4;
    }

    private void addLog(string msg)
    {
        this.lblInfo.text = msg;
    }

    private void addLog()
    {
        string info = "";
        info += "Latitude: " + Input.location.lastData.latitude + JUMP;
        info += "Longitude: " + Input.location.lastData.longitude + JUMP;
        info += "Altitude: " + Input.location.lastData.altitude + JUMP;
        info += "Horizontal: " + Input.location.lastData.horizontalAccuracy + JUMP;
        info += "Vertical: " + Input.location.lastData.verticalAccuracy + JUMP;

        //info += "X: " + this._camera.transform.position.x + JUMP;
        //info += "Y: " + this._camera.transform.position.y + JUMP;
        //info += "Z: " + this._camera.transform.position.z + JUMP;

        info += "Accelerometer: " + Input.acceleration.ToString("F4") + JUMP;

        if (!Input.gyro.enabled)
            Input.gyro.enabled = true;
        if (Input.gyro.enabled)
        {
            info += "Attitude: " + Input.gyro.attitude.ToString("F4") + JUMP; //creo que este dato es el que importa...
            info += "Gravity: " + Input.gyro.gravity.ToString("F4") + JUMP;
            info += "Rotation: " + Input.gyro.rotationRate.ToString("F4");
        }
        else
            info += "Attitude: NO";

        this.lblInfo.text = info;
    }

    public static float[] CartesianToSpherical(float x, float y, float z)
    {
        float[] retVal = new float[2];

        if (x == 0)
            x = Mathf.Epsilon;

        retVal[0] = Mathf.Atan(z / x);

        if (x < 0)
            retVal[0] += Mathf.PI;

        retVal[1] = Mathf.Asin(y / RADIUS);

        return retVal;
    }

    public static float[] CartesianToSpherical(Vector3 v)
    {
        return CartesianToSpherical(v.x, v.y, v.z);
    }

    public static Vector3 SphericalToCartesian(float latitude, float longitude)
    {
        float a = RADIUS * Mathf.Cos(longitude);
        float x = a * Mathf.Cos(latitude);
        float y = RADIUS * Mathf.Sin(longitude);
        float z = a * Mathf.Sin(latitude);

        return new Vector3(x, y, z);
    }

}
