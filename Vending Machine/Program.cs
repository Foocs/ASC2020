using System;
using System.Collections.Generic;

namespace Vending_Machine
{

    #region Context

    class Context
    {
        State _state = null;
        internal static int balance { get; set; }
        internal Context(State state)
        {
            TransitionTo(state);
        }

        internal void TransitionTo(State state)
        {
            Output.CurrentBalance();

            _state = state;
            _state.SetContext(this);
            Input.GetCoin(this);
        }
        internal void NickelRequest()
        {
            balance += 5;
            _state.NickelHandle();
        }

        internal void DimeRequest()
        {
            balance += 10;
            _state.DimeHandle();
        }

        internal void QuarterRequest()
        {
            balance += 25;
            _state.QuarterHandle();
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

        internal abstract void NickelHandle();
        internal abstract void DimeHandle();
        internal abstract void QuarterHandle();

    }

    class ConcreteStateA : State
    {
        internal override void NickelHandle()
        {
            _context.TransitionTo(new ConcreteStateB());
        }
        internal override void DimeHandle()
        {
            _context.TransitionTo(new ConcreteStateC());
        }
        internal override void QuarterHandle()
        {
            Output.ReturnNickel();
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }

    }
    class ConcreteStateB : State
    {
        internal override void NickelHandle()
        {
            _context.TransitionTo(new ConcreteStateC());
        }
        internal override void DimeHandle()
        {
            _context.TransitionTo(new ConcreteStateD());
        }
        internal override void QuarterHandle()
        {
            Output.ReturnDime();
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }
    }
    class ConcreteStateC : State
    {
        internal override void NickelHandle()
        {
            _context.TransitionTo(new ConcreteStateD());
        }
        internal override void DimeHandle()
        {
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }
        internal override void QuarterHandle()
        {
            Output.ReturnDime();
            Output.ReturnNickel();
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }
    }
    class ConcreteStateD : State
    {
        internal override void NickelHandle()
        {
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }
        internal override void DimeHandle()
        {
            Output.ReturnNickel();
            Output.DispenseMerchandise();

            _context.TransitionTo(new ConcreteStateA());
        }
        internal override void QuarterHandle()
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
        static internal void WelcomeMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  Welcome to the Vending Machine!       ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"     ___      .-""""-.      ___     ");
            Console.WriteLine(@"     \  ""-.  /      \  .-""  /     ");
            Console.WriteLine(@"      > -=.\/        \/.=- <        ");
            Console.WriteLine(@"      > -='/\        /\'=- <        ");
            Console.WriteLine(@"     /__.-'  \      /  '-.__\       ");
            Console.WriteLine(@"              '-..-'                ");
            Console.WriteLine();

            Console.ResetColor();
        }
        static internal void CurrentBalance()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("  Current Balance: ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{Context.balance / 100f:C}");

            Console.ResetColor();
        }
        static internal void DispenseMerchandise()
        {
            Context.balance = 0;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Merchandise dispensed.");
            Console.WriteLine();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  Try another merchandise.");

            Console.ResetColor();
        }

        static internal void ReturnNickel()
        {
            Console.WriteLine("  Returned a nickel in change.");
        }
        static internal void ReturnDime()
        {
            Console.WriteLine("  Returned a dime in change.");
        }
    }

    static class Input
    {
        static void Message()
        {
            Console.Write("  Introduce a ");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("N - Nickel");
            Console.ResetColor();
            Console.Write(", a ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("D - Dime");
            Console.ResetColor();
            Console.Write(" or a ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Q - Quarter");
            Console.ResetColor();
            Console.WriteLine();

            Console.Write("  Input = ");
        }
        static internal void GetCoin(Context context)
        {
            Message();

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

            Dictionary<char, Action> requests = new Dictionary<char, Action>()
                {

                {'N', () => context.NickelRequest()},
                {'D', () => context.DimeRequest()},
                {'Q', () => context.QuarterRequest()},
            };

            requests[coinType]();
        }
    }

    #endregion

    #region Driver Code
    class Vending_Machine
    {

        static internal void Main(string[] args)
        {

            try
            {
                Output.WelcomeMessage();
                var context = new Context(new ConcreteStateA());
            }

            catch (Exception e)
            {
                Console.WriteLine($"  {e.Message}");
            }
        }

    }
    #endregion
}
