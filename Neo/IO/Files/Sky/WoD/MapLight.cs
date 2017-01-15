﻿using System;
using System.Collections.Generic;
using OpenTK;

namespace Neo.IO.Files.Sky.WoD
{
    public class MapLight
    {
        private readonly LightEntryData mEntry;
        private LightParamsEntry mParams;

        private readonly List<LightDataEntry> mDataEntries = new List<LightDataEntry>();
        private LightDataEntry[] mEntryArray = new LightDataEntry[0];

        public bool IsZoneLight { get; private set; }
        public int SkyId { get { return this.mParams.Id; } }
        public Vector3 Position { get { return this.mEntry.Position; } }
        public float OuterRadius { get { return this.mEntry.OuterRadius; } }
        public float InnerRadius { get { return this.mEntry.InnerRadius; } }
        public bool IsGlobal { get { return (this.InnerRadius < 0.01f && this.OuterRadius < 0.01f); } }
        public int LightId { get { return this.mEntry.Id; } }

        public MapLight(LightEntryData entry, ref LightParamsEntry paramsEntry)
        {
	        this.IsZoneLight = false;
	        this.mEntry = entry;
	        this.mParams = paramsEntry;
        }

        public bool GetColorForTime(int time, LightColor colorType, ref Vector3 color)
        {
            if (this.mDataEntries.Count == 0)
            {
	            return false;
            }

	        if (this.mEntryArray.Length != this.mDataEntries.Count)
	        {
		        this.mEntryArray = this.mDataEntries.ToArray();
	        }

	        if (this.mEntryArray.Length == 1)
            {
                color = ToRgb(colorType, ref this.mEntryArray[0]);
                return true;
            }

            var maxTime = this.mEntryArray[this.mEntryArray.Length - 1].timeValues;
            if (maxTime == 0 || this.mEntryArray[0].timeValues > time)
            {
                color = ToRgb(colorType, ref this.mEntryArray[0]);
                return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for (var i = 0; i < this.mEntryArray.Length; ++i)
            {
                if (i + 1 >= this.mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = this.mEntryArray[eIndex1].timeValues;
                    t2 = this.mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (this.mEntryArray[i].timeValues > time || this.mEntryArray[i + 1].timeValues <= time)
                {
	                continue;
                }

	            eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = this.mEntryArray[eIndex1].timeValues;
                t2 = this.mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
            {
	            return false;
            }

	        if (t1 >= t2)
            {
                color = ToRgb(colorType, ref this.mEntryArray[eIndex1]);
                return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float)diff;
            var v1 = ToRgb(colorType, ref this.mEntryArray[eIndex1]);
            var v2 = ToRgb(colorType, ref this.mEntryArray[eIndex2]);
            color = v2 * sat + v1 * (1 - sat);

            return true;
        }

        public bool GetAllColorsForTime(int time, Vector3[] colors)
        {
            if (this.mDataEntries.Count == 0)
            {
	            return false;
            }

	        if (this.mEntryArray.Length != this.mDataEntries.Count)
	        {
		        this.mEntryArray = this.mDataEntries.ToArray();
	        }

	        if (this.mEntryArray.Length == 1)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                {
	                colors[i] = ToRgb((LightColor)i, ref this.mEntryArray[0]);
                }

	            return true;
            }

            var maxTime = this.mEntryArray[this.mEntryArray.Length - 1].timeValues;
            if (maxTime == 0 || this.mEntryArray[0].timeValues > time)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                {
	                colors[i] = ToRgb((LightColor)i, ref this.mEntryArray[0]);
                }

	            return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for (var i = 0; i < this.mEntryArray.Length; ++i)
            {
                if (i + 1 >= this.mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = this.mEntryArray[eIndex1].timeValues;
                    t2 = this.mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (this.mEntryArray[i].timeValues > time || this.mEntryArray[i + 1].timeValues <= time)
                {
	                continue;
                }

	            eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = this.mEntryArray[eIndex1].timeValues;
                t2 = this.mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
            {
	            return false;
            }

	        if (t1 >= t2)
            {
                for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
                {
	                colors[i] = ToRgb((LightColor)i, ref this.mEntryArray[eIndex1]);
                }

	            return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float)diff;

            for (var i = 0; i < (int)LightColor.MaxLightType; ++i)
            {
                var v1 = ToRgb((LightColor)i, ref this.mEntryArray[eIndex1]);
                var v2 = ToRgb((LightColor)i, ref this.mEntryArray[eIndex2]);
                colors[i] = v2 * sat + v1 * (1 - sat);
            }

            return true;
        }

        public bool GetAllFloatsForTime(int time, float[] floats)
        {
            if (this.mDataEntries.Count == 0)
            {
	            return false;
            }

	        if (this.mEntryArray.Length != this.mDataEntries.Count)
	        {
		        this.mEntryArray = this.mDataEntries.ToArray();
	        }

	        if(this.mEntryArray.Length == 1)
            {
                for (var i = 0; i < (int) LightFloat.MaxLightFloat; ++i)
                {
	                floats[i] = ToFloat((LightFloat) i, ref this.mEntryArray[0]);
                }

	            return true;
            }

            var maxTime = this.mEntryArray[this.mEntryArray.Length - 1].timeValues;
            if(maxTime == 0 || this.mEntryArray[0].timeValues > time)
            {
                for (var i = 0; i < (int)LightFloat.MaxLightFloat; ++i)
                {
	                floats[i] = ToFloat((LightFloat)i, ref this.mEntryArray[0]);
                }

	            return true;
            }

            time %= 2880;

            var eIndex1 = -1;
            var eIndex2 = -1;
            var hasLight = false;
            var t1 = 1u;
            var t2 = 1u;

            for(var i = 0; i < this.mEntryArray.Length; ++i)
            {
                if(i + 1 >= this.mEntryArray.Length)
                {
                    eIndex1 = i;
                    eIndex2 = 0;
                    hasLight = true;
                    t1 = this.mEntryArray[eIndex1].timeValues;
                    t2 = this.mEntryArray[eIndex2].timeValues + 2880;
                    break;
                }

                if (this.mEntryArray[i].timeValues > time || this.mEntryArray[i + 1].timeValues <= time)
                {
	                continue;
                }

	            eIndex1 = i;
                eIndex2 = i + 1;
                hasLight = true;
                t1 = this.mEntryArray[eIndex1].timeValues;
                t2 = this.mEntryArray[eIndex2].timeValues;
                break;
            }

            if (hasLight == false)
            {
	            return false;
            }

	        if(t1 >= t2)
            {
                for (var i = 0; i < (int) LightFloat.MaxLightFloat; ++i)
                {
	                floats[i] = ToFloat((LightFloat) i, ref this.mEntryArray[eIndex1]);
                }

	            return true;
            }

            var diff = t2 - t1;
            var sat = (time - t1) / (float) diff;

            for (var i = 0; i < (int)LightFloat.MaxLightFloat; ++i)
            {
                var v1 = ToFloat((LightFloat) i, ref this.mEntryArray[eIndex1]);
                var v2 = ToFloat((LightFloat) i, ref this.mEntryArray[eIndex2]);
                floats[i] = v2 * sat + v1 * (1 - sat);
            }

            return true;
        }

        public void AddDataEntry(ref LightDataEntry e)
        {
	        this.mDataEntries.Add(e);
        }

        public void AddAllData(IEnumerable<LightDataEntry> e)
        {
	        this.mDataEntries.AddRange(e);
	        this.mDataEntries.Sort(
                (e1, e2) => (e1.timeValues < e2.timeValues) ? -1 : ((e1.timeValues > e2.timeValues) ? 1 : 0));
        }

	    private static void ToRgb(uint value, ref Vector3 color)
        {
            color.X = ((value & 0x00FF0000) >> 16) / 255.0f;
            color.Y = ((value & 0x0000FF00) >> 8) / 255.0f;
            color.Z = ((value & 0x000000FF) >> 0) / 255.0f;
        }

	    private Vector3 ToRgb(LightColor colorType, ref LightDataEntry e)
        {
            var ret = new Vector3();
            switch(colorType)
            {
                case LightColor.Ambient:
                    ToRgb(e.globalAmbient, ref ret);
                    break;

                case LightColor.Diffuse:
                    ToRgb(e.globalDiffuse, ref ret);
                    break;

                case LightColor.Top:
                    ToRgb(e.skyColor0, ref ret);
                    break;

                case LightColor.Middle:
                    ToRgb(e.skyColor1, ref ret);
                    break;

                case LightColor.MiddleLower:
                    ToRgb(e.skyColor2, ref ret);
                    break;

                case LightColor.Lower:
                    ToRgb(e.skyColor3, ref ret);
                    break;

                case LightColor.Horizon:
                    ToRgb(e.skyColor4, ref ret);
                    break;

                case LightColor.Fog:
                    ToRgb(e.fogColor, ref ret);
                    break;

                case LightColor.Sun:
                    ToRgb(e.sunColor, ref ret);
                    break;

                case LightColor.Halo:
                    ToRgb(e.haloColor, ref ret);
                    break;

                case LightColor.Cloud:
                    ToRgb(e.cloudColor, ref ret);
                    break;

                default:
                    throw new ArgumentException("Invalid light type");
            }

            return ret;
        }

	    private float ToFloat(LightFloat type, ref LightDataEntry e)
        {
            switch(type)
            {
                case LightFloat.FogDensity:
                    return e.fogDensity;

                case LightFloat.FogEnd:
                    return e.fogEnd;

                case LightFloat.FogScale:
                    return e.fogScaler;

                default:
                    throw new ArgumentException("Light type not supported yet or invalid");
            }
        }
    }
}
