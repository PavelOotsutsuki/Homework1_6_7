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
        public Passenger (string name, int age, CarriageType carriageType)
        {
            Name = name;
            Age = age;
            CarriageType = carriageType;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
        public CarriageType CarriageType { get; private set; }
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
        protected List<Passenger> Passengers = new List<Passenger>();

        public Carriage()
        {
            Random = new Random();
        }

        public int CountSeats { get; protected set; }
        public int CarriageNumber { get; protected set; }
        public CarriageType CarriageType { get; protected set; }
        public string Title { get; protected set; }

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
            CarriageType = CarriageType.Luxury;
            CountSeats = Random.Next(MinCountSeats, MaxCountSeats+1);
            CarriageNumber = carriageNumber;
            Title = "СВ " + CarriageNumber;
        }
    }

    class EconomclassCarriage: Carriage
    {
        public EconomclassCarriage(int carriageNumber) : base()
        {
            MinCountSeats = 20;
            MaxCountSeats = 40;
            CarriageType = CarriageType.Economclass;
            CountSeats = Random.Next(MinCountSeats, MaxCountSeats+1);
            CarriageNumber = carriageNumber;
            Title = "Эконом-класс " + CarriageNumber;
        }
    }

    class CompartmentCarriage: Carriage
    {
        public CompartmentCarriage(int carriageNumber) : base()
        {
            MinCountSeats = 16;
            MaxCountSeats = 32;
            CarriageType = CarriageType.Compartment;

            do
            {
                CountSeats = Random.Next(MinCountSeats, MaxCountSeats + 1);
            } while (CountSeats % 4 !=0);

            CarriageNumber = carriageNumber;
            Title = "Купе " + CarriageNumber;
        }
    }

    class Train
    {
        private List<Ticket> _tickets = new List<Ticket>();
        private List<Passenger> _passengers = new List<Passenger>();
        private List<Carriage> _carriages = new List<Carriage>();
        private Random _random = new Random();
        private Track _track;
        private int _countPassengers;
        Dictionary<CarriageType, int> _countPassengersCarriage = new Dictionary<CarriageType, int>();
        Dictionary<CarriageType, int> _countFreeSeatsCarriage = new Dictionary<CarriageType, int>();

        public Train (Track track)
        {
            _track = track;

            AddAllCarriageTypes();
            CreatePassengers();
            CreateCarriages();
            CreateTickets();
            BuyTickets();
        }

        public void ShowInfo()
        {
            foreach (var carriage in _carriages)
            {
                Console.WriteLine(carriage.Title + ":");
                carriage.ShowInfo();
                Console.WriteLine();
                Console.ReadKey();
            }
        }

        private void AddAllCarriageTypes()
        {
            int defaultCountPassengers = 0;
            int defaultCountSeats = 0;

            for (int i = 0; i < (int)CarriageType.Lenght; i++)
            {
                _countPassengersCarriage.Add((CarriageType)i, defaultCountPassengers);
                _countFreeSeatsCarriage.Add((CarriageType)i, defaultCountSeats);
            }
        }

        private void CreateCarriages()
        {
            int carriageNumber=1;
            int countPassengersCarriage;

            for (int i = 0; i < (int)CarriageType.Lenght; i++)
            {
                countPassengersCarriage = 0;

                while (countPassengersCarriage < _countPassengersCarriage[(CarriageType)i])
                {
                    if ((CarriageType)i== CarriageType.Luxury)
                    {
                        _carriages.Add(new LuxuryCarriage(carriageNumber));
                    }
                    else if ((CarriageType)i == CarriageType.Economclass)
                    {
                        _carriages.Add(new EconomclassCarriage(carriageNumber));
                    }
                    else if ((CarriageType)i == CarriageType.Compartment)
                    {
                        _carriages.Add(new CompartmentCarriage(carriageNumber));
                    }

                    countPassengersCarriage += _carriages[carriageNumber - 1].CountSeats;
                    carriageNumber++;
                }
            }
        }

        private void CreateTickets()
        {
            for (int i = 0; i < _carriages.Count; i++)
            {
                for (int j = 0; j < _carriages[i].CountSeats; j++)
                {
                    Ticket ticket = new Ticket(_track.BeginCity, _track.EndCity, _track.BeginTime, _track.EndTime, _carriages[i].CarriageType, _carriages[i].CarriageNumber,j+1 );
                    _tickets.Add(ticket);
                    _countFreeSeatsCarriage[_carriages[i].CarriageType]++;
                }
            }
        }

        private void BuyTickets()
        {
            int seatNumber;
            int minSeatNumber = 1;
            CarriageType carriageTypePassenger;

            for (int i = 0; i < _countPassengers; i++)
            {
                carriageTypePassenger = _passengers[i].CarriageType;
                seatNumber = _random.Next(minSeatNumber, _countFreeSeatsCarriage[carriageTypePassenger] + 1);

                if (TryFindTicket(seatNumber, carriageTypePassenger, out Ticket ticket))
                { 
                    ticket.CheckIn(_passengers[i]);
                    _passengers[i].BuyTicket(ticket);
                    _carriages[ticket.CarriageNumber - 1].AddPassenger(_passengers[i]);
                    _tickets.Remove(ticket);
                    _countFreeSeatsCarriage[carriageTypePassenger]--;
                }
            }
        }

        private bool TryFindTicket(int seatNumber, CarriageType carriageType, out Ticket ticket)
        {
            int seat = 1;

            for (int i=0;i< _tickets.Count;i++)
            {
                if (_tickets[i].CarriageType== carriageType)
                {
                    if (seat== seatNumber)
                    {
                        ticket = _tickets[i];
                        return true;
                    }
                    else if (seat< seatNumber)
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

        private void CreatePassengers()
        {
            int minCountPassengers = 50;
            int maxCountPassengers = 500;
            int minCarriageType = 0;
            int maxCarriageType = (int)CarriageType.Lenght-1;
            int minPassengerAge = 18;
            int maxPassengerAge = 45;
            CarriageType carriageType;
            string passengerName;
            int passengerAge;

            _countPassengers = _random.Next(minCountPassengers, maxCountPassengers + 1);

            for (int i = 1; i <= _countPassengers; i++)
            {
                passengerName = "Иванов" + i + " Иван" + i + " Иванович" + i;
                passengerAge = _random.Next(minPassengerAge, maxPassengerAge + 1);
                carriageType = (CarriageType)_random.Next(minCarriageType, maxCarriageType+1);
                _countPassengersCarriage[carriageType]++;
                _passengers.Add(new Passenger(passengerName, passengerAge, carriageType));
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

        public Depot()
        {
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Москва", new DateTime(2023, 2, 1, 7, 0, 0), new DateTime(2023, 2, 1, 9, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Тверь", new DateTime(2023, 2, 1, 9, 0, 0), new DateTime(2023, 2, 1, 20, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Омск", new DateTime(2023, 2, 1, 11, 0, 0), new DateTime(2023, 2, 2, 5, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Москва", new DateTime(2023, 2, 1, 13, 0, 0), new DateTime(2023, 2, 1, 15, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Нижний Новгород", new DateTime(2023, 2, 1, 15, 0, 0), new DateTime(2023, 2, 1, 19, 0, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Волгоград", new DateTime(2023, 2, 1, 17, 0, 0), new DateTime(2023, 2, 3, 9, 30, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Воронеж", new DateTime(2023, 2, 1, 22, 0, 0), new DateTime(2023, 2, 4, 6, 0, 0)));
            _tracks.Enqueue(new Track("Санкт-Петебрург", "Череповец", new DateTime(2023, 2, 2, 6, 30, 0), new DateTime(2023, 2, 3, 15, 30, 0)));
        }

        public void SendTrain()
        {
            Track track = _tracks.Dequeue();
            Train train = new Train(track);

            train.ShowInfo();
            Console.WriteLine("Поезд уехал!");
            Console.ReadKey();
        }

        public bool IsHaveTracks()
        {
            if (_tracks.Count>0)
            {
                return true;
            }

            return false;
        }
    }

    class Ticket: Track
    {
        private bool _isActive;
        private string _name;
        private int _age;

        public Ticket(string beginCity, string endCity, DateTime beginTime, DateTime endTime, CarriageType carriageType, int carriageNumber, int seat) : base(beginCity, endCity, beginTime, endTime)
        {
            _isActive = false;
            CarriageType = carriageType;
            CarriageNumber = carriageNumber;
            Seat = seat;
        }

        public int Seat { get; protected set; }
        public int CarriageNumber { get; protected set; }
        public CarriageType CarriageType { get; protected set; }

        public override void ShowInfo()
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
            base.ShowInfo();
        }

        public void CheckIn(Passenger passenger)
        {
            _isActive = true;
            _name = passenger.Name;
            _age = passenger.Age;
        }
    }
}
