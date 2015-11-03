using UnityEngine;
using System.Collections;

public class GaussianRandom
{
	public static float GetGaussianFloat()
	{
		double U, u, v, S;
		
		do
		{
			u = 2.0 * Random.value - 1.0;
			v = 2.0 * Random.value - 1.0;
			S = u * u + v * v;
		}
		while (S >= 1.0);
		
		double fac = Mathf.Sqrt((float)(-2.0 * Mathf.Log((float)S) / S));
		return (float)(u * fac);
	}
	
	public static float GetGaussianCustomRange(float mean, float stdDeviation)
	{
		/*mean is the median (center value), stdDeviation is the deviation distance 
		(values will be ~mean+-stdDeviation or ~mean+-stdDeviation*1 (34%); mean+-stdDeviation*2 (13%); mean+-stdDeviation*3 (2.1%)
		; mean+-stdDeviation*4+ (0.1%)
		*/
		float raw=GetGaussianFloat();
		return raw*stdDeviation+mean;
	}
	
	public static float GetFiveStepRange(float min, float max)
	{
		float range=max-(min-1);
		
		//float max=min+4;
		float mean=(min+max)/2f;//max-2;
		float stdDeviation=range/5;//1;
		float raw=GetGaussianCustomRange(mean,stdDeviation);
		return Mathf.Clamp(raw,min,max);
	}
}
