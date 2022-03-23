// See https://aka.ms/new-console-template for more information


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int score;
            Console.WriteLine("Enter your score 0-100");
            do
            {
                score = int.Parse(Console.ReadLine());
                if (score >= 80 && score <= 100)
                {
                    Console.WriteLine("Your Grade is A");
                }
                else if (score >= 70 && score <= 79)
                {
                    Console.WriteLine("Your Grade is B");
                }
                else if (score >= 60 && score <= 69)
                {
                    Console.WriteLine("Your Grade is C");
                }
                else if (score >= 50 && score <= 59)
                {
                    Console.WriteLine("Your Grade is D");
                }
                else if (score >= 0 && score < 50)
                {
                    Console.WriteLine("Your Grade is F = not pass");
                }
                else
                {
                    Console.WriteLine("error plz try again") ;
                }
            }
            while (score < 0 || score > 100);
        }
    }
}
