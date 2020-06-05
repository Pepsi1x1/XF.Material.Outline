using System;

namespace XF.Material.Outline.Core
{
	public class Easing
	{
		/// <summary>
		///     Constant Pi / 2.
		/// </summary>
		private const float HALFPI = (float) Math.PI / 2.0f;

		/// <summary>
		///     Modeled after the piecewise exponentially-damped sine wave:
		///     y = (1/2)*sin(13pi/2*(2*time))*Math.Pow(2, 10 * ((2*time) - 1))      ; [0,0.5)
		///     y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*time-1)) + 2) ; [0.5, 1]
		/// </summary>
		public static float ElasticEaseInOut(float time)
		{
			if (time < 0.5f)
			{
				return 0.5f * (float) Math.Sin(13 * HALFPI * (2 * time)) * (float) Math.Pow(2, 10 * (2 * time - 1));
			}

			return 0.5f * (float) (Math.Sin(-13 * HALFPI * (2 * time - 1 + 1)) * Math.Pow(2, -10 * (2 * time - 1)) + 2);
		}

		public static float Smoothstep(float time)
		{
			return time * time * (3 - 2 * time);
		}
	}
}