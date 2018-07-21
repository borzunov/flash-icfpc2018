﻿namespace Flash.Infrastructure.Models
{
    public class FakeOpLog : IOpLogWriter
    {
        public void WriteLogName(string name)
        {
        }

        public void WriteInitialState(State state)
        {
        }

        public void WriteFill(Vector v)
        {
        }

        public void WriteEnergy(long energy)
        {
        }

        public void WriteAdd(Vector v)
        {
        }

        public void WriteRemove(Vector v)
        {
        }

        public void WriteMessage(string msg)
        {
        }

        public void WriteHarmonic(bool high)
        {
        }

        public void Save()
        {
        }

        public void WriteGroupAdd(Vector[] points)
        {
        }

        public void WriteGroupRemove(Vector[] points)
        {
        }
    }
}