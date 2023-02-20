using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Homework1_6_7
{
    class Program
    {
        static void Main(string[] args)
        {
            Depot depot = new Depot();

            while(depot.IsHaveTracks())
            {
                depot.SendTrain();
            }
        }
    }

    class Passenger
    {
        public Passenger (string name, int age, CarriageType wishingCarriageType)
        {
            Name = name;
            Age = age;
            WishingCarriageType = wishingCarriageType;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
        public CarriageType WishingCarriageType { get; private set; }
        public Ticket Ticket { get; private set; }

        public void BuyTicket(Ticket ticket)
        {
            Ticket = ticket;
        }

        public void ShowInfo()
        {
            Ticket.ShowInfo();
        }
    }

    abstract class Carriage
    {
        protected Random Random;
        protected int MinCountSeats;
        protected int MaxCountSeats;

        public Carriage()
        {
            Random = new Random();
            Passengers = new List<Passenger>();
        }

        public int CountSeats { get; protected set; }
        public int Number { get; protected set; }
        public CarriageType Type { get; protected set; }
        public string Title { get; protected set; }
        public List<Passenger> Passengers { get; protected set; }

        public void AddPassenger(Passenger passenger)
        {
            Passengers.Add(passenger);
        }

        public void ShowInfo()
        {
            Console.WriteLine("Всего " + CountSeats + "  мест: ");

            foreach (var passenger in Passengers)
            {
                passenger.ShowInfo();
                Console.WriteLine();
            }
        }
    }

    class LuxuryCarriage : Carriage
    {
        public LuxuryCarriage(int carriageNumber) : base()
        {
            MinCountSeats = 1;
            MaxCountSeats = 10;
            Type = CarriageType.Luxury;
            CountSeats = Random.Next(MinCountSeats, MaxCountSeats+1);
            Number = carriageNumber;
            Title = "СВ " + Number;
        }
    }

    class EconomclassCarriage: Carriage
    {
        public EconomclassCarriage(int carriageNumber) : base()
        {
            MinCountSeats = 20;
            MaxCountSeats = 40;
            Type = CarriageType.Economclass;
            CountSeats = Random.Next(MinCountSeats, MaxCountSeats+1);
            Number = carriageNumber;
            Title = "Эконом-класс " + Number;
        }
    }

    class CompartmentCarriage: Carriage
    {
        public CompartmentCarriage(int carriageNumber) : base()
        {
            MinCountSeats = 16;
            MaxCountSeats = 32;
            Type = CarriageType.Compartment;

            do
            {
                CountSeats = Random.Next(MinCountSeats, MaxCountSeats + 1);
            } while (CountSeats % 4 !=0);

            Number = carriageNumber;
            Title = "Купе " + Number;
        }
    }

    class Train
    {
        private Dictionary<CarriageType, int> _countCarriages;

        public Train (Dictionary<CarriageType, int> countPassengersCarriage)
        {
            Carriages = new List<Carriage>();

            AddAllCarriageTypes();
            CreateCarriages(countPassengersCarriage);
        }

        public List<Carriage> Carriages{ get; private set; }

        public void ShowInfo()
        {
            foreach (var carriage in Carriages)
            {
                Console.WriteLine(carriage.Title + ":");
                carriage.ShowInfo();
                Console.WriteLine();
                Console.ReadKey();
            }
        }

        public void ShowInfoCarriages()
        {
            int carriageNumber = 1;

            foreach (var carriage in Carriages)
            {
                Console.WriteLine(carriageNumber + ": " + carriage.Type + ". Занято мест " + carriage.Passengers.Count + "/" + carriage.CountSeats);
                carriageNumber++;
            }

            Console.WriteLine();
            Console.WriteLine("Итого:");

            foreach (var carriage in _countCarriages)
            {
                Console.WriteLine(carriage.Key + ": " + carriage.Value);
            }
        }

        public void CreateCarriages(Dictionary<CarriageType, int> countPassengersCarriage)
        {
            int carriageNumber=1;
            int countPassengersCarriageCounter;

            for (int i = 0; i < (int)CarriageType.Lenght; i++)
            {
                countPassengersCarriageCounter = 0;

                while (countPassengersCarriageCounter < countPassengersCarriage[(CarriageType)i])
                {
                    if ((CarriageType)i== CarriageType.Luxury)
                    {
                        Carriages.Add(new LuxuryCarriage(carriageNumber));
                    }
                    else if ((CarriageType)i == CarriageType.Economclass)
                    {
                        Carriages.Add(new EconomclassCarriage(carriageNumber));
                    }
                    else if ((CarriageType)i == CarriageType.Compartment)
                    {
                        Carriages.Add(new CompartmentCarriage(carriageNumber));
                    }

                    _countCarriages[(CarriageType)i]++;
                    countPassengersCarriageCounter += Carriages[carriageNumber - 1].CountSeats;
                    carriageNumber++;
                }
            }
        }

        public void SeatPassenger(int seat, Passenger passenger)
        {
           Carriages[seat-1].AddPassenger(passenger);
        }

        private void AddAllCarriageTypes()
        {
            int defaultCountCarriages = 0;
            _countCarriages = new Dictionary<CarriageType, int>();

            for (int i = 0; i < (int)CarriageType.Lenght; i++)
            {
                _countCarriages.Add((CarriageType)i, defaultCountCarriages);
            }
        }
    }

    enum CarriageType
    {
        Luxury,
        Economclass,
        Compartment,
        Lenght
    }

    class Track
    {
        public Track(string beginCity, string endCity, DateTime beginTime, DateTime endTime)
        {
            BeginCity = beginCity;
            EndCity = endCity;
            BeginTime = beginTime;
            EndTime = endTime;
        }

        public string BeginCity { get; protected set; }
        public string EndCity { get; protected set; }
        public DateTime BeginTime { get; protected set; }
        public DateTime EndTime { get; protected set; }

        public virtual void ShowInfo()
        {
            Console.WriteLine(BeginCity + " - " + EndCity + ". Отправляется в " + BeginTime + ". Приезд в " + EndTime); 
        }
    }

    class Depot
    {
        private Queue<Track> _tracks=new Queue<Track>();
        private List<Ticket> _tickets;
        private List<Passenger> _passengers;
        private Random _random = new Random();
        Dictionary<CarriageType, int> _countPassengersCarriage;
        Dictionary<CarriageType, int> _countFreeSeatsCarriage;

        public Depot()
        {
            CreateTracks();
        }

        public void SendTrain()
        {
            Track track = _tracks.Dequeue();
            AddAllCarriageTypes();
            CreatePassengers();
            Train train = new Train(_countPassengersCarriage);

            CreateTickets(train.Carriages, track);
            BuyTickets(train);
            ChoiceShowInfo(train,track);
        }

        public bool IsHaveTracks()
        {
            return _tracks.Count > 0;
        }

        private void ChoiceShowInfo(Train train, Track track)
        {
            const string ShowTrackInfoCommand = "1";
            const string ShowInfoCarriagesCommand = "2";
            const string ShowPassengersCarriageInfoCommand = "3";
            const string ShowCountPassengersCarriagesCommand = "4";
            const string ExitCommand = "5";

            bool isWork = true;

            while (isWork)
            {
                Console.Clear();
                Console.WriteLine(ShowTrackInfoCommand + ". Вывести путь поезда");
                Console.WriteLine(ShowInfoCarriagesCommand + ". Вывести информацию по вагонам");
                Console.WriteLine(ShowPassengersCarriageInfoCommand + ". Вывести информацию по пассажирам определенного вагона");
                Console.WriteLine(ShowCountPassengersCarriagesCommand + ". Вывести суммарное количество пассажиров каждого типа вагона");
                Console.WriteLine(ExitCommand + ". Отправить поезд");

                switch (Console.ReadLine())
                {
                    case ShowTrackInfoCommand:
                        track.ShowInfo();
                        break;
                    case ShowInfoCarriagesCommand:
                        train.ShowInfoCarriages();
                        break;
                    case ShowPassengersCarriageInfoCommand:
                        ShowPassengersCarriageInfo(train);
                        break;
                    case ShowCountPassengersCarriagesCommand:
                        ShowCountPassengersCarriages();
                        break;
                    case ExitCommand:
                        Console.WriteLine("Поезд уехал!");
                        isWork = false;
                        break;
                    default:
                        Console.WriteLine("Введена неверна команда");
                        break;
                }

                Console.ReadKey();
            }
        }

        private void ShowPassengersCarriageInfo(Train train)
        {
            Console.Write("Введите номер вагона: ");

            if (int.TryParse(Console.ReadLine(),out int carriageNumber))
            {
                if (carriageNumber>0 && carriageNumber<=train.Carriages.Count)
                {
                    train.Carriages[carriageNumber - 1].ShowInfo();
                }
                else
                {
                    Console.WriteLine("Такого вагона нет");
                }
            }
            else
            {
                Console.WriteLine("Введено неверное значение");
            }
        }

        private void ShowCountPassengersCarriages()
        {
            foreach (var carriagePassengers in _countPassengersCarriage)
            {
                Console.WriteLine(carriagePassengers.Key + " - " + carriagePassengers.Value + " пассажиров");
            }
        }

        private void CreatePassengers()
        {
            int minCountPassengers = 50;
            int maxCountPassengers = 500;
            int minCarriageType = 0;
            int maxCarriageType = (int)CarriageType.Lenght - 1;
            int minPassengerAge = 18;
            int maxPassengerAge = 45;
            CarriageType wishingCarriageType;
            string passengerName;
            int passengerAge;
            int countPassengers = _random.Next(minCountPassengers, maxCountPassengers + 1);
            _passengers = new List<Passenger>();

            for (int i = 1; i <= countPassengers; i++)
            {
                passengerName = "Иванов" + i + " Иван" + i + " Иванович" + i;
                passengerAge = _random.Next(minPassengerAge, maxPassengerAge + 1);
                wishingCarriageType = (CarriageType)_random.Next(minCarriageType, maxCarriageType + 1);
                _countPassengersCarriage[wishingCarriageType]++;
                _passengers.Add(new Passenger(passengerName, passengerAge, wishingCarriageType));
            }
        }

        private void CreateTickets(List<Carriage> carriages, Track track)
        {
            _tickets = new List<Ticket>();

            for (int i = 0; i < carriages.Count; i++)
            {
                for (int j = 0; j < carriages[i].CountSeats; j++)
                {
                    Ticket ticket = new Ticket(track, carriages[i].Type, carriages[i].Number, j + 1);
                    _tickets.Add(ticket);
                    _countFreeSeatsCarriage[carriages[i].Type]++;
                }
            }
        }

        private void BuyTickets(Train train)
        {
            int seatNumber;
            int minSeatNumber = 1;
            CarriageType carriageTypePassenger;

            for (int i = 0; i < _passengers.Count; i++)
            {
                carriageTypePassenger = _passengers[i].WishingCarriageType;
                seatNumber = _random.Next(minSeatNumber, _countFreeSeatsCarriage[carriageTypePassenger] + 1);

                if (TryFindTicket(seatNumber, carriageTypePassenger, out Ticket ticket))
                {
                    ticket.CheckIn(_passengers[i]);
                    _passengers[i].BuyTicket(ticket);
                    train.SeatPassenger(ticket.CarriageNumber, _passengers[i]);
                    _tickets.Remove(ticket);
                    _countFreeSeatsCarriage[carriageTypePassenger]--;
                }
            }
        }

        private bool TryFindTicket(int seatNumber, CarriageType carriageType, out Ticket ticket)
        {
            int seat = 1;

            for (int i = 0; i < _tickets.Count; i++)
            {
                if (_tickets[i].CarriageType == carriageType)
                {
                    if (seat == seatNumber)
                    {
                        ticket = _tickets[i];
                        return true;
                    }
                    else if (seat < seatNumber)
                    {
                        seat++;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка программы. Выход за пределы поиска свободных билетов");
                        Console.ReadKey();
                        break;
                    }
                }
            }

            ticket = null;
            return false;
        }

        private void AddAllCarriageTypes()
        {
            int defaultCountPassengers = 0;
            int defaultCountSeats = 0;
            _countPassengersCarriage = new Dictionary<CarriageType, int>();
            _countFreeSeatsCarriage = new Dictionary<CarriageType, int>();

            for (int i = 0; i < (int)CarriageType.Lenght; i++)
            {
                _countPassengersCarriage.Add((CarriageType)i, defaultCountPassengers);
                _countFreeSeatsCarriage.Add((CarriageType)i, defaultCountSeats);
            }
        }

        private void CreateTracks()
        {
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Москва", new DateTime(2023, 2, 1, 7, 0, 0), new DateTime(2023, 2, 1, 9, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Тверь", new DateTime(2023, 2, 1, 9, 0, 0), new DateTime(2023, 2, 1, 20, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Омск", new DateTime(2023, 2, 1, 11, 0, 0), new DateTime(2023, 2, 2, 5, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Москва", new DateTime(2023, 2, 1, 13, 0, 0), new DateTime(2023, 2, 1, 15, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Нижний Новгород", new DateTime(2023, 2, 1, 15, 0, 0), new DateTime(2023, 2, 1, 19, 0, 0)));
        }
    }

    class Ticket
    {
        private bool _isActive;
        private string _name;
        private int _age;
        private Track _track;

        public Ticket(Track track, CarriageType carriageType, int carriageNumber, int seat)
        {
            _isActive = false;
            CarriageType = carriageType;
            CarriageNumber = carriageNumber;
            Seat = seat;
            _track = track;
        }

        public int Seat { get; protected set; }
        public int CarriageNumber { get; protected set; }
        public CarriageType CarriageType { get; protected set; }

        public void ShowInfo()
        {
            if (_isActive)
            {
                Console.Write("Билет на имя: " + _name + ", " + _age + " лет. ");
            }
            else
            {
                Console.WriteLine("Билет не продан!");
            }

            Console.WriteLine("Вагон номер " + CarriageNumber + ", тип вагона - " + CarriageType + ", место " + Seat);
            _track.ShowInfo();
        }

        public void CheckIn(Passenger passenger)
        {
            _isActive = true;
            _name = passenger.Name;
            _age = passenger.Age;
        }

        public void SetCarriageType(CarriageType carriageType)
        {
            CarriageType = carriageType;
        }
    }
}
