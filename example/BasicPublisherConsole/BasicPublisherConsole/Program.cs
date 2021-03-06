﻿/*
 Created By Musa Demir.
 */
using EventManager.Core;
using EventManager.Core.interfaces;
using EventManager.DefaultManager.Extensions;
using EventManager.EventChecker.Extensions;
using EventManager.Shared.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
/// <summary>
/// This is a basic publisher console application. Node-red listens rabbitMq.
/// </summary>
namespace BasicPublisherConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddEMDefaultManager(
               checkerOptions => checkerOptions.UseSQL(sql => 
               sql.UseSqlServer("Data Source=LAPTOP-3O58F4FN;database=eventDB;trusted_connection=yes;")));

            var provider = services.BuildServiceProvider();

            provider.InitializeEMDefaultManager("localhost");

            var _eventPublisher = provider.GetService<IEMPublisher>();

            var key = ConsoleKey.Spacebar;
            int i = 0;
            do
            {
                string message = key.ToString() + " pressed . i:" + i++;

                try
                {
                    Task task = null;
                    switch (i % 3)
                    {
                        case 0:
                            task = Task.Run(async () =>
                               {
                                   //this event will publish by its own name (because there is no FBMEventInfo attribute on TestEvent class)
                                   await _eventPublisher.PublishAsync(new TestEvent { TestProp = "Pressed Key: " + message });
                               });
                            break;
                        case 1:
                            task = Task.Run(async () =>
                               {
                                   //UserCreatedEvent
                                   await _eventPublisher.PublishAsync(new TestEvent2 { userid = i, userTc = "tc" + i });
                               });
                            break;
                        case 2:
                            task = Task.Run(async () =>
                               {
                                   //UserDeletedEvent
                                   await _eventPublisher.PublishAsync(new MyEventClass { id = i, email = "email" + i, username = "user" + i });
                               });
                            break;
                    }
                    task.Wait();
                }
                catch (Exception e)
                {
                    throw e;
                }
                key = Console.ReadKey().Key;
            } while (key != ConsoleKey.Escape);

        }
    }

    class TestEvent : IEMEvent
    {
        public string TestProp { get; set; }
    }

    [EMEventInfo(eventName: "UserCreatedEvent")]
    class TestEvent2 : IEMEvent
    {
        public int userid { get; set; }
        public string userTc { get; set; }
    }
    [EMEventInfo(eventName: "UserDeletedEvent")]
    class MyEventClass : IEMEvent
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }
}
