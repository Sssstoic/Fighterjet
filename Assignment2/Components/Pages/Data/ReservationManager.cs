﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2.Components.Pages.Data
{
    internal class ReservationManager
    {
        private static string Reservation_TXT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\Resources\Files\reservation.csv");
        private static Random random = new Random();
        private static List<Reservation> reservations = new List<Reservation>();

        public List<Reservation> FindReservations(string reservationCode, string airline, string name)
        {
            return reservations.Where(reservation =>
                (reservation.Code.Contains(reservationCode) || reservation.Airline.Contains(airline) || reservation.Name.Contains(name)) ||
                (reservation.Name.ToUpper().Contains(name) || reservation.Airline.ToUpper().Contains(airline))
            ).ToList();
        }

        public string GenerateResCode()
        {
            return GenerateReservationCode();
        }

        public string GenerateReservationCode()
        {
            string reservationCode;

            do
            {
                char letter = (char)('A' + random.Next(26));
                string numbers = random.Next(1000, 10000).ToString();
                reservationCode = letter + numbers;
            } while (IsCodeGenerated(reservationCode));

            return reservationCode;
        }

        private static bool IsCodeGenerated(string reservationCode)
        {
            if (!File.Exists(Reservation_TXT))
                return false;

            List<string> existingCode = File.ReadAllLines(Reservation_TXT).ToList();

            return existingCode.Contains(reservationCode);
        }

        public static List<Reservation> GetReservations()
        {
            List<Reservation> res = new List<Reservation>();

            foreach (string line in File.ReadLines(Reservation_TXT))
            {
                string[] parts = line.Split(",");
                string reservationCode = parts[0];
                string flightCode = parts[1];
                string airline = parts[2];
                double cost = double.Parse(parts[3]);
                string name = parts[4];
                string citizenship = parts[5];
                string status = parts[6];

                Reservation newReservation = new Reservation(reservationCode, flightCode, airline, cost, name, citizenship, status);
                res.Add(newReservation);
            }

            reservations = res;
            return res;
        }

        public void AddReservation(Reservation res)
        {
            File.AppendAllText(Reservation_TXT, $"{res.Code},{res.FlightCode},{res.Airline},{res.Cost},{res.Name},{res.Citizenship},{res.Active}\n");
        }

        public void UpdateReservation(Reservation res)
        {
            var lines = File.ReadAllLines(Reservation_TXT).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(",");

                if (parts[0] == res.Code)
                {
                    parts[6] = "Cancelled";
                    lines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(Reservation_TXT, lines);
        }
    }
}
