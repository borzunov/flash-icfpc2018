using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Flash.Infrastructure.Models
{
    public class JsonOpLogWriter : IOpLogWriter
    {
        private readonly IJsonWriter jsonWriter;
        private int r;

        private List<object> opLog = new List<object>();

        public JsonOpLogWriter(IJsonWriter jsonWriter)
        {
            this.jsonWriter = jsonWriter;
        }

        public void WriteInitialState(State state)
        {
            r = state.Matrix.R;
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

        public void Save()
        {
            var result = new
            {
                size = r,
                createdAt = DateTime.Now.Ticks,
                log = opLog
            };

            jsonWriter.Write(result);
        }
    }
}