using System.Diagnostics.Metrics;
using System.Linq;
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
            public double lateFee;
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

        static int RemoveMember(List<Member> members, string input)
        {
            Guid.TryParse(input, out Guid guid);
            int index = members.FindIndex(o => o.id == guid || o.name == input);

            if (index != -1)
            {
                bool flag = false;
                foreach (Borrow borrow in members[index].borrows)
                {
                    if (borrow.returnDay == null)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag == false)
                {
                    members.RemoveAt(index);
                    return 0;
                }

                else
                {
                    return 2;
                }
            }

            return 1;
        }

        static int UpdateMember(List<Member> members, string id, string name, string surname, DateOnly? date)
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

                return 0;
            }

            return 1;
        }

        static int CalculateLateDays(DateOnly borrowDay, DateOnly? returnDay)
        {
            int result = (returnDay?.Day ?? 0) - borrowDay.AddDays(21).Day;
            return result > 0 ? result : 0;
        }

        static double CalculateLateFee(int lateDays)
        {
            return lateDays * 0.50;
        }

        static void AddBorrow(List<Member> members, string member, string title, string author, DateOnly date)
        {
            Guid.TryParse(member, out Guid guid);
            int index = members.FindIndex(o => o.id == guid || o.name == member);

            if (index != -1)
            {
                Borrow borrow = new Borrow();
                borrow.id = Guid.NewGuid();
                borrow.title = title;
                borrow.author = author;
                borrow.borrowDay = date;
                borrow.returnDay = null;
                borrow.lateDays = CalculateLateDays(borrow.borrowDay, borrow.returnDay);
                borrow.lateFee = CalculateLateFee(borrow.lateDays);

                members[index].borrows.Add(borrow);
            }
        }
        static (int, Borrow?) ReturnBorrow(List<Member> members, string id, DateOnly date)
        {
            Guid borrowId = new Guid(id);
            var result = members
                .SelectMany(m => m.borrows, (member, borrow) => new { member, borrow })
                .FirstOrDefault(x => x.borrow.id == borrowId);

            if (result != null && result.borrow.id == borrowId)
            {
                var member = result.member;
                var borrow = result.borrow;

                if (borrow.returnDay != null)
                {
                    return (2, null);
                }

                borrow.returnDay = date;
                borrow.lateDays = CalculateLateDays(borrow.borrowDay, borrow.returnDay);
                borrow.lateFee = CalculateLateFee(borrow.lateDays);

                return (0, borrow);
            }

            else
            {
                return (1, null);
            }
        }

        static int RemoveBorrow(List<Member> members, string input)
        {
            Guid borrowId = new Guid(input);
            var result = members
                .SelectMany(m => m.borrows, (member, borrow) => new { member, borrow })
                .FirstOrDefault(x => x.borrow.id == borrowId);

            if (result != null && result.borrow.id == borrowId)
            {
                var member = result.member;
                var borrow = result.borrow;

                member.borrows.Remove(borrow);
                return 0;
            }

            else
            {
                Guid.TryParse(input, out Guid guid);
                int index = members.FindIndex(o => o.id == guid || o.name == input);

                if (index != -1)
                {
                    members[index].borrows.Clear();
                    return 0;
                }

                else
                {
                    foreach (Member member in members)
                    {
                        foreach (Borrow borrow in member.borrows)
                        {
                            int fee = 0;
                            try
                            {
                                fee = Convert.ToInt32(input); 
                            }

                            catch (Exception ex)
                            {
                                return 1;
                            }

                            if (borrow.lateFee > fee)
                            {
                                member.borrows.Remove(borrow);
                            }
                        }
                    }

                    return 0;
                }
            }
        }

        static int UpdateBorrow(List<Member> members, string id, string title, string author, DateOnly? borrowDate, DateOnly? returnDate)
        {
            Guid borrowId = new Guid(id);
            var result = members
                .SelectMany(m => m.borrows, (member, borrow) => new { member, borrow })
                .FirstOrDefault(x => x.borrow.id == borrowId);

            if (result != null && result.borrow.id == borrowId)
            {
                var member = result.member;
                var borrow = result.borrow;

                borrow.title = title;
                borrow.author = author;

                if (borrowDate != null)
                {
                    borrow.borrowDay = (DateOnly)borrowDate;
                }

                if (returnDate != null)
                {
                    borrow.returnDay = (DateOnly)returnDate;
                }

                return 0;
            }

            else
            {
                return 1;
            }
        }

        static void PrintBorrow(Borrow borrow)
        {
            Console.WriteLine("[" + borrow.id + "] \"" + borrow.title + "\" - " + borrow.author +
                              " | posuđeno: " + borrow.borrowDay +
                              " | vraćeno: " + (borrow.returnDay == null ? "nije vraćeno" : borrow.returnDay.ToString()) +
                              " | kašnjenje: " + borrow.lateDays +
                              " | zakasnina: " + borrow.lateFee + "€");
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
                    Console.Write("Unesi ID ili ime korisnika: ");
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

                    if (RemoveMember(members, input) == 0)
                    {
                        Console.WriteLine("Korisnik " + input + " uspješno izbrisan!\n");
                    }

                    else if (RemoveMember(members, input) == 2)
                    {
                        Console.WriteLine("Korisnik ima nevraćnu knjigu te ga nije moguće obrisati!\n");
                    }

                    else
                    {
                        Console.WriteLine("Korisnik " + input + " nije pronađen!\n");
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
                                if (UpdateMember(members, id, name, surname, date) == 0)
                                {
                                    Console.WriteLine("Korisnik " + id + "uspješno ažuriran!\n");
                                }

                                else
                                {
                                    Console.WriteLine("Korisnik " + id + " nije pronađen!\n");
                                }
                            }
                        }
                    }

                    else
                    {
                        if (UpdateMember(members, id, name, surname, null) == 0)
                        {
                            Console.WriteLine("Korisnik " + id + "uspješno ažuriran!\n");
                        }

                        else
                        {
                            Console.WriteLine("Korisnik " + id + " nije pronađen!\n");
                        }
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

        static void PrintBorrowMenu(List<Member> members)
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
                    Console.Write("Odaberite člana: ");
                    string member = Console.ReadLine();

                    Console.Write("Unesite naslov: ");
                    string title = Console.ReadLine();

                    Console.Write("Unesite autora: ");
                    string author = Console.ReadLine();

                    Console.Write("Unesite datum posudbe: ");
                    string date_string = Console.ReadLine();

                    if (member == "")
                    {
                        Console.WriteLine("Ime ili ID člana ne može biti prazno!\n");
                        continue;
                    }

                    if (title == "")
                    {
                        Console.WriteLine("Naslov ne može biti prazan!\n");
                        continue;
                    }

                    if (author == "")
                    {
                        Console.WriteLine("Ime autora ne može biti prazno!\n");
                        continue;
                    }

                    if (date_string == "")
                    {
                        Console.WriteLine("Datum ne može biti prazan!\n");
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
                            Console.WriteLine("Datum posudbe ne može biti u budućnosti!\n");
                        }

                        else
                        {
                            AddBorrow(members, member, title, author, date);
                            Console.WriteLine("Posudba je uspješno dodana! Rok za vraćanje: " + date.AddDays(21) + "\n");
                        }
                    }
                }

                else if (user_input == "2")
                {
                    Console.Write("Unesite ID posudbe: ");
                    string id = Console.ReadLine();

                    Console.Write("Unesite datum vraćanja (YYYY-MM-DD): ");
                    string date_string = Console.ReadLine();

                    if (id == "")
                    {
                        Console.WriteLine("ID korisnika ne može prazan!\n");
                        continue;
                    }

                    if (date_string == "")
                    {
                        Console.WriteLine("Datum vraćanja ne može biti prazan!\n");
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
                            Console.WriteLine("Datum vraćanja ne može biti u budućnosti!\n");
                        }

                        else
                        {
                            (int status, Borrow? borrow) = ReturnBorrow(members, id, date);
                            if (status == 0)
                            {
                                Console.WriteLine("Knjiga vraćena! Kašnjenje: " + borrow?.lateDays +
                                                  ", zakasnina: " + borrow?.lateFee + "\n");
                            }

                            else if (status == 1)
                            {
                                Console.WriteLine("Knjiga je već vraćena!\n");
                            }

                            else
                            {
                                Console.WriteLine("Posudba " + id + " nije pronađena!\n");
                            }
                        }
                    }
                }

                else if (user_input == "3")
                {
                    Console.Write("Unesite ID posudbe (X za sljedeću opciju): ");
                    string input = Console.ReadLine();

                    if (input == "" || input == "X" || input == "x")
                    {
                        Console.Write("Unesite ID ili ime korisnika za brisanje svih posudbi (X za sljedeću opciju): ");
                        input = Console.ReadLine();

                        if (input == "" || input == "X" || input == "x")
                        {
                            Console.Write("Unesite iznos zakasnine za brisanje svih posudbi sa zakasninom većom od tog iznosa: ");
                            input = Console.ReadLine();

                            if (input == "")
                            {
                                Console.WriteLine("Opcija mora biti odabrana!\n");
                                continue;
                            }
                        }
                    }

                    if (RemoveBorrow(members, input) == 0)
                    {
                        Console.WriteLine("Posudba " + input + " uspješno izbrisana!\n");
                    }

                    else
                    {
                        Console.WriteLine("Nije bilo moguće izbrisati posudbu " + input + "!\n");
                    }
                }

                else if (user_input == "4")
                {
                    Console.Write("Unesi ID trenutne posudbe: ");
                    string id = Console.ReadLine();

                    Console.Write("Unesi novi naslov ili 0 za ostaviti staro: ");
                    string title = Console.ReadLine();

                    Console.Write("Unesi novog autora ili 0 za ostaviti staro: ");
                    string author = Console.ReadLine();

                    Console.Write("Unesi novi datum posudbe (YYYY-MM-DD) ili 0 za ostaviti staro: ");
                    string borrow_date_string = Console.ReadLine();

                    Console.Write("Unesi novi datum vraćanja (YYYY-MM-DD) ili 0 za ostaviti staro: ");
                    string return_date_string = Console.ReadLine();

                    Console.Write("Želite li urediti posudbu " + id + " (y ili n): ");
                    string confirmation = Console.ReadLine();

                    if (confirmation != "y" && confirmation != "yes")
                    {
                        Console.WriteLine("Odustajem...\n");
                        continue;
                    }

                    if (title == "")
                    {
                        Console.WriteLine("Ime ne može biti prazno!\n");
                        continue;
                    }

                    if (author == "")
                    {
                        Console.WriteLine("Prezime ne može biti prazno!\n");
                        continue;
                    }

                    if (borrow_date_string == "")
                    {
                        Console.WriteLine("Datum posudbe ne može biti prazan!\n");
                        continue;
                    }

                    if (return_date_string == "")
                    {
                        Console.WriteLine("Datum vraćanja ne može biti prazan!\n");
                        continue;
                    }

                    DateOnly? borrow_date = null;
                    DateOnly? return_date = null;

                    if (borrow_date_string != "0")
                    {
                        try
                        {
                            borrow_date = DateOnly.ParseExact(borrow_date_string, "yyyy-MM-dd");
                        }

                        catch (FormatException ex)
                        {
                            Console.WriteLine("Točan oblik datuma posudbe je YYYY-MM-DD!\n");
                        }
                    }

                    if (return_date_string != "0")
                    {
                        try
                        {
                            return_date = DateOnly.ParseExact(return_date_string, "yyyy-MM-dd");
                        }

                        catch (FormatException ex)
                        {
                            Console.WriteLine("Točan oblik datuma posudbe je YYYY-MM-DD!\n");
                        }
                    }

                    if (UpdateBorrow(members, id,title, author, borrow_date, return_date) == 0)
                    {
                        Console.WriteLine("Posudba " + id + " uspješno izmijenjena!\n");
                    }

                    else
                    {
                        Console.WriteLine("Posudba " + id + " nije pronađena!\n");
                    }
                }

                else if (user_input == "5")
                {
                    Console.WriteLine("1 - sve posudbe redom");
                    Console.WriteLine("2 - sve posudbe sortirane uzlazno (2u) ili silazno (2s)");
                    Console.WriteLine("3 - sve posudbe sortirane po zakasnini uzlazno (3u) ili silazno (3s)");
                    Console.WriteLine("4 - sve trenutno nevraćene posudbe");
                    Console.WriteLine("5 - pretraga posudbi po dijelu naslova ili autora");
                    Console.WriteLine("6 - posudbe grupirane po mjesecu posudbe");
                    Console.Write("Unesite odabir: ");

                    string choice = Console.ReadLine();
                    if (choice == "1")
                    {
                        foreach (Member member in members)
                        {
                            foreach (Borrow borrow in member.borrows)
                            {
                                PrintBorrow(borrow);
                            }
                        }
                    }

                    else if (choice == "2u")
                    {
                        var borrows = members.SelectMany(m => m.borrows).OrderBy(b => b.borrowDay);
                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "2s")
                    {
                        var borrows = members.SelectMany(m => m.borrows).OrderByDescending(b => b.borrowDay);
                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "3u")
                    {
                        var borrows = members.SelectMany(m => m.borrows).OrderBy(b => b.lateDays);
                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "3s")
                    {
                        var borrows = members.SelectMany(m => m.borrows).OrderByDescending(b => b.lateDays);
                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "4")
                    {
                        var borrows = members.SelectMany(m => m.borrows).Where(b => b.returnDay == null);
                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "5")
                    {
                        Console.Write("Unesite dio naslova ili autora: ");
                        string search = Console.ReadLine();

                        var borrows = members.SelectMany(m => m.borrows)
                                             .Where(b =>
                                                    b.title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                    b.author.Contains(search, StringComparison.OrdinalIgnoreCase));

                        foreach (Borrow borrow in borrows)
                        {
                            PrintBorrow(borrow);
                        }
                    }

                    else if (choice == "6")
                    {
                        var borrows = members.SelectMany(m => m.borrows)
                                             .GroupBy(b => new { b.borrowDay.Year, b.borrowDay.Month })
                                             .OrderBy(g => g.Key.Year)
                                             .ThenBy(g => g.Key.Month);

                        foreach (var group in borrows)
                        {
                            Console.WriteLine("\n" + group.Key.Month + "/" + group.Key.Year);
                            Console.WriteLine("-------------------------");

                            foreach (Borrow borrow in group)
                            {
                                PrintBorrow(borrow);
                            }
                        }
                    }

                    else
                    {
                        Console.WriteLine("Krivi odabir!\n");
                    }
                }

                else if (user_input == "6")
                {
                    Console.WriteLine("Unesite ID ili ime korinsika: ");
                    string input = Console.ReadLine();

                    if (input == "")
                    {
                        Console.WriteLine("ID ili ime korisnika ne može biti prazno!\n");
                        continue;
                    }

                    Guid.TryParse(input, out Guid guid);
                    int index = members.FindIndex(o => o.id == guid || o.name == input);

                    if (index == -1)
                    {
                        Console.WriteLine("Nije moguće pronaći korisnika " + input + "!\n");
                        continue;
                    }

                    Console.WriteLine("Izvještaj za korisnika " + input + ":");

                    int total = members[index].borrows.Count();
                    int totalUnreturned = members[index].borrows.Count(b => b.returnDay == null);
                    Console.WriteLine($"Ukupan broj posudbi: {total} | Ukupan broj nevraćenih knjiga: {totalUnreturned}");

                    double totalFee = members[index].borrows.Sum(b => b.lateFee);
                    Console.WriteLine($"Ukupna zakasnina: {totalFee}");

                    var returned = members[index].borrows.Where(b => b.returnDay != null).ToList();
                    double average = returned.Count > 0
                        ? returned.Average(b => b.returnDay.Value.DayNumber - b.borrowDay.DayNumber)
                        : 0;
                    Console.WriteLine($"Prosječno trajanje posudbe u danima: {returned}");

                    Borrow? largestFee = members[index].borrows.OrderByDescending(b => b.lateDays).FirstOrDefault();
                    if (largestFee != null)
                    {
                        Console.WriteLine("Najveća zakasnina: ");
                        Console.WriteLine($"\tTitle: {largestFee.Value.title}");
                        Console.WriteLine($"\tAuthor: {largestFee.Value.author}");
                        Console.WriteLine($"\tKašnjenje: {largestFee.Value.lateDays} dana");
                    }

                    Console.WriteLine("Unesi početni datum: ");
                    string from_date_string = Console.ReadLine();

                    Console.WriteLine("Unesi završni datum: ");
                    string to_date_string = Console.ReadLine();

                    DateOnly from = new DateOnly();
                    DateOnly to = new DateOnly();

                    try
                    {
                        from = DateOnly.ParseExact(from_date_string, "yyyy-MM-dd");
                        to = DateOnly.ParseExact(to_date_string, "yyyy-MM-dd");
                    }

                    catch (FormatException ex)
                    {
                        Console.WriteLine("Točan oblik datuma je YYYY-MM-DD!\n");
                    }

                    finally
                    {
                        var borrows = members[index].borrows.Where(b => b.borrowDay >= from && b.borrowDay <= to).ToList();
                        foreach (var borrow in borrows)
                        {
                            Console.WriteLine($"{borrow.title} - {borrow.borrowDay}");
                        }
                    }
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
                    PrintBorrowMenu(members);
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
