// See https://aka.ms/new-console-template for more information


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            int[] ages = new int[3];
            string[] name = new string[3];
            string[] surname = new string[3];
            int[] salarly = new int[3];
            
            for(i=0; i<3;i++)
            {
                Console.WriteLine("Enter your Age");
                ages[i] = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter your name");
                name[i] = Console.ReadLine();
                Console.WriteLine("Enter your surname");
                surname[i] = Console.ReadLine();
                Console.WriteLine("Enter your salarly");
                salarly[i] = int.Parse(Console.ReadLine());
                Console.WriteLine("\n\r------------------------");
                
            }
            for (i = 0; i < 3; i++)
            {
                Console.WriteLine("Name: " + name[i]);
                Console.WriteLine("Surname: " + surname[i]);
                Console.WriteLine("Ages: " + ages[i]);
                Console.WriteLine("Salarly: " + salarly[i]);
                Console.WriteLine("\n\r------------------------");
            }

            foreach(int data in ages)
            {
                Console.WriteLine(data);
            }


        }
    }
}