using UnityEngine;
using System.Collections;

public class BresenhamLines 
{

	
		/// <summary>
		/// The Bresenham algorithm collection
		/// </summary>
		//public static class Algorithms
		//{
			private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }
			
			/// <summary>
			/// The plot function delegate
			/// </summary>
			/// <param name="x">The x co-ord being plotted</param>
			/// <param name="y">The y co-ord being plotted</param>
			/// <returns>True to continue, false to stop the algorithm</returns>
			public delegate bool PlotFunction(int x, int y);
			
			/// <summary>
			/// Plot the line from (x0, y0) to (x1, y10
			/// </summary>
			/// <param name="x0">The start x</param>
			/// <param name="y0">The start y</param>
			/// <param name="x1">The end x</param>
			/// <param name="y1">The end y</param>
			/// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
			public static void Line(int x1, int y1, int x2, int y2, PlotFunction plot)
			{
				/*
				bool steep = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
				if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
				if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
				int dX = (x1 - x0), dY = Mathf.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;
				
				for (int x = x0; x <= x1; ++x)
				{
					if (!(steep ? plot(y, x) : plot(x, y))) return;
					err = err - dY;
					if (err < 0) { y += ystep;  err += dX; }
				}*/
		int deltax,deltay,x,y,xinc1,xinc2,yinc1,yinc2,numadd,numpixels,curpixel;
		float den,num;
		deltax = Mathf.Abs(x2 - x1);		// The difference between the x's
		deltay = Mathf.Abs(y2 - y1);		// The difference between the y's
		x = x1;				   	// Start x off at the first pixel
		y = y1;				   	// Start y off at the first pixel
		
		if (x2 >= x1)			 	// The x-values are increasing
		{
			xinc1 = 1;
			xinc2 = 1;
		}
		else						  // The x-values are decreasing
		{
			xinc1 = -1;
			xinc2 = -1;
		}
		
		if (y2 >= y1)			 	// The y-values are increasing
		{
			yinc1 = 1;
			yinc2 = 1;
		}
		else						  // The y-values are decreasing
		{
			yinc1 = -1;
			yinc2 = -1;
		}
		
		if (deltax >= deltay)	 	// There is at least one x-value for every y-value
		{
			xinc1 = 0;				  // Don't change the x when numerator >= denominator
			yinc2 = 0;				  // Don't change the y for every iteration
			den = deltax;
			num = deltax / 2;
			numadd = deltay;
			numpixels = deltax;	 	// There are more x-values than y-values
		}
		else						  // There is at least one y-value for every x-value
		{
			xinc2 = 0;				  // Don't change the x for every iteration
			yinc1 = 0;				  // Don't change the y when numerator >= denominator
			den = deltay;
			num = deltay / 2;
			numadd = deltax;
			numpixels = deltay;	 	// There are more y-values than x-values
		}
		
		for (curpixel = 0; curpixel <= numpixels; curpixel++)
		{
			if (!plot(x, y)) {return;}		 	// Draw the current pixel
			num += numadd;			  // Increase the numerator by the top of the fraction
			if (num >= den)		 	// Check if numerator >= denominator
			{
				num -= den;		   	// Calculate the new numerator value
				x += xinc1;		   	// Change the x as appropriate
				y += yinc1;		   	// Change the y as appropriate
			}
			x += xinc2;			 	// Change the x as appropriate
			y += yinc2;			 	// Change the y as appropriate
		}
			}
		//}
	
}
