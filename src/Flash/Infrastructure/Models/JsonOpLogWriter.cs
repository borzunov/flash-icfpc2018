﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Flash.Infrastructure.Models
{
    public class JsonOpLogWriter : IOpLogWriter
    {
        private readonly IJsonWriter jsonWriter;
        private int r;
        private string name;

        private List<object> opLog = new List<object>();

        public JsonOpLogWriter(IJsonWriter jsonWriter)
        {
            this.jsonWriter = jsonWriter;
        }

        public void WriteLogName(string logName)
        {
            this.name = logName;
        }

        public void WriteResolution(int resolution)
        {
            r = resolution;
        }

        public void WriteInitialState(State state)
        {
            WriteResolution(state.Matrix.R);
            WriteHarmonic(state.Harmonics);
            WriteAdd(state.Bots[0].Pos);
            WriteEnergy(0);
        }

        public void WriteFill(Vector v)
        {
            opLog.Add(new { p = $"{v.X}/{v.Y}/{v.Z}", t = 0 });
        }

        public void WriteEnergy(long energy)
        {
            opLog.Add(new { e = energy, t = 4 });
        }

        public void WriteAdd(Vector v)
        {
            opLog.Add(new { p = $"{v.X}/{v.Y}/{v.Z}", t = 1 });
        }
		
	    public void WriteColor(Vector v, string color, double opacity)
	    {
		    opLog.Add(new { p = $"{v.X}/{v.Y}/{v.Z}", t = 6, c = color, o = Math.Min(opacity, 1) });
	    }

		public void WriteRemove(Vector v)
        {
            opLog.Add(new { p = $"{v.X}/{v.Y}/{v.Z}", t = 2 });
        }

        public void WriteMessage(string msg)
        {
            opLog.Add(new { m = msg, t = 3 });
        }

        public void WriteHarmonic(bool high)
        {
            opLog.Add(new { h = high, t = 5 });
        }

        public void WriteGroupAdd(Vector[] points)
        {
            opLog.Add(new { p = points.Select(v => $"{v.X}/{v.Y}/{v.Z}"), t = 7 });
        }

        public void WriteGroupRemove(Vector[] points)
        {
            opLog.Add(new { p = points.Select(v => $"{v.X}/{v.Y}/{v.Z}"), t = 8 });
        }

        public void Save()
        {
            var result = new
            {
                size = r,
                createdAt = DateTime.Now.Ticks,
                log = opLog,
                name
            };

            jsonWriter.Write(result);
        }
    }
}