using System;

namespace Vending_Machine
{
    class Program
    {
        #region Context
        
        class Context
        {
            private State _state = null;

            public Context(State state)
            {
                this.TransitionTo(state);
            }

            public void TransitionTo(State state)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("  Current Balance: ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{balance / 100f:C}");

                Console.ResetColor();

                this._state = state;
                this._state.SetContext(this);
                Input(this);
            }
            public void NickelRequest()
            {
                balance += 5;
                this._state.NickelHandle();
            }

            public void DimeRequest()
            {
                balance += 10;
                this._state.DimeHandle();
            }

            public void QuarterRequest()
            {
                balance += 25;
                this._state.QuarterHandle();
            }
        }

        #endregion

        #region States
        abstract class State
        {
            protected Context _context;

            public void SetContext(Context context)
            {
                _context = context;
            }

            public abstract void NickelHandle();
            public abstract void DimeHandle();
            public abstract void QuarterHandle();

        }

        class ConcreteStateA : State
        {
            public override void NickelHandle()
            {
                _context.TransitionTo(new ConcreteStateB());
            }
            public override void DimeHandle()
            {
                _context.TransitionTo(new ConcreteStateC());
            }
            public override void QuarterHandle()
            {
                Output.ReturnNickel();
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }

        }
        class ConcreteStateB : State
        {
            public override void NickelHandle()
            {
                _context.TransitionTo(new ConcreteStateC());
            }
            public override void DimeHandle()
            {
                _context.TransitionTo(new ConcreteStateD());
            }
            public override void QuarterHandle()
            {
                Output.ReturnDime();
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
        }
        class ConcreteStateC : State
        {
            public override void NickelHandle()
            {
                _context.TransitionTo(new ConcreteStateD());
            }
            public override void DimeHandle()
            {
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
            public override void QuarterHandle()
            {
                Output.ReturnDime();
                Output.ReturnNickel();
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
        }
        class ConcreteStateD : State
        {
            public override void NickelHandle()
            {
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
            public override void DimeHandle()
            {
                Output.ReturnNickel();
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
            public override void QuarterHandle()
            {
                Output.ReturnDime();
                Output.ReturnDime();
                Output.DispenseMerchandise();

                _context.TransitionTo(new ConcreteStateA());
            }
        }

        #endregion

        #region I/O
        static class Output
        {
            static public void DispenseMerchandise()
            {
                balance = 0;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  Merchandise dispensed.");
                Console.WriteLine();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Try another merchandise.");

                Console.ResetColor();
            }

            static public void ReturnNickel()
            {
                Console.WriteLine("  Returned a nickel in change.");
            }
            static public void ReturnDime()
            {
                Console.WriteLine("  Returned a dime in change.");
            }

        }
        static void Input(Context context)
        {
            Console.WriteLine("  Introduce a N - Nickel, a D - Dime or a Q - Quarter");

            Console.Write("  Input = ");
            char coinType = char.Parse(Console.ReadLine().ToUpper());

            while (coinType != 'N' && coinType != 'D' && coinType != 'Q')
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("  Wrong Input.");
                Console.ResetColor();

                Console.Write("  Input = ");
                coinType = char.Parse(Console.ReadLine().ToUpper());
            }

            Console.WriteLine();

            switch (coinType)
            {
                case 'N':
                    context.NickelRequest();
                    break;
                case 'D':
                    context.DimeRequest();
                    break;
                case 'Q':
                    context.QuarterRequest();
                    break;
            }
        }
        #endregion

        #region Driver Code

        static private int balance = 0;
        static void Main(string[] args)
        {

            try
            {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Welcome to the Vending Machine!       ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("     ___      .-\"\"-.      ___         ");
                Console.WriteLine("     \\  \"-.  /      \\  .-\"  /       ");
                Console.WriteLine("      > -=.\\/        \\/.=- <          ");
                Console.WriteLine("      > -='/\\        /\\'=- <           ");
                Console.WriteLine("     /__.-'  \\      /  '-.__\\         ");
                Console.WriteLine("              '-..-'                    ");
                Console.WriteLine();

                Console.ResetColor();
                var context = new Context(new ConcreteStateA());

                Input(context);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        #endregion
    }
}
