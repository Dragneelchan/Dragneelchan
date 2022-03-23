// See https://aka.ms/new-console-template for more information
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string[][] user = new string[3][];
            
            
            for(int i=0 ; i<3 ; i++)
            {
                user[i] = new string[3];
                Console.WriteLine("Enter your name");
                user[i][0] = Console.ReadLine();
                Console.WriteLine("Enter your adress");
                user[i][1] = Console.ReadLine();
                Console.WriteLine("Enter your Telnumber");
                user[i][2] = Console.ReadLine();
                Console.WriteLine("-------\n");
            }

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Name:" + user[i][0]);
               
                Console.WriteLine("Adress:" + user[i][1]);
          
                Console.WriteLine("Tel.number" + user[i][2]);

                Console.WriteLine("-------\n");

            }
        }
    }
}