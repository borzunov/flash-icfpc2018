namespace Flash.Infrastructure.Models
{
    public class Bot
    {
        public Bot(int bid, Vector pos, int[] seeds)
        {
            Bid = bid;
            Pos = pos;
            Seeds = seeds;
        }

        public int Bid;
        public Vector Pos;
        public int[] Seeds;
    }
}
