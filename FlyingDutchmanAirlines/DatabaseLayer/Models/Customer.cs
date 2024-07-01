﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models;

public sealed class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public Customer(string name)
    {
        Name = name;
    }

    public static bool operator == (Customer? x, Customer? y)
    {
        CustomerEqualityComparer comparer = new CustomerEqualityComparer();
        return comparer.Equals(x, y);
    }

    public static bool operator != (Customer? x, Customer? y) => !(x == y);

    internal class CustomerEqualityComparer : EqualityComparer<Customer>
    {
        public override bool Equals(Customer? x, Customer? y)
        {
            return (x.CustomerId == y.CustomerId) && (x.Name == y.Name);
        }

        public override int GetHashCode(Customer obj)
        {
            int randomNumber = RandomNumberGenerator.GetInt32(Int32.MaxValue / 2);
            return (obj.CustomerId + obj.Name.Length + randomNumber).GetHashCode();
        }
    }
}
