using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Knjiznica
{
    internal class Program
    {
        struct Borrow()
        {
            public Guid id;
            public string title;
            public string author;
            public DateOnly borrowDay;
            public DateOnly? returnDay;
            public int lateDays;
            public int lateFee;
        };

        struct Member
        {
            public Guid id;
            public string name;
            public string surname;
            public DateOnly birthDay;
            public List<Borrow> borrows;
        };

        static void AddMember(List<Member> members, string name, string surname, DateOnly date, List<Borrow> borrows)
        {
            Member member = new Member();
            member.id = Guid.NewGuid();
            member.name = name;
            member.surname = surname;
            member.birthDay = date;
            member.borrows = borrows ?? new List<Borrow>();

            members.Add(member);
        }

        static void RemoveMember(List<Member> members, string input, string choice)
        {
            if (choice == "1")
            {
                int index = members.FindIndex(o => o.id == new Guid(input));
                if (index != -1)
                {
                    members.RemoveAt(index);
                }
            }

            else
            {
                int index = members.FindIndex(o => o.name == input);
                if (index != -1)
                {
                    members.RemoveAt(index);
                }
            }
        }

        static void UpdateMember(List<Member> members, string id, string name, string surname, DateOnly? date)
        {
            int index = members.FindIndex(o => o.id == new Guid(id));
            if (index != -1)
            {
                Member member = members[index];

                if (name != "0")
                {
                    member.name = name;
                }

                if (surname != "0")
                {
                    member.surname = surname;
                }

                if (date != null)
                {
                    member.birthDay = (DateOnly)date;
                }
            }
        }

        static void PrintMembersMenu(List<Member> members)
        {
            Console.WriteLine("1 - Unos novog člana");
            Console.WriteLine("2 - Brisanje člana");
            Console.WriteLine("3 - Uređivanje člana");
            Console.WriteLine("4 - Pregled svih članova");
            Console.WriteLine("0 - Povratak na glavni izbornik");

            while (true)
            {
                Console.Write("[Izbornik članova] Odabir: ");
                string user_input = Console.ReadLine();

                Console.Write("\n");
                if (user_input == "0")
                {
                    break;
                }

                else if (user_input == "1")
                {
                    Console.Write("Unesi ime: ");
                    string name = Console.ReadLine();

                    Console.Write("Unesi prezime: ");
                    string surname = Console.ReadLine();

                    Console.Write("Unesi datum (YYYY-MM-DD): ");
                    string date_string = Console.ReadLine();

                    Console.Write("Želiš li dodati novog korisnika " + name + " (y ili n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation != "y" && confirmation != "yes")
                    {
                        continue;
                    }

                    if (name == "")
                    {
                        Console.WriteLine("Ime ne može biti prazno!\n");
                        continue;
                    }

                    if (surname == "")
                    {
                        Console.WriteLine("Prezime ne može biti prazno!\n");
                        continue;
                    }

                    if (date_string == "")
                    {
                        Console.WriteLine("Datum rođenja ne može biti prazan!\n");
                        continue;
                    }

                    DateOnly date = new DateOnly();
                    try
                    {
                        date = DateOnly.ParseExact(date_string, "yyyy-MM-dd");
                    }

                    catch (FormatException ex)
                    {
                        Console.WriteLine("Točan oblik datuma je YYYY-MM-DD!");
                    }

                    finally
                    {
                        if (date > DateOnly.FromDateTime(DateTime.Today))
                        {
                            Console.WriteLine("Datum rođenja ne može biti u budućnosti!\n");
                        }

                        else
                        {
                            AddMember(members, name, surname, date, null);
                            Console.WriteLine("Korisnik " + name + " uspješno dodan!\n");
                        }
                    }
                }

                else if (user_input == "2")
                {
                    Console.Write("1 - po ID-u | 2 - po imenu i prezimenu ([prezime] [ime]): ");
                    string choice = Console.ReadLine();

                    Console.Write("Unesi: ");
                    string input = Console.ReadLine();

                    Console.Write("Jeste li sigurni da želite izbrisati korisnika " + input + " (y ili n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation != "y" && confirmation != "yes")
                    {
                        Console.WriteLine("Odustajem...\n");
                        continue;
                    }

                    if (input == "")
                    {
                        Console.WriteLine("ID ili ime korisnika ne može biti prazno!\n");
                        continue;
                    }

                    if (choice == "1" || choice == "2")
                    {
                        RemoveMember(members, input, choice);
                        Console.WriteLine("Korisnik " + input + " uspješno izbrisan!\n");
                    }

                    else
                    {
                        Console.WriteLine("Krivi odabir!\n");
                    }
                }

                else if (user_input == "3")
                {
                    Console.Write("Unesi ID trenutnog člana: ");
                    string id = Console.ReadLine();

                    Console.Write("Unesi novo ime ili 0 za ostaviti staro: ");
                    string name = Console.ReadLine();

                    Console.Write("Unesi novo prezime ili 0 za ostaviti staro: ");
                    string surname = Console.ReadLine();

                    Console.Write("Unesi novi datum (YYYY-MM-DD) ili 0 za ostaviti staro: ");
                    string date_string = Console.ReadLine();

                    Console.Write("Želite li urediti korisnika " + id + " (y ili n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation != "y" && confirmation != "yes")
                    {
                        Console.WriteLine("Odustajem...");
                        continue;
                    }

                    if (name == "")
                    {
                        Console.WriteLine("Ime ne može biti prazno!\n");
                        continue;
                    }

                    if (surname == "")
                    {
                        Console.WriteLine("Prezime ne može biti prazno!\n");
                        continue;
                    }

                    if (date_string == "")
                    {
                        Console.WriteLine("Datum rođenja ne može biti prazan!\n");
                        continue;
                    }

                    if (date_string != "0")
                    {
                        DateOnly date = new DateOnly();
                        try
                        {
                            date = DateOnly.ParseExact(date_string, "yyyy-MM-dd");
                        }

                        catch (FormatException ex)
                        {
                            Console.WriteLine("Točan oblik datuma je YYYY-MM-DD!");
                        }

                        finally
                        {
                            if (date > DateOnly.FromDateTime(DateTime.Today))
                            {
                                Console.WriteLine("Datum rođenja ne može biti u budućnsoti!\n");
                            }

                            else
                            {
                                UpdateMember(members, id, name, surname, date);
                                Console.WriteLine("Korisnik " + id + "uspješno ažuriran!\n");
                            }
                        }
                    }

                    else
                    {
                        UpdateMember(members, id, name, surname, null);
                        Console.WriteLine("Korisnik " + id + "uspješno ažuriran!\n");
                    }
                }

                else if (user_input == "4")
                {
                    members.Sort((a, b) => a.name.CompareTo(b.name));

                    Console.WriteLine("Ispis članova abecedno: ");
                    foreach (Member member in members)
                    {
                        Console.WriteLine("[" + member.id + "] " + member.surname + ", " + member.name +
                                          " (" + (DateTime.Now.Year - member.birthDay.Year) + ") - broj posudbi: "
                                          + member.borrows.Count);
                    }

                    Console.WriteLine("\nČlanovi mlađi od 18: ");
                    foreach (Member member in members)
                    {
                        if (DateTime.Now.Year - member.birthDay.Year <= 18)
                        {
                            Console.WriteLine("[" + member.id + "] " + member.surname + ", " + member.name +
                                              " (" + (DateTime.Now.Year - member.birthDay.Year) + ") - broj posudbi: "
                                              + member.borrows.Count);

                        }
                    }

                    Console.WriteLine("\nSvi članovi s barem jednom ne vraćenom knjigom: ");
                    foreach (Member member in members)
                    {
                        bool flag = false;
                        foreach (Borrow borrow in member.borrows)
                        {
                            if (borrow.returnDay == null)
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag == true)
                        {
                            Console.WriteLine("[" + member.id + "] " + member.surname + ", " + member.name +
                                              " (" + (DateTime.Now.Year - member.birthDay.Year) + ") - broj posudbi: "
                                              + member.borrows.Count);
                        }
                    }

                    Console.Write("\n");
                }

                else
                {
                    Console.WriteLine("Krivi unos!\n");
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

            while (true)
            {
                Console.Write("[Izbornik posudbi] Odabir: ");
                string user_input = Console.ReadLine();

                Console.Write("\n");
                if (user_input == "0")
                {
                    break;
                }

                else if (user_input == "1")
                {

                }

                else if (user_input == "2")
                {

                }

                else if (user_input == "3")
                {
                    
                }

                else if (user_input == "4")
                {

                }

                else if (user_input == "5")
                {
                    
                }

                else if (user_input == "6")
                {

                }

                else
                {
                    Console.WriteLine("Krivi unos!\n");
                }
            }
        }

        static void MainMenu(List<Member> members)
        {
            while (true)
            {
                Console.Write("[Glavni izbornik] Odabir: ");
                string user_input = Console.ReadLine();

                Console.Write("\n");
                if (user_input == "0")
                {
                    System.Environment.Exit(0);
                }

                else if (user_input == "1")
                {
                    PrintMembersMenu(members);
                }

                else if (user_input == "2")
                {
                    PrintBorrowMenu();
                }

                else
                {
                    Console.WriteLine("Krivi unos!\n");
                }
            }
        }

        static void Main(string[] args)
        {
            List<Member> members = new List<Member>();
            AddMember(members, "Jabuka", "Voce", new DateOnly(2000, 4, 10), null);
            AddMember(members, "Kruska", "Voce", new DateOnly(2000, 4, 1), null);
            AddMember(members, "Bundeva", "Povrce", new DateOnly(2000, 6, 2), null);

            Console.WriteLine("Evidencija posudbi u knjižnici\n");

            Console.WriteLine("1 - Članovi");
            Console.WriteLine("2 - Posudbe");
            Console.WriteLine("0 - Izlaz iz aplikacije");

            MainMenu(members);
        }
    }
}
