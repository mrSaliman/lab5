﻿using lab5.Areas.FurnitureFactory.Models;

namespace lab5.Areas.FurnitureFactory.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AcmeDataContext db)
        {
            db.Database.EnsureCreated();

            if (db.OrderDetails.Any())
            {
                return;   // База данных инициализирована
            }

            const int customersNumber = 35;
            const int employeesNumber = 35;
            const int furnituresNumber = 35;
            const int ordersNumber = 300;
            const int orderDetailsNumber = 300;
            
            Random randObj = new(1);

            string[] personFirstVoc = { "Дмитрий", "Евгений", "Даниил", "Денис", "Василий" };
            string[] personLastVoc = { "Валуев", "Иванов", "Бондарев", "Демиденко", "Ермолин" };
            string[] personMiddleVoc = { "Осипович", "Александрович", "Дмитриевич", "Абхазьевич", "Сергеевич" };
            string[] materialVoc = { "Сталь", "Платина", "Алюминий", "ПЭТ", "Чугун", "Алюминий", "Сталь" };
            string[] positionVoc = { "Администратор", "Механик", "Супервайзер", "Продавец", "Уборщик" };
            string[] educationVoc = { "Среднее", "Высшее", "Нет" };

            for (int i = 1; i <= customersNumber; i++)
            {
                var customer = new Customer
                {
                    CompanyName = $"Company {i}",
                    RepresentativeLastName = personLastVoc[randObj.Next(personLastVoc.Length)],
                    RepresentativeFirstName = personFirstVoc[randObj.Next(personFirstVoc.Length)],
                    RepresentativeMiddleName = personMiddleVoc[randObj.Next(personMiddleVoc.Length)],
                    PhoneNumber = $"123-456-789{i.ToString().PadLeft(2, '0')}",
                    Address = $"Address {i}"
                };
                db.Customers.Add(customer);
            }

            db.SaveChanges();
            
            for (int i = 1; i <= employeesNumber; i++)
            {
                var employee = new Employee
                {
                    LastName = personLastVoc[randObj.Next(personLastVoc.Length)],
                    FirstName = personFirstVoc[randObj.Next(personFirstVoc.Length)],
                    MiddleName = personMiddleVoc[randObj.Next(personMiddleVoc.Length)],
                    Position = positionVoc[randObj.Next(positionVoc.Length)],
                    Education = educationVoc[randObj.Next(educationVoc.Length)]
                };
                db.Employees.Add(employee);
            }
            
            db.SaveChanges();

            for (int i = 1; i <= furnituresNumber; i++)
            {
                var furniture = new Furniture
                {
                    FurnitureName = $"Furniture {i}",
                    Description = $"Description {i}",
                    MaterialType = materialVoc[randObj.Next(materialVoc.Length)],
                    Price = randObj.Next(100, 1000),
                    QuantityOnHand = randObj.Next(1, 100)
                };
                db.Furnitures.Add(furniture);
            }
            
            db.SaveChanges();

            for (int i = 1; i <= ordersNumber; i++)
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now.AddDays(-randObj.Next(1, 365)),
                    CustomerId = randObj.Next(1, customersNumber),
                    SpecialDiscount = randObj.Next(0, 25),
                    IsCompleted = randObj.Next(0, 2) == 1,
                    ResponsibleEmployeeId = randObj.Next(1, employeesNumber)
                };
                db.Orders.Add(order);
            }
            
            db.SaveChanges();

            for (int i = 1; i <= orderDetailsNumber; i++)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = randObj.Next(1, ordersNumber),
                    FurnitureId = randObj.Next(1, furnituresNumber),
                    Quantity = randObj.Next(1, 10)
                };
                db.OrderDetails.Add(orderDetail);
            }
            db.SaveChanges();
        }

    }

}

