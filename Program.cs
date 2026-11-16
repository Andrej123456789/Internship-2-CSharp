namespace Knjiznica
{
    internal class Program
    {
        static void MembersMenu()
        {

        }

        static void BorrowMenu()
        {

        }

        static void MainMenu()
        {
            while (true)
            {
                Console.Write("Odabir: ");

                char user_input = '\0';
                try
                {
                    user_input = Convert.ToChar(Console.ReadLine());
                }

                catch (Exception ex)
                {
                    
                }

                switch (user_input)
                {
                    case '0':
                        System.Environment.Exit(0);
                        break;

                    case '1':
                        MainMenu();
                        break;

                    case '2':
                        BorrowMenu();
                        break;

                    default:
                        Console.WriteLine("Krivi unos!");
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Evidencija posudbi u knjižnici");

            Console.WriteLine("1 - Članovi");
            Console.WriteLine("2 - Posudbe");
            Console.WriteLine("0 - Izlaz iz aplikacije");

            MainMenu();
        }
    }
}
