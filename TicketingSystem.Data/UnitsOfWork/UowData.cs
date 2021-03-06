﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TicketingSystem.Data.Repositories;
using TicketingSystem.Models;

namespace TicketingSystem.Data.UnitsOfWork
{
	public class UowData : IUowData
	{
		private readonly DbContext context;
		private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();

		public UowData()
			: this(new AppDbContext())
		{
		}

		public UowData(DbContext context)
		{
			this.context = context;
		}

		public int SaveChanges()
		{
			return this.context.SaveChanges();
		}
  
		public void Dispose()
		{
			this.context.Dispose();
		}
  
		public IRepository<Category> Categories
		{
			get
			{
				return this.GetRepository<Category>();
			}
		}
  
		public IRepository<Ticket> Tickets
		{
			get
			{
				return this.GetRepository<Ticket>();
			}
		}
  
		public IRepository<Comment> Comments
		{
			get
			{
				return this.GetRepository<Comment>();
			}
		}

		public IRepository<AppUser> Users
		{
			get
			{
				return this.GetRepository<AppUser>();
			}
		}

		public IRepository<Attachment> Attachments
		{
			get
			{
				return this.GetRepository<Attachment>();
			}
		}
  
		private IRepository<T> GetRepository<T>() where T : class
		{
			if (!this.repositories.ContainsKey(typeof(T)))
			{
				var type = typeof(GenericRepository<T>);

				this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.context));
			}

			return (IRepository<T>)this.repositories[typeof(T)];
		}
	}
}