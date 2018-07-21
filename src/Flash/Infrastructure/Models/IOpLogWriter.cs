﻿namespace Flash.Infrastructure.Models
{
    public interface IOpLogWriter
    {
        void WriteInitialState(State state);
        void WriteFill(Vector v);
        void WriteEnergy(long energy);
        void WriteAdd(Vector v);
        void WriteRemove(Vector v);
        void WriteMessage(string msg);
        void WriteHarmonic(bool high);
        void Save();
    }
}