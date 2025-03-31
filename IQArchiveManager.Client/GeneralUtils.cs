using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQArchiveManager.Client
{
    public static class GeneralUtils
    {
        public static float ConvertAudioSample(byte sample)
        {
            return (sample - 127.5f) / 127.5f;
        }

        /// <summary>
        /// Finds the nearest time to the reference time and returns it.
        /// </summary>
        /// <param name="times"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static DateTime FindNearest(this IEnumerable<DateTime> times, DateTime reference)
        {
            TimeSpan nearestDistance = TimeSpan.MaxValue;
            DateTime nearest = DateTime.MinValue;
            TimeSpan distance;
            foreach (var t in times)
            {
                //Calculate distance
                distance = (reference - t).Abs();

                //Check if it is shorter
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = t;
                }
            }
            return nearest;
        }

        /// <summary>
        /// Returns the absolute value of this timestamp.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan Abs(this TimeSpan time)
        {
            return new TimeSpan(Math.Abs(time.Ticks));
        }

        /// <summary>
        /// Takes the value and adds the name followed by an s if value != 1
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToStringPlural(this int value, string name)
        {
            return value + " " + name + (value == 1 ? "" : "s");
        }
    }
}
