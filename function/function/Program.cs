namespace Condition
{
    class program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Enter A number");
            int a = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter B number");
            int b = int.Parse(Console.ReadLine());
            Console.WriteLine("The Result of plus is " + plus(a,b));
            Console.WriteLine("The Result of minus is " + minus(a, b));
            Console.WriteLine("The Result of time is " + time(a, b));
            Console.WriteLine("The Result of devided is " + devided(a, b));

            sayHelloWorld();
        }
        
        private static void sayHelloWorld()
        {
            for(int i = 0; i <10; i++)
            Console.WriteLine("Hello World");
            
        }
        private static int plus(int a, int b)
        {
            return (a+b); 
        }
        private static int minus(int a, int b)
        {
            return (a - b);
        }
        private static int time(int a, int b)
        {
            return (a * b);
        }
        private static int devided(int a, int b)
        {
            return (a / b);
        }
    }
}