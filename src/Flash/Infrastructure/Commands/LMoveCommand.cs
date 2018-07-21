namespace Flash.Infrastructure.Commands
{
    public class LMoveCommand : ICommand
    {
        public Vector FirstDirection { get; }
        public Vector SecondDirection { get; }

        public LMoveCommand(Vector firstDirection, Vector secondDirection)
        {
            FirstDirection = firstDirection;
            SecondDirection = secondDirection;
        }

        public void Apply(State state, Bot bot)
        {
            var newPosition = bot.Pos + FirstDirection + SecondDirection;

            bot.Pos = newPosition;
            state.Energy += 2*(FirstDirection.Mlen + 2 + SecondDirection.Mlen);
        }
    }
}
