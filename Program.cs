using System.Numerics;

namespace Knjiznica
{
    internal class Program
    {
        struct Borrow()
        {
            Guid id;
            string title;
            string author;
            DateOnly borrowDay;
            DateOnly returnDay;
            int lateDays;
            int lateFee;
        };

        struct Member
        {
            Guid id;
            string name;
            string surname;
            DateOnly birthDay;
            Vector<Borrow> borrows;
        };

        static void PrintMembersMenu()
        {
            Console.WriteLine("1 - Unos novog člana");
            Console.WriteLine("2 - Brisanje člana");
            Console.WriteLine("3 - Uređivanje člana");
            Console.WriteLine("4 - Pregled svih članova");
            Console.WriteLine("0 - Povratak na glavni izbornik");

            bool flag = true;
            while (flag)
            {
                Console.Write("\n[Izbornik članova] Odabir: ");

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
                        flag = false;
                        break;

                    case '1':
                        break;

                    case '2':
                        break;

                    case '3':
                        break;

                    case '4':
                        break;

                    default:
                        Console.WriteLine("Krivi unos!");
                        break;
                }
            }
        }

        static void PrintBorrowMenu()
        {
            Console.WriteLine("1 - Unos nove posudbe");
            Console.WriteLine("2 - Vraćanje knjige");
            Console.WriteLine("3 - Brisanje posudbe");
            Console.WriteLine("4 - Uređivanje postojeće posudbe");
            Console.WriteLine("5 - Pregled svih ponudi");
            Console.WriteLine("6 - Izvještaji i analize");
            Console.WriteLine("0 - povratak na glavni izbornik");

            bool flag = true;
            while (flag)
            {
                Console.Write("\n[Izbornik posudbi] Odabir: ");

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
                        flag = false;
                        break;

                    case '1':
                        break;

                    case '2':
                        break;

                    case '3':
                        break;

                    case '4':
                        break;

                    case '5':
                        break;

                    case '6':
                        break;

                    default:
                        Console.WriteLine("Krivi unos!");
                        break;
                }
            }
        }

        static void MainMenu()
        {
            while (true)
            {
                Console.Write("\n[Glavni izbornik] Odabir: ");

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
                        PrintMembersMenu();
                        break;

                    case '2':
                        PrintBorrowMenu();
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
