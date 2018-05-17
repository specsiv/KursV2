namespace KursV2.Helpers
{
    class Command
    {
        public int q { get; private set; }
        public string value { get; private set; }
        public int dimension { get; private set; }
        public Moves move { get; private set; }

        public Command(int q, string value, int dimension, Moves move)
        {
            this.q = q;
            this.value = value;
            this.dimension = dimension;
            this.move = move;
        }
    }
}