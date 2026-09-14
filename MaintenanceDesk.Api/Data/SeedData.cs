using MaintenanceDesk.Api.Domain;

namespace MaintenanceDesk.Api.Data;

public static class SeedData
{
    public static class Ids
    {
        public static readonly Guid Tallbacken = new("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Hamnhuset = new("10000000-0000-0000-0000-000000000002");

        public static readonly Guid Tallbacken1001 = new("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Tallbacken1101 = new("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Tallbacken1201 = new("20000000-0000-0000-0000-000000000003");
        public static readonly Guid Hamnhuset1001 = new("20000000-0000-0000-0000-000000000004");
        public static readonly Guid Hamnhuset1102 = new("20000000-0000-0000-0000-000000000005");
        public static readonly Guid Hamnhuset1301 = new("20000000-0000-0000-0000-000000000006");

        public static readonly Guid AnnaLindqvist = new("30000000-0000-0000-0000-000000000001");
        public static readonly Guid ErikJohansson = new("30000000-0000-0000-0000-000000000002");
        public static readonly Guid SaraNilsson = new("30000000-0000-0000-0000-000000000003");
        public static readonly Guid OmarHaddad = new("30000000-0000-0000-0000-000000000004");

        public static readonly Guid JohanBerg = new("40000000-0000-0000-0000-000000000001");
        public static readonly Guid MariaHolm = new("40000000-0000-0000-0000-000000000002");
        public static readonly Guid LarsEk = new("40000000-0000-0000-0000-000000000003");
    }

    public static readonly Property[] Properties =
    [
        new() { Id = Ids.Tallbacken, Name = "Tallbacken", StreetAddress = "Tallbacksvägen 12", PostalCode = "752 36", City = "Uppsala" },
        new() { Id = Ids.Hamnhuset, Name = "Hamnhuset", StreetAddress = "Hamngatan 8", PostalCode = "211 22", City = "Malmö" },
    ];

    // Swedish apartment numbering
    public static readonly Unit[] Units =
    [
        new() { Id = Ids.Tallbacken1001, PropertyId = Ids.Tallbacken, Designation = "1001", Floor = 0 },
        new() { Id = Ids.Tallbacken1101, PropertyId = Ids.Tallbacken, Designation = "1101", Floor = 1 },
        new() { Id = Ids.Tallbacken1201, PropertyId = Ids.Tallbacken, Designation = "1201", Floor = 2 },
        new() { Id = Ids.Hamnhuset1001, PropertyId = Ids.Hamnhuset, Designation = "1001", Floor = 0 },
        new() { Id = Ids.Hamnhuset1102, PropertyId = Ids.Hamnhuset, Designation = "1102", Floor = 1 },
        new() { Id = Ids.Hamnhuset1301, PropertyId = Ids.Hamnhuset, Designation = "1301", Floor = 3 },
    ];

    public static readonly Resident[] Residents =
    [
        new() { Id = Ids.AnnaLindqvist, UnitId = Ids.Tallbacken1101, FullName = "Anna Lindqvist", Email = "anna.lindqvist@example.com", PhoneNumber = "070-1740605" },
        new() { Id = Ids.ErikJohansson, UnitId = Ids.Tallbacken1201, FullName = "Erik Johansson", Email = "erik.johansson@example.com", PhoneNumber = "070-1740612" },
        new() { Id = Ids.SaraNilsson, UnitId = Ids.Hamnhuset1001, FullName = "Sara Nilsson", Email = "sara.nilsson@example.com" },
        new() { Id = Ids.OmarHaddad, UnitId = Ids.Hamnhuset1301, FullName = "Omar Haddad", Email = "omar.haddad@example.com", PhoneNumber = "070-1740623" },
    ];

    public static readonly Technician[] Technicians =
    [
        new() { Id = Ids.JohanBerg, FullName = "Johan Berg", Email = "johan.berg@example.com", Trade = "Plumber" },
        new() { Id = Ids.MariaHolm, FullName = "Maria Holm", Email = "maria.holm@example.com", Trade = "Electrician" },
        new() { Id = Ids.LarsEk, FullName = "Lars Ek", Email = "lars.ek@example.com", Trade = "Caretaker" },
    ];
}
