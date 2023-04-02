using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneCode
{
    public enum ZoneType { HIGH, LOW, STANDARD };
    public struct ZoneData
    {
        ZoneType type;

        int beatLength;
        int beatIni;
        int beatEnd;
        float timeIniZone;
        float timeEndZone;
        public bool activatedIni;
        bool activatedEnd;

        public ZoneData(ZoneType t, int blength, int bIni, int bEnd, float tIni, float tEnd, bool actIni, bool actEnd)
        {
            type = t;
            beatLength = blength;
            beatIni = bIni;
            beatEnd = bEnd;
            timeIniZone = tIni;
            timeEndZone = tEnd;
            activatedIni = actIni;
            activatedEnd = actEnd;
        }

        //Getters
        public ZoneType getType() { return type; }
        public int getBeatLength() { return beatLength; }
        public int getBeatIni() { return beatIni; }
        public int getBeatEnd() { return beatEnd; }
        public float getTimeIniZone() { return timeIniZone; }
        public float getTimeEndZone() { return timeEndZone; }
        public bool getActivatedIni() { return activatedIni; }
        public bool getActivatedEnd() { return activatedEnd; }

        //Setters
        public void setType(ZoneType t) { type = t; }
        public void setBeatLength(int i) { beatLength = i; }
        public void setBeatIni(int i) { beatIni = i; }
        public void setBeatEnd(int i) { beatEnd = i; }
        public void setTimeIniZone(float i) { timeIniZone = i; }
        public void setTimeEndZone(float i) { timeEndZone = i; }
        public void setActivatedIni(bool i) { activatedIni = i; }
        public void setActivatedEnd(bool i) { activatedEnd = i; }
    }
}