/*     INFINITY CODE 2013-2017      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Helper class, which contains all the basic methods.
/// </summary>
public static class OnlineMapsUtils
{
    /// <summary>
    /// Intercepts requests to download and allows you to create a custom query behavior.
    /// </summary>

    /// <summary>
    /// Arcseconds in meters.
    /// </summary>
    public const float angleSecond = 1 / 3600f;

    /// <summary>
    /// Earth radius.
    /// </summary>
    public const double R = 6371;

    /// <summary>
    /// Degrees-to-radians conversion constant.
    /// </summary>
    public const double Deg2Rad = Math.PI / 180;

    /// <summary>
    /// Radians-to-degrees conversion constant.
    /// </summary>
    public const double Rad2Deg = 180 / Math.PI;

    /// <summary>
    /// Bytes per megabyte.
    /// </summary>
    public const int mb = 1024 * 1024;

    /// <summary>
    /// PI * 4
    /// </summary>
    public const float pi4 = 4 * Mathf.PI;


    /// <summary>
    /// The second in ticks.
    /// </summary>
    public const long second = 10000000;


    /// <summary>
    /// The angle between the two points in degree.
    /// </summary>
    /// <param name="point1">Point 1</param>
    /// <param name="point2">Point 2</param>
    /// <returns>Angle in degree</returns>
    public static float Angle2D(Vector2 point1, Vector2 point2)
    {
        return Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// The angle between the two points in degree.
    /// </summary>
    /// <param name="point1">Point 1</param>
    /// <param name="point2">Point 2</param>
    /// <returns>Angle in degree</returns>
    public static float Angle2D(Vector3 point1, Vector3 point2)
    {
        return Mathf.Atan2(point2.z - point1.z, point2.x - point1.x) * Mathf.Rad2Deg;
    }


    /// <summary>
    /// The angle between the two points in degree.
    /// </summary>
    /// <param name="p1x">Point 1 X</param>
    /// <param name="p1y">Point 1 Y</param>
    /// <param name="p2x">Point 2 X</param>
    /// <param name="p2y">Point 2 Y</param>
    /// <returns>Angle in degree</returns>
    public static double Angle2D(double p1x, double p1y, double p2x, double p2y)
    {
        return Math.Atan2(p2y - p1y, p2x - p1x) * Rad2Deg;
    }

    /// <summary>
    /// The angle between the three points in degree.
    /// </summary>
    /// <param name="point1">Point 1</param>
    /// <param name="point2">Point 2</param>
    /// <param name="point3">Point 3</param>
    /// <param name="unsigned">Return a positive result.</param>
    /// <returns>Angle in degree</returns>
    public static float Angle2D(Vector3 point1, Vector3 point2, Vector3 point3, bool unsigned = true)
    {
        float angle1 = Angle2D(point1, point2);
        float angle2 = Angle2D(point2, point3);
        float angle = angle1 - angle2;
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360;
        if (unsigned) angle = Mathf.Abs(angle);
        return angle;
    }

    /// <summary>
    /// The angle between the two points in radians.
    /// </summary>
    /// <param name="point1">Point 1</param>
    /// <param name="point2">Point 2</param>
    /// <param name="offset">Result offset in degrees.</param>
    /// <returns>Angle in radians</returns>
    public static float Angle2DRad(Vector3 point1, Vector3 point2, float offset = 0)
    {
        return Mathf.Atan2(point2.z - point1.z, point2.x - point1.x) + offset * Mathf.Deg2Rad;
    }


    /// <summary>
    /// The angle between the two points in radians.
    /// </summary>
    /// <param name="p1x">Point 1 X</param>
    /// <param name="p1z">Point 1 Z</param>
    /// <param name="p2x">Point 2 X</param>
    /// <param name="p2z">Point 2 Z</param>
    /// <param name="offset">Result offset in degrees.</param>
    /// <returns>Angle in radians</returns>
	public static double Angle2DRad(double p1x, double p1z, double p2x, double p2z, float offset = 0)
    {
		return Math.Atan2((p2z - p1z), (p2x - p1x)) + offset * Mathf.Deg2Rad;
    }

    public static float AngleOfTriangle(Vector2 A, Vector2 B, Vector2 C)
    {
        float a = (B - C).magnitude;
        float b = (A - C).magnitude;
        float c = (A - B).magnitude;

        return Mathf.Acos((a * a + b * b - c * c) / (2 * a * b));
    }

	/// <summary>
	/// The distance between two geographical coordinates.
	/// </summary>
	/// <param name="point1">Coordinate (X - Lng, Y - Lat)</param>
	/// <param name="point2">Coordinate (X - Lng, Y - Lat)</param>
	/// <returns>Distance (km).</returns>
	public static double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
	{
		double scfY = Math.Sin(point1.y * Deg2Rad);
		double sctY = Math.Sin(point2.y * Deg2Rad);
		double ccfY = Math.Cos(point1.y * Deg2Rad);
		double cctY = Math.Cos(point2.y * Deg2Rad);
		double cX = Math.Cos((point1.x - point2.x) * Deg2Rad);
		double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
		double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
		double sizeX = (sizeX1 + sizeX2) / 2.0;
		double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
		if (double.IsNaN(sizeY)) sizeY = 0;
		return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
	}

	/// <summary>
	/// The distance between two geographical coordinates.
	/// </summary>
	/// <param name="point1">Coordinate (X - Lng, Y - Lat)</param>
	/// <param name="point2">Coordinate (X - Lng, Y - Lat)</param>
	/// <returns>Distance (km).</returns>
	public static double DistanceBetweenPointsD(double point1x, double point1y, double point2x, double point2y)
	{
/*		double scfY = Math.Sin(point1y * Deg2Rad);
		double sctY = Math.Sin(point2y * Deg2Rad);
		double ccfY = Math.Cos(point1y * Deg2Rad);
		double cctY = Math.Cos(point2y * Deg2Rad);
		double cX = Math.Cos((point1x - point2x) * Deg2Rad);
		double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
		double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
		double sizeX = (sizeX1 + sizeX2) / 2.0;
		double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
		if (double.IsNaN(sizeY)) sizeY = 0;
		return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);*/

		double d = Math.Acos(Math.Cos(Deg2Rad*(90-point1x))*Math.Cos(Deg2Rad*(90-point2x))+Math.Sin(Deg2Rad*(90-point1x))*Math.Sin(Deg2Rad*(90-point2x))*Math.Cos(Deg2Rad*(point1y-point2y)))*6371;
		return d;
	}

	public static double bearingP1toP2(double P1_latitude, double P1_longitude, double P2_latitude, double P2_longitude)

	{

		 // 현재 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에

		 //라디안 각도로 변환한다.

		double Cur_Lat_radian = P1_latitude * Deg2Rad;

		double Cur_Lon_radian = P1_longitude * Deg2Rad;

		 // 목표 위치 : 위도나 경도는 지구 중심을 기반으로 하는 각도이기 때문에

		 // 라디안 각도로 변환한다.

		double Dest_Lat_radian = P2_latitude * Deg2Rad;

		double Dest_Lon_radian = P2_longitude * Deg2Rad;

		 // radian distance

		double radian_distance = 0;

		radian_distance = Math.Acos(Math.Sin(Cur_Lat_radian) * Math.Sin(Dest_Lat_radian) + Math.Cos(Cur_Lat_radian) * Math.Cos(Dest_Lat_radian) * Math.Cos(Cur_Lon_radian - Dest_Lon_radian));

		 // 목적지 이동 방향을 구한다.(현재 좌표에서 다음 좌표로 이동하기 위해서는

		 //방향을 설정해야 한다. 라디안값이다.

		double radian_bearing = Math.Acos((Math.Sin(Dest_Lat_radian) - Math.Sin(Cur_Lat_radian) * Math.Cos(radian_distance)) / (Math.Cos(Cur_Lat_radian) * Math.Sin(radian_distance)));

		 // acos의 인수로 주어지는 x는 360분법의 각도가 아닌 radian(호도)값이다.

		double true_bearing = 0;

		if (Math.Sin(Dest_Lon_radian - Cur_Lon_radian) < 0) {

			true_bearing = radian_bearing * (180 / 3.141592);

			true_bearing = 360 - true_bearing;

		} else {

			true_bearing = radian_bearing * (180 / 3.141592);

		}

		return  true_bearing;

	}
	
	public static double DistanceBetweenPointsD_GRS(double P1_latitude, double P1_longitude,double P2_latitude, double P2_longitude)
	{
		if ((P1_latitude == P2_latitude) && (P1_longitude == P2_longitude))
		{
			return 0;
		}
		// convert from degree to radian

		double e10 = P1_latitude * Deg2Rad;

		double e11 = P1_longitude * Deg2Rad;

		double e12 = P2_latitude * Deg2Rad;

		double e13 = P2_longitude * Deg2Rad;

		 // 타원체 GRS80

		double c16 = 6356752.314140910;

		double c15 = 6378137.000000000;

		double c17 = 0.0033528107;

		double f15 = c17 + c17 * c17;

		double f16 = f15 / 2;

		double f17 = c17 * c17 / 2;

		double f18 = c17 * c17 / 8;

		double f19 = c17 * c17 / 16;

		double c18 = e13 - e11;

		double c20 = (1 - c17) * Math.Tan(e10);

		double c21 = Math.Atan(c20);

		double c22 = Math.Sin(c21);

		double c23 = Math.Cos(c21);

		double c24 = (1 - c17) * Math.Tan(e12);

		double c25 = Math.Atan(c24);

		double c26 = Math.Sin(c25);

		double c27 = Math.Cos(c25);

		double c29 = c18;

		double c31 = (c27 * Math.Sin(c29) * c27 * Math.Sin(c29)) + (c23 * c26 - c22 * c27 * Math.Cos(c29)) * (c23 * c26 - c22 * c27 * Math.Cos(c29));

		double c33 = (c22 * c26) + (c23 * c27 * Math.Cos(c29));

		double c35 = Math.Sqrt(c31) / c33;

		double c36 = Math.Atan(c35);

		double c38 = 0;

		if (c31 == 0)

		{

			c38 = 0;

		} else {

			c38 = c23 * c27 * Math.Sin(c29) / Math.Sqrt(c31);

		}

		double c40 = 0;





		if ((Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38))) == 0)

		{

			c40 = 0;

		} else {

			c40 = c33 - 2 * c22 * c26 / (Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38)));

		}

		double c41 = Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38)) * (c15 * c15 - c16 * c16) / (c16 * c16);

		double c43 = 1 + c41 / 16384 * (4096 + c41 * (-768 + c41 * (320 - 175 * c41)));

		double c45 = c41 / 1024 * (256 + c41 * (-128 + c41 * (74 - 47 * c41)));

		double c47 = c45 * Math.Sqrt(c31) * (c40 + c45 / 4 * (c33 * (-1 + 2 * c40 * c40) - c45 / 6 * c40 * (-3 + 4 * c31) * (-3 + 4 * c40 * c40)));

		double c50 = c17 / 16 * Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38)) * (4 + c17 * (4 - 3 * Math.Cos(Math.Asin(c38)) * Math.Cos(Math.Asin(c38))));

		double c52 = c18 + (1 - c50) * c17 * c38 * (Math.Acos(c33) + c50 * Math.Sin(Math.Acos(c33)) * (c40 + c50 * c33 * (-1 + 2 * c40 * c40)));

		double c54 = c16 * c43 * (Math.Atan(c35) - c47);

		

		// return distance in meter

		return c54;

		}



}